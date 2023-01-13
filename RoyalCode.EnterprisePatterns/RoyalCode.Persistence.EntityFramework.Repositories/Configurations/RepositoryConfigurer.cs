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
    private readonly Action<Type>? callback;

    /// <summary>
    /// Creates a new instance of <see cref="RepositoryConfigurer{TDbContext}"/>.
    /// </summary>
    /// <param name="services">The service collection to register the repositories as a service.</param>
    /// <param name="lifetime">The lifetime of the service.</param>
    /// <param name="callback">The callback to be called after the repository is registered.</param>
    public RepositoryConfigurer(
        IServiceCollection services,
        ServiceLifetime lifetime,
        Action<Type>? callback = null)
    {
        this.services = services;
        this.lifetime = lifetime;
        this.callback = callback;
    }

    /// <inheritdoc />
    public IRepositoryConfigurer<TDbContext> Add<TEntity>() where TEntity : class
    {
        // TODO: poderia haver um IRepository<TDbContext, TEntity> para que, na hora do WorkContext resolver,
        //       ele possa usar o DbContext correto.
        //       O mesmo é válido para ISearch.

        var repoType = typeof(IRepository<>).MakeGenericType(typeof(TEntity));

        services.Add(ServiceDescriptor.Describe(
            typeof(IRepository<>).MakeGenericType(typeof(TEntity)),
            typeof(Repository<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity)),
            lifetime));

        foreach (var dataService in repoType.GetInterfaces())
        {
            services.Add(ServiceDescriptor.Describe(dataService, sp => sp.GetService(repoType)!, lifetime));
        }

        callback?.Invoke(typeof(TEntity));

        return this;
    }
}