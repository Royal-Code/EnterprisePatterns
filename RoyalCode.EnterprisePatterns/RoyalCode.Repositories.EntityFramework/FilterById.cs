using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RoyalCode.Repositories.EntityFramework;

/// <summary>
/// <para>
///     Utility class for applying filters to entities by ID in the context of the database.
/// </para>
/// </summary>
/// <typeparam name="TEntity">The type of entity to be filtered.</typeparam>
public static class FilterById<TEntity>
    where TEntity : class
{
    private static Func<object, Expression<Func<TEntity, bool>>>? entityFilterFactory;

    /// <summary>
    /// Applies a filter to search for an entity by its ID in the context of the database.
    /// </summary>
    /// <param name="db">The database context for accessing the entity model and the DbSet.</param>
    /// <param name="id">The identifier of the entity to be filtered.</param>
    /// <returns>An IQueryable query that represents the filter applied to the entity's DbSet.</returns>
    public static IQueryable<TEntity> Filter(DbContext db, object id)
    {
        entityFilterFactory ??= CreateEntityFilterFactory(db);
        var filter = entityFilterFactory(id);
        return db.Set<TEntity>().Where(filter);
    }

    /// <summary>
    /// Creates a filter expression to search for an entity by its ID.
    /// </summary>
    /// <param name="db">The database context for accessing the entity model.</param>
    /// <param name="id">The identifier of the entity to be filtered.</param>
    /// <returns>An expression that represents the filter for the entity that can be used in LINQ queries.</returns>
    public static Expression<Func<TEntity, bool>> GetFilterExpression(DbContext db, object id)
    {
        entityFilterFactory ??= CreateEntityFilterFactory(db);
        return entityFilterFactory(id);
    }

    private static Func<object, Expression<Func<TEntity, bool>>> CreateEntityFilterFactory(DbContext db)
    {
        var entityType = db.Model.FindEntityType(typeof(TEntity))
                ?? throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} not found in model.");
        var keyProps = entityType.FindPrimaryKey()?.Properties
            ?? throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not have a primary key.");

        return (idValue) =>
        {
            var entityParam = Expression.Parameter(typeof(TEntity), "e");
            Expression? body = null;
            if (keyProps.Count == 1)
            {
                var keyProp = keyProps[0];
                var entityProp = Expression.Property(entityParam, keyProp.Name);
                var idConst = Expression.Constant(idValue, keyProp.ClrType);
                body = Expression.Equal(entityProp, idConst);
            }
            else
            {
                var idObj = idValue;
                var idType = idObj.GetType();
                Expression? composed = null;
                for (int i = 0; i < keyProps.Count; i++)
                {
                    var keyProp = keyProps[i];
                    var entityProp = Expression.Property(entityParam, keyProp.Name);
                    object partValue;
                    if (idType.IsArray)
                    {
                        partValue = ((object[])idObj)[i];
                    }
                    else
                    {
                        var tupleProp = idType.GetProperty($"Item{i + 1}")!;
                        partValue = tupleProp.GetValue(idObj)!;
                    }
                    var idConst = Expression.Constant(partValue, keyProp.ClrType);
                    var eq = Expression.Equal(entityProp, idConst);
                    composed = composed == null ? eq : Expression.AndAlso(composed, eq);
                }
                body = composed!;
            }
            return Expression.Lambda<Func<TEntity, bool>>(body!, entityParam);
        };
    }
}
