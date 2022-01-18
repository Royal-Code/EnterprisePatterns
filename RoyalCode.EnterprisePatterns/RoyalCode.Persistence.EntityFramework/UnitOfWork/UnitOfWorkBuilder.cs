using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.EntityFramework.Repositories;
using RoyalCode.Repositories.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork;

internal class UnitOfWorkBuilder<TDbContext> : IUnitOfWorkBuilder<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceCollection services;
    private readonly ServiceLifetime lifetime;
    
    public UnitOfWorkBuilder(IServiceCollection services, ServiceLifetime lifetime)
    {
        this.services = services;
        this.lifetime = lifetime;
    }

    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer)
    {
        services.AddDbContextPool<TDbContext>(configurer);
        return this;
    }
    
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        services.AddDbContextPool<TDbContext>(configurer);
        return this;
    }
    
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<DbContextOptionsBuilder> configurer)
    {
        services.AddDbContext<TDbContext>(configurer, lifetime);
        return this;
    }
    
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        services.AddDbContext<TDbContext>(configurer, lifetime);
        return this;
    }

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