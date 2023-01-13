using Microsoft.EntityFrameworkCore;

namespace RoyalCode.Persistence.EntityFramework.Repositories.Configurations;

/// <summary>
///    Configure repositories for the unit of work.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public interface IRepositoryConfigurer<out TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Add a repository for an entity as a service, related to <see cref="DbContext"/> used by the unit of work.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The same instance.</returns>
    IRepositoryConfigurer<TDbContext> Add<TEntity>()
        where TEntity : class;
}
