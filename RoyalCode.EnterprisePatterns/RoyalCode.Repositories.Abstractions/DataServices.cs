
using RoyalCode.Entities;
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.Repositories.Abstractions;

/// <summary>
/// <para>
///     Data service to add and persist new entities.
/// </para>
/// </summary>
/// <remarks>
/// <para>
///     When implemented together with the Unit Of Work pattern the entity will 
///     not be persisted directly when using the methods of this service.
/// </para>
/// </remarks>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IAdder<in TEntity>
{
    /// <summary>
    /// <para>
    ///     Adds a new entity to the repository to be persisted.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// <param name="entity">The new entity instance.</param>
    void Add(TEntity entity);

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
    /// <param name="entities">A collection of new entities.</param>
    void AddRange(IEnumerable<TEntity> entities);
}

/// <summary>
/// <para>
///     Data service for finding an existing entity.
/// </para>
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IFinder<TEntity>
{
    /// <summary>
    /// <para>
    ///     Finds an existing entity through its unique identity (Id).
    /// </para>
    /// </summary>
    /// <param name="id">The entity identity.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    TEntity? Find(object id);

    /// <summary>
    /// <para>
    ///     Finds an existing entity through its unique identity (Id).
    /// </para>
    /// </summary>
    /// <param name="id">The entity identity.</param>
    /// <param name="token">Token for cancelling tasks.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    ValueTask<TEntity?> FindAsync(object id, CancellationToken token = default);
}

/// <summary>
/// <para>
///     Data service to find an existing entity through its Guid.
/// </para>
/// </summary>
/// <remarks>
/// <para>
///     This service supports entities whose ID is not of type GUID 
///     and the entity has an additional property for the GUID, 
///     defined by the interface <see cref="IHasGuid"/>.
/// </para>
/// </remarks>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IFinderByGuid<TEntity>
    where TEntity : IHasGuid
{
    /// <summary>
    /// <para>
    ///     Finds an existing entity through its Guid.
    /// </para>
    /// </summary>
    /// <param name="guid">The entity Guid.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    TEntity? FindByGuid(Guid guid);

    /// <summary>
    /// <para>
    ///     Finds an existing entity through its Guid.
    /// </para>
    /// </summary>
    /// <param name="guid">The entity Guid.</param>
    /// <param name="token">Token for cancelling tasks.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    Task<TEntity?> FindByGuidAsync(Guid guid, CancellationToken token = default);
}

/// <summary>
/// <para>
///     Data service to find an existing entity through its Code.
/// </para>
/// </summary>
/// <remarks>
/// <para>
///     The code is not the entity ID, but a unique identifier, usually more human-friendly.
/// </para>
/// <para>
///     The code property is defined by the interface <see cref="IHasCode{TCode}"/>.
/// </para>
/// </remarks>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TCode">The code type.</typeparam>
public interface IFinderByCode<TEntity, in TCode>
    where TEntity : IHasCode<TCode>
{
    /// <summary>
    /// <para>
    ///     Finds an existing entity through its Code.
    /// </para>
    /// </summary>
    /// <param name="code">The entity code.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    TEntity? FindByCode(TCode code);

    /// <summary>
    /// <para>
    ///     Finds an existing entity through its Code.
    /// </para>
    /// </summary>
    /// <param name="code">The entity code.</param>
    /// <param name="token">Token for cancelling tasks.</param>
    /// <returns>
    /// <para>
    ///     Existing instance, or null/default if it does not exist.
    /// </para>
    /// </returns>
    Task<TEntity?> FindByCodeAsync(TCode code, CancellationToken token = default);
}

/// <summary>
/// <para>
///     Data service to merge data to an existing entity.
/// </para>
/// </summary>
/// <remarks>
/// <para>
///     When implemented together with the Unit Of Work pattern the entity will 
///     not be persisted directly when using the methods of this service.
/// </para>
/// </remarks>
/// <typeparam name="TEntity">Tipo da entidade.</typeparam>
[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", 
    Justification = "Update an entity, used to identify the entity")]
public interface IUpdater<TEntity>
{
    /// <summary>
    /// <para>
    ///     Operation to merge a data model to an existing entity.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     The data model should have an id, which will be used to get the entity from the database.
    /// </para>
    /// <para>
    ///     The fields of the data model should be the same as the entity's fields.
    /// </para>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// <param name="model">A data model with information to update an existing entity.</param>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <returns>
    /// <para>
    ///     True if the entity exists and has been updated, false otherwise.
    /// </para>
    /// </returns>
    bool Merge<TId>(IHasId<TId> model);

    /// <summary>
    /// <para>
    ///     Operation to merge a data model to an existing entity.
    /// </para>
    /// <remarks>
    /// <para>
    ///     The data model should have an id, which will be used to get the entity from the database.
    /// </para>
    /// <para>
    ///     The fields of the data model should be the same as the entity's fields.
    /// </para>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// </summary>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <param name="models">A collection of models.</param>
    /// <returns>For each model, true if the entity exists and has been updated, false otherwise.</returns>
    IEnumerable<bool> MergeRange<TId>(IEnumerable<IHasId<TId>> models)
    {
        return models.Select(Merge);
    }

    /// <summary>
    /// <para>
    ///     Operation to merge a data model to an existing entity.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     The data model should have an id, which will be used to get the entity from the database.
    /// </para>
    /// <para>
    ///     The fields of the data model should be the same as the entity's fields.
    /// </para>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// <param name="model">A data model with information to update an existing entity.</param>
    /// <param name="token">Token for cancelling tasks.</param>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <returns>
    /// <para>
    ///     True if the entity exists and has been updated, false otherwise.
    /// </para>
    /// </returns>
    Task<bool> MergeAsync<TId>(IHasId<TId> model, CancellationToken token = default);

    /// <summary>
    /// <para>
    ///     Operation to merge a data model to an existing entity.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     The data model should have an id, which will be used to get the entity from the database.
    /// </para>
    /// <para>
    ///     The fields of the data model should be the same as the entity's fields.
    /// </para>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// <typeparam name="TId">The Id type.</typeparam>
    /// <param name="models">A collection of models.</param>
    /// <param name="token">Token for cancelling tasks.</param>
    /// <returns>For each model, true if the entity exists and has been updated, false otherwise.</returns>
    async Task<IEnumerable<bool>> MergeRangeAsync<TId>(
        IEnumerable<IHasId<TId>> models,
        CancellationToken token = default)
    {
        return await Task.WhenAll(models.Select(model => MergeAsync(model, token)));
    }
}

/// <summary>
/// <para>
///     Data service to remove/delete entities.
/// </para>
/// </summary>
/// <remarks>
/// <para>
///     When implemented together with the Unit Of Work pattern the entity will 
///     not be persisted directly when using the methods of this service.
/// </para>
/// </remarks>
/// <typeparam name="TEntity">Tipo da entidade.</typeparam>
public interface IRemover<TEntity>
{
    /// <summary>
    /// <para>
    ///     Remove an entity from the data base.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     It is assumed that the entity was previously obtained from the database and it exists.
    /// </para>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// <param name="entity">The entity.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// <para>
    ///     Remove a range of entities from the data base.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     It is assumed that the entity was previously obtained from the database and it exists.
    /// </para>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// <param name="entities">The entities.</param>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="entities"/> is null.
    /// </exception>
    void RemoveRange(IEnumerable<TEntity> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
            Remove(entity);
    }

    /// <summary>
    /// <para>
    ///     Delete the entity by its Id.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// <param name="id">The entity Id.</param>
    /// <returns>
    /// <para>
    ///     The entity excluded, or null if the entity is not found.
    /// </para>
    /// </returns>
    TEntity? Delete(object id);

    /// <summary>
    /// <para>
    ///     Delete a range of entities by their Ids.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     When implemented together with the Unit Of Work pattern the entity 
    ///     will not be persisted directly when calling this method, 
    ///     it will be stored in memory until the completion of the unit of work.
    /// </para>
    /// </remarks>
    /// <param name="ids">The entities Ids.</param>
    /// <returns>
    /// <para>
    ///     The entities excluded, or null if the entity is not found.
    /// </para>
    /// </returns>
    IEnumerable<TEntity?> DeleteRange(IEnumerable<object> ids)
    {
        return ids.Select(Delete);
    }

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
    /// <param name="id">The entity Id.</param>
    /// <param name="token">Token for cancelling tasks.</param>
    /// <returns>
    /// <para>
    ///     The entity excluded, or null if the entity is not found.
    /// </para>
    /// </returns>
    Task<TEntity?> DeleteAsync(object id, CancellationToken token = default);

    /// <summary>
    /// <para>
    ///     Delete a range of entities by their Ids.
    /// </para>
    /// </summary>
    /// <param name="ids">The entities Ids.</param>
    /// <param name="token">Token for cancelling tasks.</param>
    /// <returns>
    /// <para>
    ///     The entities excluded, or null if the entity is not found.
    /// </para>
    /// </returns>
    async Task<IEnumerable<TEntity?>> DeleteRangeAsync(IEnumerable<object> ids, CancellationToken token = default)
    {
        return await Task.WhenAll(ids.Select(id => DeleteAsync(id, token)));
    }
}
