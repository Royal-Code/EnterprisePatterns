
using System.Linq.Expressions;

namespace RoyalCode.Persistence.Searches.Abstractions.Linq.Selector;

/// <summary>
/// <para>
///     Default implementation of <see cref="ISelectorExpressionGenerator"/>.
/// </para>
/// <para>
///     This implementation can resolve selectors of plain types, 
///     where the DTO has properties with the same type of entity's properties.
/// </para>
/// </summary>
public sealed class DefaultSelectorExpressionGenerator : ISelectorExpressionGenerator
{
    /// <inheritdoc />
    public Expression<Func<TEntity, TDto>>? Generate<TEntity, TDto>()
        where TEntity : class
        where TDto : class
    {
        // get DTO constructor
        var ctor = typeof(TDto).GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0);
        if (ctor is null)
            return null;

        // create the match of the properties.
        var match = typeof(TDto).MatchProperties(typeof(TEntity));
        if (match.PropertyMatches.Any(m => !m.Match || !m.TypeMatch))
            return null;

        // create the input (entity) parameter.
        var parameter = Expression.Parameter(typeof(TEntity), "entity");

        // generate de bindings for create the new DTO
        var bindings = match.PropertyMatches
            .Select(m => Expression.Bind(m.OriginProperty, m.TargetSelection!.GetAccessExpression(parameter)));

        // create the new DTO with the bindings.
        var newDto = Expression.MemberInit(Expression.New(ctor), bindings);

        // generate the lambda expression
        return Expression.Lambda<Func<TEntity, TDto>>(newDto, parameter);
    }
}
