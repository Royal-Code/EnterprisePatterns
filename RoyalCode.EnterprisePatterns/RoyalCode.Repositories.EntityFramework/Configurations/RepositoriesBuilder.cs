using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Entities;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.Repositories.Abstractions;

namespace RoyalCode.Repositories.EntityFramework.Configurations;

/// <summary>
/// Default implementation of <see cref="IRepositoriesBuilder{TDbContext}"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public sealed class RepositoriesBuilder<TDbContext> : IRepositoriesBuilder<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceCollection services;
    private readonly ServiceLifetime lifetime;

    /// <summary>
    /// Creates a new instance of <see cref="RepositoriesBuilder{TDbContext}"/>.
    /// </summary>
    /// <param name="services">The service collection to register the repositories as a service.</param>
    /// <param name="lifetime">The lifetime of the service.</param>
    public RepositoriesBuilder(
        IServiceCollection services,
        ServiceLifetime lifetime)
    {
        this.services = services;
        this.lifetime = lifetime;
    }

    /// <inheritdoc />
    public IRepositoriesBuilder<TDbContext> Add<TEntity>() where TEntity : class
    {
        // register the repository
        var repoType = typeof(IRepository<>).MakeGenericType(typeof(TEntity));
        var dbRepoType = typeof(IRepository<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity));
        var repoImplType = typeof(InternalRepository<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity));

        services.Add(ServiceDescriptor.Describe(dbRepoType, repoImplType, lifetime));
        services.Add(ServiceDescriptor.Describe(repoType, sp => sp.GetService(dbRepoType)!, lifetime));

        foreach (var dataService in repoType.GetInterfaces())
            services.Add(ServiceDescriptor.Describe(dataService, sp => sp.GetService(dbRepoType)!, lifetime));

        // if the entity implements IHasGuid interface, register the FinderByGuid
        if (typeof(IHasGuid).IsAssignableFrom(typeof(TEntity)))
        {
            var finderType = typeof(IFinderByGuid<>).MakeGenericType(typeof(TEntity));
            var finderImplType = typeof(FinderByGuid<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity));

            services.Add(ServiceDescriptor.Describe(finderType, finderImplType, lifetime));
        }
        
        // if the entity implements IHasCode interface, register the FinderByCode
        if (IsGenericAssinableFrom(typeof(IFinderByCode<,>), typeof(TEntity), out var finderByCodeType))
        {
            var finderImplType = typeof(FinderByCode<,,>)
                .MakeGenericType(typeof(TDbContext), typeof(TEntity), finderByCodeType.GetGenericArguments()[1]);

            services.Add(ServiceDescriptor.Describe(finderByCodeType, finderImplType, lifetime));
        }
        
        return this;
    }

    /// <inheritdoc />
    public IRepositoriesBuilder<TDbContext> ConfigureOperationHints(Action<IHintHandlerRegistry>? configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        services.AddOperationHints();

        if (configure is not null)
        {
            var registry = services.GetOrAddHintHandlerRegistry();
            configure(registry);
        }

        return this;
    }

    private bool IsGenericAssinableFrom(Type genericType, Type type, [NotNullWhen(true)] out Type? closedType)
    {
        if (genericType.IsGenericTypeDefinition)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    closedType = interfaceType;
                    return true;
                }
            }
        }

        closedType = null;
        return false;
    }
}