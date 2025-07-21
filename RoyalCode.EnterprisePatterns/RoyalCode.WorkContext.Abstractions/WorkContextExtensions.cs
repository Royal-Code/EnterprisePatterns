using RoyalCode.Entities;
using RoyalCode.Repositories;
using RoyalCode.SmartProblems.Entities;
using System.Linq.Expressions;

namespace RoyalCode.WorkContext;

/// <summary>
/// Extensions methods for <see cref="IWorkContext"/>.
/// </summary>
public static class WorkContextExtensions
{
    /// <summary>
    /// <para>
    ///     Adds a new entity to the repository to be persisted.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="entity">The new entity instance.</param>
    public static void Add<TEntity>(this IWorkContext context, TEntity entity)
        where TEntity : class
        => context.Repository<TEntity>().Add(entity);

    /// <summary>
    /// <para>
    ///     Adds a new entity to the repository to be persisted.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="entity">The new entity instance.</param>
    /// <param name="ct">Cancellation token.</param>
    public static ValueTask AddAsync<TEntity>(this IWorkContext context, TEntity entity,
        CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().AddAsync(entity, ct);

    /// <summary>
    /// <para>
    ///     Adds a collection of new entities to the repository to be persisted.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="entities">A collection of new entities.</param>
    public static void AddRange<TEntity>(this IWorkContext context, IEnumerable<TEntity> entities)
        where TEntity : class
        => context.Repository<TEntity>().AddRange(entities);

    /// <summary>
    /// <para>
    ///     Adds a collection of new entities to the repository to be persisted.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="entities">A collection of new entities.</param>
    /// <param name="ct">Cancellation token.</param>
    public static Task AddRangeAsync<TEntity>(this IWorkContext context, IEnumerable<TEntity> entities, CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().AddRangeAsync(entities, ct);

    /// <summary>
    /// <para>
    ///     Finds an existing entity through its unique identity (id).
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="id">The entity identity.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    public static TEntity? Find<TEntity>(this IWorkContext context, object id)
        where TEntity : class
        => context.Repository<TEntity>().Find(id);

    /// <summary>
    /// <para>
    ///     Finds an existing entity through its unique identity (id).
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="id">The entity identity.</param>
    /// <param name="ct">Token for canceling tasks.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    public static ValueTask<TEntity?> FindAsync<TEntity>(this IWorkContext context, object id,
        CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().FindAsync(id, ct);

    /// <summary>
    /// <para>
    ///     Try to find an existing entity through its unique identity (Id).
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The type o entity id.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="id">The entity id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// <para>
    ///     An entry representing the entity record obtained from the database.
    /// </para>
    /// </returns>
    public static Task<FindResult<TEntity, TId>> FindAsync<TEntity, TId>(this IWorkContext context, Id<TEntity, TId> id,
        CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().FindAsync(id, ct);

    /// <summary>
    /// <para>
    ///     Try to find an existing entity through a filter expression.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="filter">The filter expression to apply.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// <para>
    ///     A result representing the entity record obtained from the database.
    /// </para>
    /// </returns>
    public static Task<FindResult<TEntity>> FindAsync<TEntity>(
        this IWorkContext context,
        Expression<Func<TEntity, bool>> filter,
        CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().FindAsync(filter, ct);

    /// <summary>
    /// <para>
    ///     Tries to find an existing entity through a property selector and a filter value.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="propertySelector">The property selector expression.</param>
    /// <param name="filterValue">The value to filter by.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// <para>
    ///     A result representing the entity record obtained from the database.
    /// </para>
    /// </returns>
    public static Task<FindResult<TEntity>> FindAsync<TEntity, TValue>(
        this IWorkContext context,
        Expression<Func<TEntity, TValue>> propertySelector,
        TValue filterValue,
        CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().FindAsync(propertySelector, filterValue, ct);

    /// <summary>
    /// <para>
    ///     Finds an existing entity through its Guid.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="guid">The entity Guid.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    public static TEntity? FindByGuid<TEntity>(this IWorkContext context, Guid guid)
        where TEntity : class, IHasGuid
        => context.GetService<IFinderByGuid<TEntity>>().FindByGuid(guid);

    /// <summary>
    /// <para>
    ///     Finds an existing entity through its Guid.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="guid">The entity Guid.</param>
    /// <param name="ct">Token for canceling tasks.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    public static Task<TEntity?> FindByGuidAsync<TEntity>(this IWorkContext context, Guid guid,
        CancellationToken ct = default)
        where TEntity : class, IHasGuid
        => context.GetService<IFinderByGuid<TEntity>>().FindByGuidAsync(guid, ct);

    /// <summary>
    /// <para>
    ///     Finds an existing entity through its Code.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="code">The entity code.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    public static TEntity? FindByCode<TEntity, TCode>(this IWorkContext context, TCode code)
        where TEntity : class, IHasCode<TCode>
        => context.GetService<IFinderByCode<TEntity, TCode>>().FindByCode(code);

    /// <summary>
    /// <para>
    ///     Finds an existing entity through its Code.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="code">The entity code.</param>
    /// <param name="ct">Token for canceling tasks.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    public static Task<TEntity?> FindByCodeAsync<TEntity, TCode>(this IWorkContext context, TCode code, CancellationToken ct = default)
        where TEntity : class, IHasCode<TCode>
        => context.GetService<IFinderByCode<TEntity, TCode>>().FindByCodeAsync(code, ct);

    /// <summary>
    /// <para>
    ///     Operation to merge a data model to an existing entity.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="model">A data model with information to update an existing entity.</param>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <returns>
    /// <para>
    ///     True if the entity exists and has been updated, false otherwise.
    /// </para>
    /// </returns>
    public static bool Merge<TEntity, TId>(this IWorkContext context, IHasId<TId> model)
        where TEntity : class
        => context.Repository<TEntity>().Merge(model);

    /// <summary>
    /// <para>
    ///     Operation to merge a data model to an existing entity.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="models">A collection of models.</param>
    /// <returns>For each model, true if the entity exists and has been updated, false otherwise.</returns>
    public static IEnumerable<bool> MergeRange<TEntity, TId>(this IWorkContext context, IEnumerable<IHasId<TId>> models)
        where TEntity : class
        => context.Repository<TEntity>().MergeRange(models);

    /// <summary>
    /// <para>
    ///     Operation to merge a data model to an existing entity.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="model">A data model with information to update an existing entity.</param>
    /// <param name="ct">Token for canceling tasks.</param>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <returns>
    /// <para>
    ///     True if the entity exists and has been updated, false otherwise.
    /// </para>
    /// </returns>
    public static Task<bool> MergeAsync<TEntity, TId>(this IWorkContext context, IHasId<TId> model,
        CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().MergeAsync(model, ct);

    /// <summary>
    /// <para>
    ///     Operation to merge a data model to an existing entity.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <typeparam name="TModel">The model with the data.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="id">The id value.</param>
    /// <param name="model">The model.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// <para>
    ///     True if the entity exists and has been updated, false otherwise.
    /// </para>
    /// </returns>
    public static Task<bool> MergeAsync<TEntity, TId, TModel>(this IWorkContext context,
        Id<TEntity, TId> id, TModel model, CancellationToken ct = default)
        where TEntity : class
        where TModel : class
        => context.Repository<TEntity>().MergeAsync(id, model, ct);

    /// <summary>
    /// <para>
    ///     Operation to merge a data model to an existing entity.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="models">A collection of models.</param>
    /// <param name="ct">Token for canceling tasks.</param>
    /// <returns>For each model, true if the entity exists and has been updated, false otherwise.</returns>
    public static Task<IEnumerable<bool>> MergeRangeAsync<TEntity, TId>(
        this IWorkContext context, IEnumerable<IHasId<TId>> models, CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().MergeRangeAsync(models, ct);

    /// <summary>
    /// <para>
    ///     Remove an entity from the database.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="entity">The entity.</param>
    public static void Remove<TEntity>(this IWorkContext context, TEntity entity)
        where TEntity : class
        => context.Repository<TEntity>().Remove(entity);

    /// <summary>
    /// <para>
    ///     Remove a range of entities from the database.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="entities">The entities.</param>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="entities"/> is null.
    /// </exception>
    public static void RemoveRange<TEntity>(this IWorkContext context, IEnumerable<TEntity> entities)
        where TEntity : class
        => context.Repository<TEntity>().RemoveRange(entities);

    /// <summary>
    /// <para>
    ///     Delete the entity by its Id.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="id">The entity id.</param>
    /// <returns>
    /// <para>
    ///     The entity excluded, or null if the entity is not found.
    /// </para>
    /// </returns>
    public static TEntity? Delete<TEntity>(this IWorkContext context, object id)
        where TEntity : class
        => context.Repository<TEntity>().Delete(id);

    /// <summary>
    /// <para>
    ///     Delete a range of entities by their Ids.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="ids">The entities Ids.</param>
    /// <returns>
    /// <para>
    ///     The entities excluded, or null if the entity is not found.
    /// </para>
    /// </returns>
    public static IEnumerable<TEntity?> DeleteRange<TEntity>(this IWorkContext context, IEnumerable<object> ids)
        where TEntity : class
        => context.Repository<TEntity>().DeleteRange(ids);

    /// <summary>
    /// <para>
    ///     Delete the entity by their Id.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="id">The entity Id.</param>
    /// <param name="ct">Token for canceling tasks.</param>
    /// <returns>
    /// <para>
    ///     The entity excluded, or null if the entity is not found.
    /// </para>
    /// </returns>
    public static Task<TEntity?> DeleteAsync<TEntity>(this IWorkContext context, object id,
        CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().DeleteAsync(id, ct);

    /// <summary>
    /// <para>
    ///     Delete the entity by their Id.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="id">The entity Id.</param>
    /// <param name="ct">Token for canceling tasks.</param>
    /// <returns>
    /// <para>
    ///     A find result representing the excluded entity from the database.
    /// </para>    
    /// </returns>
    public static Task<FindResult<TEntity, TId>> DeleteAsync<TEntity, TId>(
        this IWorkContext context, 
        Id<TEntity, TId> id, 
        CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().DeleteAsync(id, ct);


    /// <summary>
    /// <para>
    ///     Delete a range of entities by their Ids.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="ids">The entities Ids.</param>
    /// <param name="ct">Token for canceling tasks.</param>
    /// <returns>
    /// <para>
    ///     The entities excluded, or null if the entity is not found.
    /// </para>
    /// </returns>
    public static Task<IEnumerable<TEntity?>> DeleteRangeAsync<TEntity>(
        this IWorkContext context, IEnumerable<object> ids, CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().DeleteRangeAsync(ids, ct);

    /// <summary>
    /// <para>
    ///     Delete a range of entities by their Ids.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="ids">The entities Ids.</param>
    /// <param name="ct">Token for canceling tasks.</param>
    /// <returns>
    /// <para>
    ///     A collection of find results representing the excluded entities from the database.
    /// </para>
    /// </returns>
    public static Task<IEnumerable<FindResult<TEntity, TId>>> DeleteRangeAsync<TEntity, TId>(
        this IWorkContext context,
        IEnumerable<Id<TEntity, TId>> ids,
        CancellationToken ct = default)
        where TEntity : class
        => context.Repository<TEntity>().DeleteRangeAsync(ids, ct);
}