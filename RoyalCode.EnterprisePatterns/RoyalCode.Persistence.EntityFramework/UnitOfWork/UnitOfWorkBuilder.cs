using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.EntityFramework.Repositories;
using RoyalCode.Repositories.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork;

/// <summary>
/// <para>
///     Internal default implementation of <see cref="IUnitOfWorkBuilder{TDbContext}"/>.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
internal class UnitOfWorkBuilder<TDbContext> : IUnitOfWorkBuilder<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceCollection services;
    private readonly ServiceLifetime lifetime;
    
    /// <summary>
    /// Creates a new builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The lifetime that will be used when register services.</param>
    public UnitOfWorkBuilder(IServiceCollection services, ServiceLifetime lifetime)
    {
        this.services = services;
        this.lifetime = lifetime;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer)
    {
        services.AddDbContextPool<TDbContext>(configurer);
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        services.AddDbContextPool<TDbContext>(configurer);
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<DbContextOptionsBuilder> configurer)
    {
        services.AddDbContext<TDbContext>(configurer, lifetime);
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        services.AddDbContext<TDbContext>(configurer, lifetime);
        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> AddRepository<TEntity>() where TEntity : class
    {
        var repoType = typeof(IRepository<>).MakeGenericType(typeof(TEntity));
        
        services.Add(ServiceDescriptor.Describe(
            typeof(IRepository<>).MakeGenericType(typeof(TEntity)),
            typeof(Repository<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity)),
            lifetime));

        foreach (var dataService in repoType.GetInterfaces())
        {
            services.Add(ServiceDescriptor.Describe(dataService, sp => sp.GetService(repoType), lifetime));
        }
        
        return this;
    }
}