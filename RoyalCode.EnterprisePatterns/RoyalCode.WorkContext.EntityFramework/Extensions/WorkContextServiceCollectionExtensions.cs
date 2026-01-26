using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoyalCode.Repositories;
using RoyalCode.Repositories.EntityFramework;
using RoyalCode.SmartSearch;
using RoyalCode.SmartSearch.EntityFramework.Services;
using RoyalCode.SmartSearch.Linq;
using RoyalCode.UnitOfWork;
using RoyalCode.UnitOfWork.EntityFramework;
using RoyalCode.WorkContext;
using RoyalCode.WorkContext.Commands;
using RoyalCode.WorkContext.EntityFramework;
using RoyalCode.WorkContext.EntityFramework.Configurations;
using RoyalCode.WorkContext.EntityFramework.Internal;

#pragma warning disable S3267

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class WorkContextServiceCollectionExtensions
{
    /// <summary>
    /// Adds a work context related to a default <see cref="DbContext"/>,
    /// and configure the <see cref="DbContext"/> with services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IWorkContextBuilder<DefaultDbContext> AddWorkContextDefault(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddWorkContextInternal<IWorkContext, WorkContext<DefaultDbContext>, DefaultDbContext>(lifetime)
            .ConfigureDbContextWithService();
    }

    /// <summary>
    /// Adds a work context related to a <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext used in the work context.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IWorkContextBuilder<TDbContext> AddWorkContext<TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        return services.AddWorkContextInternal<IWorkContext, WorkContext<TDbContext>, TDbContext>(lifetime);
    }

    /// <summary>
    /// Adds a work context related to a <see cref="DbContext"/> and register all implemented interfaces
    /// of the <typeparamref name="TDbWorkContext"/> that is assignable to <see cref="IUnitOfWork"/>.
    /// </summary>
    /// <typeparam name="TDbWorkContext">The type of the work context.</typeparam>
    /// <typeparam name="TDbContext">The type of the DbContext used in the work context.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IWorkContextBuilder<TDbContext> AddWorkContext<TDbWorkContext, TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbWorkContext : class, IWorkContext<TDbContext>
        where TDbContext : DbContext
    {
        return services.AddWorkContextInternal<IWorkContext, TDbWorkContext, TDbContext>(lifetime);
    }

    /// <summary>
    /// Adds a work context related to a <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TWorkContext">Specific <see cref="IWorkContext"/> interface.</typeparam>
    /// <typeparam name="TDbWorkContext">Implementation of <typeparamref name="TWorkContext"/>.</typeparam>
    /// <typeparam name="TDbContext">The type of the DbContext used in the work context.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IWorkContextBuilder<TDbContext> AddWorkContext<TWorkContext, TDbWorkContext, TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TWorkContext : IWorkContext
        where TDbWorkContext : class, IWorkContext<TDbContext>
        where TDbContext : DbContext
    {
        return services.AddWorkContextInternal<TWorkContext, TDbWorkContext, TDbContext>(lifetime);
    }

    private static IWorkContextBuilder<TDbContext> AddWorkContextInternal<TWorkContext, TDbWorkContext, TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TWorkContext : IWorkContext
        where TDbWorkContext : class, IWorkContext<TDbContext>
        where TDbContext : DbContext
    {
        services.AddSmartSearchLinq();

        services.Add(ServiceDescriptor.Describe(
            typeof(TDbWorkContext),
            typeof(TDbWorkContext),
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(IWorkContext<TDbContext>),
            sp => sp.GetService<TDbWorkContext>()!,
            lifetime));

        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IWorkContext),
            sp => sp.GetService<TDbWorkContext>()!,
            lifetime));

        if (typeof(TWorkContext) != typeof(IWorkContext))
        {
            services.Add(ServiceDescriptor.Describe(
                typeof(TWorkContext),
                sp => sp.GetService<TDbWorkContext>()!,
                lifetime));
        }

        services.Add(ServiceDescriptor.Describe(
            typeof(IEntityManager<TDbContext>),
            sp => sp.GetService<TDbWorkContext>()!,
            lifetime));

        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IEntityManager),
            sp => sp.GetService<TDbWorkContext>()!,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(ISearchManager<TDbContext>),
            sp => sp.GetService<TDbWorkContext>()!,
            lifetime));

        services.TryAdd(ServiceDescriptor.Describe(
            typeof(ISearchManager),
            sp => sp.GetService<TDbWorkContext>()!,
            lifetime));

        services.AddUnitOfWork<TDbWorkContext, TDbContext>(lifetime);

        return new WorkContextBuilder<TDbContext>(services, lifetime);
    }

    /// <summary>
    /// Adds <see cref="ICommandDispatcher"/> as a service.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IWorkContextBuilder<TDbContext> AddCommandDispatcher<TDbContext>(this IWorkContextBuilder<TDbContext> builder)
        where TDbContext : DbContext
    {
        builder.Services.TryAdd(ServiceDescriptor.Describe(
            typeof(ICommandDispatcher),
            typeof(CommandDispatcher),
            ServiceLifetime.Transient));
        
        return builder;
    }

    /// <summary>
    /// Adds <see cref="ICommandDispatcher"/> as a service.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommandDispatcher(this IServiceCollection services)
    {
        services.TryAdd(ServiceDescriptor.Describe(
            typeof(ICommandDispatcher), 
            typeof(CommandDispatcher),
            ServiceLifetime.Transient));

        return services;
    }
}
