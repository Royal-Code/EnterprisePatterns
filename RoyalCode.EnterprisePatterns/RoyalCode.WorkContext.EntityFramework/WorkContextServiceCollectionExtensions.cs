using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.SmartSearch;
using RoyalCode.SmartSearch.Linq;
using RoyalCode.UnitOfWork.Abstractions;
using RoyalCode.UnitOfWork.EntityFramework;
using RoyalCode.UnitOfWork.EntityFramework.Internals;
using RoyalCode.WorkContext.Abstractions;
using RoyalCode.WorkContext.Abstractions.Querying;
using RoyalCode.WorkContext.EntityFramework;
using RoyalCode.WorkContext.EntityFramework.Commands.Configurations;
using RoyalCode.WorkContext.EntityFramework.Commands.Configurations.Internals;
using RoyalCode.WorkContext.EntityFramework.Querying.Configurations;
using RoyalCode.WorkContext.EntityFramework.Querying.Configurations.Internals;

namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable S3267

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class WorkContextServiceCollectionExtensions
{
    /// <summary>
    /// Adds a work context related to a <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext used in the work context.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IUnitOfWorkBuilder<TDbContext> AddWorkContext<TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        services.AddSmartSearchLinq();

        services.Add(ServiceDescriptor.Describe(
            typeof(IWorkContext<TDbContext>),
            typeof(WorkContext<TDbContext>),
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(IWorkContext),
            sp => sp.GetService<IWorkContext<TDbContext>>()!,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(IEntityManager),
            sp => sp.GetService<IWorkContext<TDbContext>>()!,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(ISearchManager),
            sp => sp.GetService<IWorkContext<TDbContext>>()!,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(IQueryDispatcher),
            sp => sp.GetService<IWorkContext<TDbContext>>()!,
            lifetime));

        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IUnitOfWork<TDbContext>),
            sp => sp.GetService<IWorkContext<TDbContext>>()!,
            lifetime));

        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IUnitOfWork),
            sp => sp.GetService<IWorkContext<TDbContext>>()!,
            lifetime));

        return new UnitOfWorkBuilder<TDbContext>(services, lifetime);
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
    public static IUnitOfWorkBuilder<TDbContext> AddWorkContext<TDbWorkContext, TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbWorkContext : class, IWorkContext<TDbContext>
        where TDbContext : DbContext
    {
        services.Add(ServiceDescriptor.Describe(
            typeof(TDbWorkContext),
            typeof(TDbWorkContext),
            lifetime));
        
        foreach(var implements in typeof(TDbWorkContext).GetInterfaces())
        {
            if (typeof(IUnitOfWork).IsAssignableFrom(implements))
                services.Add(ServiceDescriptor.Describe(
                    implements,
                    sp => sp.GetService<TDbWorkContext>()!,
                    lifetime));
        }

        return new UnitOfWorkBuilder<TDbContext>(services, lifetime);
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
    public static IUnitOfWorkBuilder<TDbContext> AddWorkContext<TWorkContext, TDbWorkContext, TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TWorkContext : IWorkContext
        where TDbWorkContext : class, IWorkContext<TDbContext>
        where TDbContext : DbContext
    {
        services.Add(ServiceDescriptor.Describe(
            typeof(TWorkContext),
            typeof(TDbWorkContext),
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(IWorkContext<TDbContext>),
            sp => sp.GetService<TWorkContext>()!,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(IWorkContext),
            sp => sp.GetService<TWorkContext>()!,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(IEntityManager),
            sp => sp.GetService<TWorkContext>()!,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(ISearchManager),
            sp => sp.GetService<TWorkContext>()!,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(IQueryDispatcher),
            sp => sp.GetService<TWorkContext>()!,
            lifetime));

        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IUnitOfWork<TDbContext>),
            sp => sp.GetService<TWorkContext>()!,
            lifetime));

        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IUnitOfWork),
            sp => sp.GetService<TWorkContext>()!,
            lifetime));

        return new UnitOfWorkBuilder<TDbContext>(services, lifetime);
    }

    /// <summary>
    /// Configure the operation hints for the unit of work.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext used in the work context.</typeparam>
    /// <param name="builder">The unit of work builder.</param>
    /// <param name="configure">The action to configure the operation hints.</param>
    /// <returns>The same instance of <paramref name="builder"/> for chaining.</returns>
    public static IUnitOfWorkBuilder<TDbContext> ConfigureOperationHints<TDbContext>(
        this IUnitOfWorkBuilder<TDbContext> builder,
        Action<IHintHandlerRegistry>? configure = null)
        where TDbContext : DbContext
    {
        builder.Services.AddOperationHints();

        if (configure is not null)
        {
            var registry = builder.Services.GetOrAddHintHandlerRegistry();
            configure(registry);
        }
        
        return builder;
    }

    /// <summary>
    /// Configures query-related services for the specified unit of work builder.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context used by the unit of work.</typeparam>
    /// <param name="builder">The unit of work builder to configure.</param>
    /// <param name="configureAction">An action that applies query configurations using the provided <see cref="IQueryConfigurer{TDbContext}"/>.</param>
    /// <returns>The same <see cref="IUnitOfWorkBuilder{TDbContext}"/> instance, allowing for method chaining.</returns>
    public static IUnitOfWorkBuilder<TDbContext> ConfigureQueries<TDbContext>(
        this IUnitOfWorkBuilder<TDbContext> builder,
        Action<IQueryConfigurer<TDbContext>> configureAction)
         where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configureAction);
        var configurations = new QueryConfigurer<TDbContext>(builder.Services);
        configureAction(configurations);
        return builder;
    }

    /// <summary>
    /// Configures command-related services for the specified unit of work builder.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context used by the unit of work.</typeparam>
    /// <param name="builder">The unit of work builder to configure.</param>
    /// <param name="configureAction">An action that applies command configurations using the provided <see cref="ICommandsConfigurer"/>.</param>
    /// <returns>The same <see cref="IUnitOfWorkBuilder{TDbContext}"/> instance, allowing for method chaining.</returns>
    public static IUnitOfWorkBuilder<TDbContext> ConfigureCommands<TDbContext>(
        this IUnitOfWorkBuilder<TDbContext> builder,
        Action<ICommandsConfigurer> configureAction)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configureAction);
        var configurations = new CommandsConfigurer<TDbContext>(builder.Services);
        configureAction(configurations);
        return builder;
    }

}
