using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.EntityFramework.Repositories.Configurations;
using RoyalCode.Searches.Persistence.EntityFramework.Configurations;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork;

/// <summary>
/// <para>
///     Interface to configure one unit of work with the DbContext.
/// </para>
/// <para>
///     It is designed to work with dependency injection.
/// </para>
/// </summary>
public interface IUnitOfWorkBuilder
{
    /// <summary>
    /// The service collection.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// The <see cref="ServiceLifetime"/> used for register the services and the <see cref="DbContext"/>.
    /// </summary>
    ServiceLifetime Lifetime { get; }
}

/// <summary>
/// <para>
///     Interface to configure one unit of work with the DbContext.
/// </para>
/// <para>
///     It is designed to work with dependency injection.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of DbContext for the unit of work.</typeparam>
public interface IUnitOfWorkBuilder<out TDbContext> : IUnitOfWorkBuilder
    where TDbContext : DbContext
{
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
    /// Configure the <see cref="DbContext"/> for the unit of work..
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<DbContextOptionsBuilder> configurer);

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work..
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer);

    /// <summary>
    /// Configure the repositories for the unit of work.
    /// </summary>
    /// <param name="configureAction">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureRepositories(Action<IRepositoryConfigurer<TDbContext>> configureAction);

    /// <summary>
    /// Configure the searches for the unit of work.
    /// </summary>
    /// <param name="configureAction">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureSearches(Action<ISearchConfigurations<TDbContext>> configureAction);
}