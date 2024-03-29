using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Repositories.EntityFramework.Configurations;
using RoyalCode.Searches.Persistence.EntityFramework.Configurations;

namespace RoyalCode.UnitOfWork.EntityFramework;

/// <summary>
/// <para>
///     Internal default implementation of <see cref="IUnitOfWorkBuilder{TDbContext}"/>.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
internal sealed class UnitOfWorkBuilder<TDbContext> : IUnitOfWorkBuilder<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Creates a new builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The lifetime that will be used when register services.</param>
    public UnitOfWorkBuilder(
        IServiceCollection services, 
        ServiceLifetime lifetime)
    {
        Services = services;
        Lifetime = lifetime;
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public ServiceLifetime Lifetime { get; }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        Services.AddDbContextPool<TDbContext>(builder =>
        {
            builder.UseUnitOfWork();
            configurer(builder);
        });
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        Services.AddDbContextPool<TDbContext>((sp, builder) =>
        {
            builder.UseUnitOfWork();
            configurer(sp, builder);
        });
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<DbContextOptionsBuilder> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        Services.AddDbContext<TDbContext>(builder =>
        {
            builder.UseUnitOfWork();
            configurer(builder);
        }, Lifetime);
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        Services.AddDbContext<TDbContext>((sp, builder) =>
        {
            builder.UseUnitOfWork();
            configurer(sp, builder);
        }, Lifetime);
        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureRepositories(Action<IRepositoriesBuilder<TDbContext>> configureAction)
    {
        if (configureAction is null)
            throw new ArgumentNullException(nameof(configureAction));

        var repositoryConfigurer = new RepositoriesBuilder<TDbContext>(Services, Lifetime);
        configureAction(repositoryConfigurer);
        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureSearches(Action<ISearchConfigurations<TDbContext>> configureAction)
    {
        if (configureAction is null)
            throw new ArgumentNullException(nameof(configureAction));

        var configurations = new SearchConfigurations<TDbContext>(Services);
        configureAction(configurations);
        return this;
    }
}