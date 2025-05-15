using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Repositories.EntityFramework.Configurations;
using RoyalCode.SmartSearch.EntityFramework.Configurations;
using RoyalCode.UnitOfWork.EntityFramework.Services;

namespace RoyalCode.UnitOfWork.EntityFramework;

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
    /// <para>
    ///     Configure the <see cref="DbContext"/> for the unit of work.
    /// </para>
    /// <para>
    ///     The configuration is done by the <see cref="IConfigureDbContextService{TDbContext}"/>
    ///     registered in the services.
    /// </para>
    /// <para>
    ///     When the <see cref="IConfigureDbContextService{TDbContext}"/> is not registered, an
    ///     <see cref="InvalidOperationException"/> is thrown.
    /// </para>
    /// </summary>
    /// <returns>The same instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     The <see cref="IConfigureDbContextService{TDbContext}"/> is not registered.
    /// </exception>
    IUnitOfWorkBuilder<TDbContext> ConfigureWithService()
    {
        Services.AddDbContext<TDbContext>((sp, builder) =>
        {
            var configurator = sp.GetService<IConfigureDbContextService<TDbContext>>();

            if (configurator is null)
                throw new InvalidOperationException(
                    "The IConfigureDbContextService is not registered. " +
                    "When using the ConfigureWithService method, it is necessary to register the " +
                    "IConfigureDbContextService<TDbContext>.");

            configurator.ConfigureDbContext(builder);
        }, Lifetime);
        return this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work as pooled.
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContextPool<TDbContext>(configurer);
        return this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work as pooled.
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContextPool<TDbContext>(configurer);
        return this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work..
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContext<TDbContext>(configurer, Lifetime);
        return this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work..
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContext<TDbContext>(configurer, Lifetime);
        return this;
    }

    /// <summary>
    /// Configure the repositories for the unit of work.
    /// </summary>
    /// <param name="configureAction">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureRepositories(Action<IRepositoriesBuilder<TDbContext>> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);
        var repositoryConfigurer = new RepositoriesBuilder<TDbContext>(Services, Lifetime);
        configureAction(repositoryConfigurer);
        return this;
    }

    /// <summary>
    /// Configure the searches for the unit of work.
    /// </summary>
    /// <param name="configureAction">Action to configure.</param>
    /// <returns>The same instance.</returns>
    IUnitOfWorkBuilder<TDbContext> ConfigureSearches(Action<ISearchConfigurations<TDbContext>> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);
        var configurations = new SearchConfigurations<TDbContext>(Services);
        configureAction(configurations);
        return this;
    }
}