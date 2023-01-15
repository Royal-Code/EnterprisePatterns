using RoyalCode.Extensions.PropertySelection;
using RoyalCode.Searches.Abstractions;
using System.Linq.Expressions;
using System.Reflection;

namespace RoyalCode.Persistence.Searches.Abstractions.Linq.Filter;

public class DefaultSpecifierFunctionGenerator : ISpecifierFunctionGenerator
{
    public Func<IQueryable<TModel>, TFilter, IQueryable<TModel>>? Generate<TModel, TFilter>()
        where TModel : class
        where TFilter : class
    {
        // creates criterion resolution for each filter type properties.
        var criterionResolutions = typeof(TFilter).GetProperties()
            .Select(p => new CriterionResolution(p, p.GetCustomAttribute<CriterionAttribute>()));

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
            .Where(cr => cr.TargetSelection is null && !cr.Criterion.Ignore)
            .Join(selectionMatch.PropertyMatches,
                cr => cr.FilterPropertyInfo.Name,
                sm => sm.OriginProperty.Name,
                (cr, sm) => (cr, sm));

        // for each unconfigured property, creates a property selection for the model type.
        foreach (var (criterionResolution, propertyMatch) in unconfiguredProperties)
            if (propertyMatch.Match && propertyMatch.TypeMatch)
                criterionResolution.TargetSelection = propertyMatch.TargetSelection;

        // get all criterion resolutions that must have a property selection.
        var allProperties = criterionResolutions
            .Where(cr => !cr.Criterion.Ignore)
            .ToList();

        // check if all resolution that are not setted to ignore have a property selection.
        if (allProperties.Any(cr => cr.TargetSelection is null))
            return null;

        // creates a function to apply the filter in a query.
        return Create<TModel, TFilter>(allProperties);
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
            var targetParam = Expression.Parameter(resolution.TargetSelection!.RootDeclaringType);
            var targetMemberAccess = resolution.TargetSelection!.GetAccessExpression(targetParam);
            var operatorExpression = CreateOperatorExpression(
                resolution.Criterion.Operator, 
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

            body.Add(assign);
        }

        // build the expression of the lambda function to apply the filters.
        var funcType = typeof(Func<,,>).MakeGenericType(
            typeof(IQueryable<TModel>),
            typeof(TFilter),
            typeof(IQueryable<TModel>));

        var bodyBlock = Expression.Block(body);

        var lambdaFunc = Expression.Lambda(funcType, bodyBlock, queryParam, filterParam);

        // compile the lambda function.
        return (Func<IQueryable<TModel>, TFilter, IQueryable<TModel>>?)lambdaFunc.Compile();
    }

    private Expression CreateOperatorExpression(
        CriterionOperator @operator,
        MemberExpression filterMemberAccess,
        MemberExpression targetMemberAccess)
    {
        switch (@operator)
        {
            case CriterionOperator.Equal:
                return Expression.Equal(targetMemberAccess, filterMemberAccess);
            case CriterionOperator.NotEqual:
                return Expression.NotEqual(targetMemberAccess, filterMemberAccess);
            case CriterionOperator.GreaterThan:
                return Expression.GreaterThan(targetMemberAccess, filterMemberAccess);
            case CriterionOperator.GreaterThanOrEqual:
                return Expression.GreaterThanOrEqual(targetMemberAccess, filterMemberAccess);
            case CriterionOperator.LessThan:
                return Expression.LessThan(targetMemberAccess, filterMemberAccess);
            case CriterionOperator.LessThanOrEqual:
                return Expression.LessThanOrEqual(targetMemberAccess, filterMemberAccess);
            case CriterionOperator.Like:
                return Expression.Call(
                    targetMemberAccess,
                    typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                    filterMemberAccess);
            case CriterionOperator.NotLike:
                return Expression.Not(Expression.Call(
                    targetMemberAccess,
                    typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                    filterMemberAccess));
            case CriterionOperator.IsNull:
                return Expression.Equal(targetMemberAccess, Expression.Constant(null));
            case CriterionOperator.IsNotNull:
                return Expression.NotEqual(targetMemberAccess, Expression.Constant(null));
        }

        throw new NotSupportedException("Criterion operator not supported.");
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