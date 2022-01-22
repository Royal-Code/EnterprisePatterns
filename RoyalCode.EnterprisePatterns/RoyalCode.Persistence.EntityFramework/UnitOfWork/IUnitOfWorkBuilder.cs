using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork;

/// <summary>
/// <para>
///     Interface to configure one unit of work with the DbContext.
/// </para>
/// <para>
///     It is designed to work with dependecy injection.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of DbContext for the unit of work.</typeparam>
public interface IUnitOfWorkBuilder<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// The service collection.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work as pooled.
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer);

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work as pooled.
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer);

    /// <summary>
    ///  Configure the <see cref="DbContext"/> for the unit of work..
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<DbContextOptionsBuilder> configurer);

    /// <summary>
    ///  Configure the <see cref="DbContext"/> for the unit of work..
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer);

    /// <summary>
    /// Add a repository for an entity as a service, related to <see cref="DbContext"/> used by the unit of work.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> AddRepository<TEntity>()
        where TEntity : class;
}