using Microsoft.EntityFrameworkCore;
using RoyalCode.SmartSearch.Linq.Services;
using System.Linq.Expressions;

namespace RoyalCode.Repositories.EntityFramework;

/// <summary>
/// <para>
///     Utility class to project entities into DTOs in the context of the database,
///     caching the selection expression resolved from the <see cref="ISelectorFactory"/>.
/// </para>
/// <para>
///     The projection is applied to the query, so it is translated and executed by the provider,
///     without materializing or tracking the entity.
/// </para>
/// </summary>
/// <typeparam name="TEntity">Type of entity to be projected.</typeparam>
/// <typeparam name="TDto">Type of DTO to be selected.</typeparam>
public static class SelectDto<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    private static Expression<Func<TEntity, TDto>>? selectorExpression;

    /// <summary>
    /// <para>
    ///     Gets the selection expression to project an entity into a DTO.
    /// </para>
    /// <para>
    ///     The <see cref="ISelectorFactory"/> service from the database context will be used.
    ///     If it is not registered, an exception will be thrown.
    /// </para>
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <returns>An expression that represents the selection of DTO from the entity.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the <see cref="ISelectorFactory"/> is not registered in the service provider of the
    ///     <see cref="DbContext"/>, or if there is no selector for the entity and DTO types.
    /// </exception>
    public static Expression<Func<TEntity, TDto>> GetSelector(DbContext db)
    {
        selectorExpression ??= CreateSelector(db);
        return selectorExpression;
    }

    /// <summary>
    /// Projects the query of entities into a query of DTOs, using the selector obtained from the context.
    /// </summary>
    /// <param name="db">The database context, used to resolve the selector.</param>
    /// <param name="query">The query of entities to be projected.</param>
    /// <returns>An IQueryable of the DTO, translated and executed by the provider.</returns>
    public static IQueryable<TDto> Select(DbContext db, IQueryable<TEntity> query)
    {
        return query.Select(GetSelector(db));
    }

    /// <summary>
    /// Filters the entity set of the context and projects the result into a query of DTOs.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="filter">The filter expression to apply to the entity set.</param>
    /// <returns>An IQueryable of the DTO, translated and executed by the provider.</returns>
    public static IQueryable<TDto> SelectWhere(DbContext db, Expression<Func<TEntity, bool>> filter)
    {
        return db.Set<TEntity>().Where(filter).Select(GetSelector(db));
    }

    private static Expression<Func<TEntity, TDto>> CreateSelector(DbContext db)
    {
        ISelectorFactory selectorFactory = db.GetApplicationService<ISelectorFactory>()
            ?? throw new InvalidOperationException(
                "ISelectorFactory not found in service provider of the DbContext");

        var selector = selectorFactory.Create<TEntity, TDto>()
            ?? throw new InvalidOperationException(
                $"Selector for {typeof(TEntity).Name} to {typeof(TDto).Name} not found.");

        return selector.GetSelectExpression();
    }
}
