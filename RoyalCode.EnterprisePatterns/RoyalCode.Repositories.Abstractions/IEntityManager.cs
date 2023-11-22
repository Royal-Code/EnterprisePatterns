
namespace RoyalCode.Repositories.Abstractions;

/// <summary>
/// <para>
///     Represents a manager of entities for persistence unit.
/// </para>
/// <para>
///     This interface is used to get the repositories of the entities of the persistence unit.
/// </para>
/// </summary>
public interface IEntityManager
{
    /// <summary>
    /// <para>
    ///     Get an repository for the entity type.
    /// </para>
    /// <para>
    ///     The repository must be part of the persistence unit, otherwise an exception will be thrown.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The repository for the entity type.</returns>
    /// <exception cref="RepositoryNotFoundException">
    ///     If the repository for the entity type is not found. 
    ///     It's happens when the entity type is not registered in the persistence unit.
    /// </exception>
    IRepository<TEntity> Repository<TEntity>()
        where TEntity : class;
}
