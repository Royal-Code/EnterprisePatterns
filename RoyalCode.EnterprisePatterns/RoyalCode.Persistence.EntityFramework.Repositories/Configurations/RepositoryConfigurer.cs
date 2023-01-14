using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Repositories.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.Repositories.Configurations;

/// <summary>
/// Default implementation of <see cref="IRepositoryConfigurer{TDbContext}"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public sealed class RepositoryConfigurer<TDbContext> : IRepositoryConfigurer<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceCollection services;
    private readonly ServiceLifetime lifetime;

    /// <summary>
    /// Creates a new instance of <see cref="RepositoryConfigurer{TDbContext}"/>.
    /// </summary>
    /// <param name="services">The service collection to register the repositories as a service.</param>
    /// <param name="lifetime">The lifetime of the service.</param>
    public RepositoryConfigurer(
        IServiceCollection services,
        ServiceLifetime lifetime)
    {
        this.services = services;
        this.lifetime = lifetime;
    }

    /// <inheritdoc />
    public IRepositoryConfigurer<TDbContext> Add<TEntity>() where TEntity : class
    {
        var repoType = typeof(IRepository<>).MakeGenericType(typeof(TEntity));
        var dbRepoType = typeof(IRepository<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity));
        var repoImplType = typeof(InternalRepository<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity));

        services.Add(ServiceDescriptor.Describe(dbRepoType, repoImplType, lifetime));
        services.Add(ServiceDescriptor.Describe(repoType, sp => sp.GetService(dbRepoType)!, lifetime));

        foreach (var dataService in repoType.GetInterfaces())
            services.Add(ServiceDescriptor.Describe(dataService, sp => sp.GetService(dbRepoType)!, lifetime));

        return this;
    }
}