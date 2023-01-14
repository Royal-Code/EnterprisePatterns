namespace RoyalCode.Repositories.Abstractions;

/// <summary>
/// <para>
///     Implementation of the repository pattern.
/// </para>
/// <para>
///     This is a complete repository, with all the standard operations of a repository,
///     being able to create, add, find, modify and remove an entity.
/// </para>
/// <para>
///     Typically repository implementations are done in conjunction with the Unit Of Work pattern,
///     where the repository keeps entities in memory during the unit of work and 
///     the changes are applied against the database at the completion of the unit of work.
/// </para>
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IRepository<T> : IAdder<T>, IFinder<T>, IUpdater<T>, IRemover<T>
    where T : class
{ }