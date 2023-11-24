using Microsoft.EntityFrameworkCore;
using RoyalCode.OperationHint.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.Repositories.Configurations;

/// <summary>
///    Configure repositories for the unit of work.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public interface IRepositoriesBuilder<out TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Add a repository for an entity as a service, related to <see cref="DbContext"/> used by the unit of work.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The same instance.</returns>
    IRepositoriesBuilder<TDbContext> Add<TEntity>()
        where TEntity : class;

    /// <summary>
    /// Allows the configuration of hints for repository operations.
    /// </summary>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The same instance.</returns>
    IRepositoriesBuilder<TDbContext> ConfigureOperationHints(Action<IHintHandlerRegistry> configure);
}
