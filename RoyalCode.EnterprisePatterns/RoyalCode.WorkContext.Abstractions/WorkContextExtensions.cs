namespace RoyalCode.WorkContext.Abstractions;

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
    /// <param name="token">Token for cancelling tasks.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    public static ValueTask<TEntity?> FindAsync<TEntity>(this IWorkContext context, object id, CancellationToken token = default)
        where TEntity : class
        => context.Repository<TEntity>().FindAsync(id, token);
    
    /// <summary>
    /// <para>
    ///     Remove an entity from the database.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="entity">The entity.</param>
    public static void Remove<TEntity>(this IWorkContext context,TEntity entity)
        where TEntity : class
        => context.Repository<TEntity>().Remove(entity);

    /// <summary>
    /// <para>
    ///     Remove a range of entities from the database.
    /// </para>
    /// </summary>
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
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="id">The entity Id.</param>
    /// <param name="token">Token for cancelling tasks.</param>
    /// <returns>
    /// <para>
    ///     The entity excluded, or null if the entity is not found.
    /// </para>
    /// </returns>
    public static Task<TEntity?> DeleteAsync<TEntity>(this IWorkContext context, object id, CancellationToken token = default)
        where TEntity : class
        => context.Repository<TEntity>().DeleteAsync(id, token);

    /// <summary>
    /// <para>
    ///     Delete a range of entities by their Ids.
    /// </para>
    /// </summary>
    /// <param name="context">The work context to get the repository.</param>
    /// <param name="ids">The entities Ids.</param>
    /// <param name="token">Token for cancelling tasks.</param>
    /// <returns>
    /// <para>
    ///     The entities excluded, or null if the entity is not found.
    /// </para>
    /// </returns>
    public static Task<IEnumerable<TEntity?>> DeleteRangeAsync<TEntity>(
        this IWorkContext context, IEnumerable<object> ids, CancellationToken token = default)
        where TEntity : class
        => context.Repository<TEntity>().DeleteRangeAsync(ids, token);
}