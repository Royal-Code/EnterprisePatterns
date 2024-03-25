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
    private readonly IServiceCollection services;
    private readonly ServiceLifetime lifetime;

    /// <summary>
    /// Creates a new builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The lifetime that will be used when register services.</param>
    public UnitOfWorkBuilder(
        IServiceCollection services, 
        ServiceLifetime lifetime)
    {
        this.services = services;
        this.lifetime = lifetime;
    }

    /// <inheritdoc />
    public IServiceCollection Services => services;

    /// <inheritdoc />
    public ServiceLifetime Lifetime => lifetime;

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        services.AddDbContextPool<TDbContext>(builder =>
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

        services.AddDbContextPool<TDbContext>((sp, builder) =>
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

        services.AddDbContext<TDbContext>(builder =>
        {
            builder.UseUnitOfWork();
            configurer(builder);
        }, lifetime);
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        services.AddDbContext<TDbContext>((sp, builder) =>
        {
            builder.UseUnitOfWork();
            configurer(sp, builder);
        }, lifetime);
        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureRepositories(Action<IRepositoriesBuilder<TDbContext>> configureAction)
    {
        if (configureAction is null)
            throw new ArgumentNullException(nameof(configureAction));

        var repositoryConfigurer = new RepositoriesBuilder<TDbContext>(services, lifetime);
        configureAction(repositoryConfigurer);
        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureSearches(Action<ISearchConfigurations<TDbContext>> configureAction)
    {
        if (configureAction is null)
            throw new ArgumentNullException(nameof(configureAction));

        var configurations = new SearchConfigurations<TDbContext>(services);
        configureAction(configurations);
        return this;
    }
}