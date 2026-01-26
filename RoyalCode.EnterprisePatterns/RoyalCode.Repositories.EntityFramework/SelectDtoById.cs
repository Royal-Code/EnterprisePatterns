using Microsoft.EntityFrameworkCore;
using RoyalCode.SmartSearch.Linq.Services;
using System.Linq.Expressions;

namespace RoyalCode.Repositories.EntityFramework;

/// <summary>
/// Utility class to search for DTOs mapped from entities by ID in the context of the database.
/// </summary>
/// <typeparam name="TEntity">Type of entity to be filtered.</typeparam>
/// <typeparam name="TDto">Type of DTO to be designed.</typeparam>
public static class SelectDtoById<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    private static Expression<Func<TEntity, TDto>>? selectorExpression;

    /// <summary>
    /// Applies a filter to search for an entity by its ID in the context of the database,
    /// projecting the result to a DTO.
    /// </summary>
    /// <param name="db">The database context for accessing the entity model and the DbSet.</param>
    /// <param name="id">The identifier of the entity to be filtered.</param>
    /// <returns>An IQueryable that represents the filter applied to the entity's DbSet, designed for the DTO.</returns>
    public static IQueryable<TDto> FindByIdAndSelectDto(DbContext db, object id)
    {
        selectorExpression ??= CreateSelector(db);
        return FilterById<TEntity>.Filter(db, id).Select(selectorExpression);
    }

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
    public static Expression<Func<TEntity, TDto>> GetSelector(DbContext db)
    {
        selectorExpression ??= CreateSelector(db);
        return selectorExpression;
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
