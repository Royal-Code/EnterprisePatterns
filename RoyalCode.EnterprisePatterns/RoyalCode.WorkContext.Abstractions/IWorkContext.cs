using RoyalCode.Repositories.Abstractions;
using RoyalCode.Searches.Abstractions;
using RoyalCode.UnitOfWork.Abstractions;

namespace RoyalCode.WorkContext.Abstractions;

/// <summary>
/// <para>
///     A <see cref="IWorkContext"/> is an extension of the <see cref="IUnitOfWorkContext"/> 
///     that enables the access to data access components related to a context,
///     or one could say, related to a persistence unit.
/// </para>
/// </summary>
public interface IWorkContext : IUnitOfWorkContext
{
    /// <summary>
    /// <para>
    ///     Gets the repository for the entity.
    /// </para>
    /// <para>
    ///     The repository must be part of the persistence unit, otherwise an exception will be thrown.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The entity repository.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If the entity is not part of the persistence unit, 
    ///     where there will not be a repository configured for the entity in this context.
    /// </exception>
    IRepository<T> Repository<T>() 
        where T : class;

    /// <summary>
    /// <para>
    ///     Gets an instance of the search component for a context entity.
    /// </para>
    /// <para>
    ///     There must be a search component for the persistence unit, otherwise an exception will be thrown.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>A new search.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If entity is not part of the persistence unit or there is no search component for it.
    /// </exception>
    ISearch<T> Search<T>() 
        where T : class;
}