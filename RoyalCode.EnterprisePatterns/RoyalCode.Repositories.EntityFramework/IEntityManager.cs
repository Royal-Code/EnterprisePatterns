using Microsoft.EntityFrameworkCore;
using RoyalCode.Repositories.Abstractions;

namespace RoyalCode.Repositories.EntityFramework;

/// <summary>
/// Defines an interface for managing entities within a specific <see cref="DbContext"/> type.
/// </summary>
/// <typeparam name="TDbContext">
///     The type of the <see cref="DbContext"/> used by the entity manager. Must derive from <see cref="DbContext"/>.
/// </typeparam>
public interface IEntityManager<TDbContext> : IEntityManager
    where TDbContext : DbContext
{
    /// <summary>
    /// Gets the database context associated with the current operation.
    /// </summary>
    TDbContext Db { get; }
}
