using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.Persistence.EntityFramework.UnitOfWork;
using RoyalCode.Persistence.EntityFramework.WorkContext;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.Searches.Abstractions;
using RoyalCode.UnitOfWork.Abstractions;
using RoyalCode.WorkContext.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

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
        services.Add(ServiceDescriptor.Describe(
            typeof(IWorkContext),
            typeof(WorkContext<TDbContext>),
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(IUnitOfWork),
            sp => sp.GetService<IWorkContext>()!,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(IEntityManager),
            sp => sp.GetService<IWorkContext>()!,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            typeof(ISearchable),
            sp => sp.GetService<IWorkContext>()!,
            lifetime));

        return services.AddUnitOfWork<TDbContext>(lifetime);
    }

    public static IUnitOfWorkBuilder<TDbContext> ConfigureOperationHints<TDbContext>(
        this IUnitOfWorkBuilder<TDbContext> builder,
        Action<IHintHandlerRegistry>? configure = null)
        where TDbContext : DbContext
    {
        builder.Services.TryAdd(ServiceDescriptor.Describe(
            typeof(DefaultHintPerformer),
            typeof(DefaultHintPerformer),
            builder.Lifetime));

        builder.Services.TryAdd(ServiceDescriptor.Describe(
            typeof(IHintPerformer),
            sp => sp.GetService<DefaultHintPerformer>()!,
            builder.Lifetime));

        builder.Services.TryAdd(ServiceDescriptor.Describe(
            typeof(IHintsContainer),
            sp => sp.GetService<DefaultHintPerformer>()!,
            builder.Lifetime));

        if (configure is not null)
        {
            var registry = builder.Services.GetOrAddHintHandlerRegistry();
            configure(registry);
        }
        
        return builder;
    }

    private static IHintHandlerRegistry GetOrAddHintHandlerRegistry(this IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IHintHandlerRegistry)
            && d.ImplementationType is not null
            && d.ImplementationType.IsAssignableTo(typeof(DefaultHintHandlerRegistry)));

        if (descriptor is not null)
        {
            return (IHintHandlerRegistry)descriptor.ImplementationInstance!;
        }

        var registry = new DefaultHintHandlerRegistry();
        services.AddSingleton<IHintHandlerRegistry>(registry);
        return registry;
    }
}
