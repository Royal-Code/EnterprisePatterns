using RoyalCode.Extensions.PropertySelection;
using RoyalCode.Persistence.Searches.Abstractions.Extensions;
using RoyalCode.Searches.Abstractions;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace RoyalCode.Persistence.Searches.Abstractions.Linq.Filter;

/// <summary>
/// <para>
///     Generates a function that apply filters in a query.
/// </para>
/// </summary>
public class DefaultSpecifierFunctionGenerator : ISpecifierFunctionGenerator
{
    private static readonly MethodInfo IsEmptyMethod = typeof(IsEmptyExtension)
        .GetMethod(nameof(IsEmptyExtension.IsEmpty))!;

    private static readonly MethodInfo IsNullOrWhiteSpaceMethod =
        typeof(string).GetMethod(nameof(string.IsNullOrWhiteSpace))!;

    private static readonly MethodInfo AnyMethod = typeof(Enumerable).GetMethods()
        .Where(m => m.Name == nameof(Enumerable.Any))
        .Where(m => m.GetParameters().Count() == 1)
        .First();

    private static readonly MethodInfo IsBlankMethod = typeof(IsEmptyExtension)
        .GetMethod(nameof(IsEmptyExtension.IsBlank), new Type[] { typeof(DateTime) })!;

    private static readonly Type[] GreaterThenTypes = new Type[]
    {
        typeof(byte),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(float),
        typeof(double),
        typeof(decimal),
    };

    /// <inheritdoc />
    public Func<IQueryable<TModel>, TFilter, IQueryable<TModel>>? Generate<TModel, TFilter>()
        where TModel : class
        where TFilter : class
    {
        // creates criterion resolution for each filter type properties.
        var criterionResolutions = typeof(TFilter).GetProperties()
            .Select(p => new CriterionResolution(p, p.GetCustomAttribute<CriterionAttribute>(true)))
            .Where(cr => !cr.Criterion.Ignore)
            .ToList();

        // for each criterion resolution where the criterion attribute has a property name,
        // creates a property selection for the model type.
        var configuredProperties = criterionResolutions
            .Where(cr => cr.Criterion.TargetProperty is not null);
        foreach (var resolution in configuredProperties)
        {
            var propertySelection = typeof(TModel).TrySelectProperty(resolution.Criterion.TargetProperty!);
            if (propertySelection is null)
                return null;

            resolution.TargetSelection = propertySelection;
        }

        // match properties of filter type with properties of model type.
        var selectionMatch = typeof(TFilter).MatchProperties(typeof(TModel));

        // join the criterion resolutions with the selection match,
        // where the criterion resolution does not have a property selection,
        // and the criterion attribute is not setted to ignore the property.
        var unconfiguredProperties = criterionResolutions
            .Where(cr => cr.TargetSelection is null);
        foreach (var criterionResolution in unconfiguredProperties)
            foreach (var match in selectionMatch.PropertyMatches)
                if (match.OriginProperty.Name == criterionResolution.FilterPropertyInfo.Name
                    && match.Match && match.TypeMatch)
                {
                    criterionResolution.TargetSelection = match.TargetSelection;
                }

        // check if all resolution that are not setted to ignore have a property selection.
        if (criterionResolutions.Any(cr => cr.TargetSelection is null))
            return null;

        // creates a function to apply the filter in a query.
        return Create<TModel, TFilter>(criterionResolutions);
    }

    private Func<IQueryable<TModel>, TFilter, IQueryable<TModel>>? Create<TModel, TFilter>(
        List<CriterionResolution> allProperties)
        where TModel : class
        where TFilter : class
    {
        var filterParam = Expression.Parameter(typeof(TFilter), "filter");
        var queryParam = Expression.Parameter(typeof(IQueryable<TModel>), "query");
        List<Expression> body = new();

        foreach (var resolution in allProperties)
        {
            var filterMemberAccess = Expression.MakeMemberAccess(filterParam, resolution.FilterPropertyInfo);
            var targetParam = Expression.Parameter(resolution.TargetSelection!.RootDeclaringType, "e");
            var targetMemberAccess = GetMemberAccess(resolution.TargetSelection!, targetParam);
            var operatorExpression = CreateOperatorExpression(
                DiscoveryCriterionOperator(resolution.Criterion, resolution.FilterPropertyInfo),
                resolution.Criterion.Negation,
                filterMemberAccess,
                targetMemberAccess);

            // generate the type of the predicate.
            var predicateType = typeof(Func<,>).MakeGenericType(
                resolution.TargetSelection.RootDeclaringType,
                typeof(bool));

            // create the lamdba expression for the queryable
            var lambda = Expression.Lambda(predicateType, operatorExpression, targetParam);

            // create the method call to apply the filter in the query.
            var methodCall = Expression.Call(
                typeof(Queryable),
                "Where",
                new[] { resolution.TargetSelection.RootDeclaringType },
                queryParam,
                lambda);

            // assign the query with the value of the call.
            var assign = Expression.Assign(queryParam, methodCall);

            // create an expression to check if the filter property is empty
            body.Add(resolution.Criterion.IgnoreIfIsEmpty
                ? GetIfIsEmptyConstraintExpression(filterMemberAccess, assign) 
                : assign);
        }

        // build the expression of the lambda function to apply the filters.
        var funcType = typeof(Func<,,>).MakeGenericType(
            typeof(IQueryable<TModel>),
            typeof(TFilter),
            typeof(IQueryable<TModel>));

        body.Add(queryParam);
        var bodyBlock = Expression.Block(body);

        var lambdaFunc = Expression.Lambda(funcType, bodyBlock, queryParam, filterParam);

        // compile the lambda function.
        return (Func<IQueryable<TModel>, TFilter, IQueryable<TModel>>?)lambdaFunc.Compile();
    }

    private Expression CreateOperatorExpression(
        CriterionOperator @operator,
        bool negation,
        MemberExpression filterMemberAccess,
        MemberExpression targetMemberAccess)
    {
        Expression? expression = null;

        switch (@operator)
        {
            case CriterionOperator.Equal:
                return negation
                    ? Expression.NotEqual(targetMemberAccess, filterMemberAccess)
                    : Expression.Equal(targetMemberAccess, filterMemberAccess);
            case CriterionOperator.GreaterThan:
                expression = Expression.GreaterThan(targetMemberAccess, filterMemberAccess);
                break;
            case CriterionOperator.GreaterThanOrEqual:
                expression = Expression.GreaterThanOrEqual(targetMemberAccess, filterMemberAccess);
                break;
            case CriterionOperator.LessThan:
                expression = Expression.LessThan(targetMemberAccess, filterMemberAccess);
                break;
            case CriterionOperator.LessThanOrEqual:
                expression = Expression.LessThanOrEqual(targetMemberAccess, filterMemberAccess);
                break;
            case CriterionOperator.Like:
                expression = Expression.Call(
                    targetMemberAccess,
                    typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                    filterMemberAccess);
                break;
        }

        if (expression is null)
            throw new InvalidOperationException("The operator is not supported.");

        if (negation)
            expression = Expression.Not(expression);

        return expression;
    }

    /// <summary>
    /// Obtém o tipo de uma condição para um filtro a partir do <see cref="CriterionAttribute"/>.
    /// </summary>
    /// <param name="criterion"><see cref="CriterionAttribute"/>.</param>
    /// <param name="filterProperty">Propriedade de um filtro.</param>
    /// <returns>O tipo de condição que se deverá aplicar no filtro.</returns>
    public static CriterionOperator DiscoveryCriterionOperator(
        CriterionAttribute criterion,
        PropertyInfo filterProperty)
    {
        if (criterion.Operator != CriterionOperator.Auto)
        {
            return criterion.Operator;
        }
        if (filterProperty.PropertyType == typeof(string))
        {
            return CriterionOperator.Like;
        }
        //else if (typeof(IEnumerable).IsAssignableFrom(filterProperty.PropertyType))
        //{
        //    return CriterionOperator.In;
        //}
        //else if (PropertyFilter.GreaterThanOrEqualSuffix.Any(filterProperty.PropertyName.EndsWith))
        //{
        //    return CriterionOperator.GreaterThanOrEqual;
        //}
        //else if (PropertyFilter.GreaterThanSuffix.Any(filterProperty.PropertyName.EndsWith))
        //{
        //    return CriterionOperator.GreaterThan;
        //}
        //else if (PropertyFilter.LessThanOrEqualSuffix.Any(filterProperty.PropertyName.EndsWith))
        //{
        //    return CriterionOperator.LessThanOrEqual;
        //}
        //else if (PropertyFilter.LessThanSuffix.Any(filterProperty.PropertyName.EndsWith))
        //{
        //    return CriterionOperator.LessThan;
        //}

        return CriterionOperator.Equal;
    }
    
    /// <summary>
    /// Gets the expression to access the member, checking if it is a Nullable to get the Value if it is.
    /// </summary>
    /// <param name="propertySelection">The property selection.</param>
    /// <param name="parameterExpression">The parameter expression.</param>
    /// <returns>The expression to access the member.</returns>
    public static MemberExpression GetMemberAccess(
        PropertySelection propertySelection,
        Expression parameterExpression)
    {
        // expressão da propriedade
        return Nullable.GetUnderlyingType(propertySelection.PropertyType) == null
            ? propertySelection.GetAccessExpression(parameterExpression)
            : propertySelection.SelectChild("Value")!.GetAccessExpression(parameterExpression);
    }

    /// <summary>
    /// Cria uma expressão condicional para verificando se o valor do filtro não é vazio para execução de uma expressão.
    /// </summary>
    /// <param name="filterMemberAccess">Expressão que retorna o valor do filtro.</param>
    /// <param name="assignExpression">Expressão que será executada se a condição for verdadeira.</param>
    /// <returns>Nova expressão da condição, ou a mesma expressão caso não se possa aplicar uma condição.</returns>
    public static Expression GetIfIsEmptyConstraintExpression(
        Expression filterMemberAccess,
        Expression assignExpression)
    {
        Expression constraint;
        var type = filterMemberAccess.Type;
        if (type == typeof(string))
        {
            constraint = Expression.Not(Expression.Call(IsNullOrWhiteSpaceMethod, filterMemberAccess));
        }
        else if (type == typeof(IEnumerable))
        {
            constraint = Expression.Call(AnyMethod, filterMemberAccess);
        }
        else if (Nullable.GetUnderlyingType(type) != null)
        {
            constraint = Expression.MakeMemberAccess(filterMemberAccess, type.GetProperty("HasValue")!);
        }
        else if (GreaterThenTypes.Contains(type))
        {
            constraint = Expression.GreaterThan(filterMemberAccess, Expression.Default(type));
        }
        else if (type == typeof(DateTime))
        {
            constraint = Expression.Not(Expression.Call(IsBlankMethod, filterMemberAccess));
        }
        else if (type.IsClass)
        {
            constraint = Expression.NotEqual(filterMemberAccess, Expression.Constant(null, type));
        }
        else
        {
            constraint = Expression.Call(IsEmptyMethod, filterMemberAccess);
        }

        return Expression.IfThen(constraint, assignExpression);
    }
}

internal class CriterionResolution
{
    public CriterionResolution(PropertyInfo property, CriterionAttribute? criterionAttribute)
    {
        FilterPropertyInfo = property;
        Criterion = criterionAttribute ?? new CriterionAttribute();
    }

    public PropertyInfo FilterPropertyInfo { get; set; }

    public CriterionAttribute Criterion { get; set; }

    public PropertySelection? TargetSelection { get; set; }
}