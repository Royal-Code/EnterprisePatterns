
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoyalCode.Persistence.EntityFramework.Events;
using RoyalCode.Persistence.EntityFramework.Events.Entity;
using RoyalCode.Persistence.EntityFramework.Events.Services;
using RoyalCode.Persistence.EntityFramework.UnitOfWork;
using RoyalCode.UnitOfWork.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class PersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Adds a unit of work related to a <see cref="DbContext"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <typeparam name="TDbContext">The type of the DbContext used in the unit of work.</typeparam>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories.
    /// </returns>
    public static IUnitOfWorkBuilder<TDbContext> AddUnitOfWork<TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        services.TryAddTransient<DomainEventHandlerFactory>();
        services.TryAddTransient<IDomainEventProcessorAggregate, DomainEventProcessorAggregate>();
        
        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IUnitOfWorkContext), 
            typeof(UnitOfWorkContext<>).MakeGenericType(typeof(TDbContext)),
            lifetime));

        return new UnitOfWorkBuilder<TDbContext>(services, lifetime);
    }

    /// <summary>
    /// <para>
    ///     Adds the service for store the domain events handled by the unit of work as 
    ///     <see cref="DomainEventDetails"/>.
    /// </para>
    /// <para>
    ///     It is required that <see cref="DomainEventDetails"/> be an entity configured in the <see cref="DbContext"/>
    ///     used by the unit of work.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>Same instance of <paramref name="services"/>.</returns>
    public static IServiceCollection AddStoreDomainEventAsDetailsService(this IServiceCollection services)
    {
        services.TryAddTransient<IDomainEventProcessor, StoreDomainEventAsDetails>();
        return services;
    }

    /// <summary>
    /// <para>
    ///     Adds the service for store the domain events handled by the unit of work as 
    ///     <see cref="DomainEventDetails"/>.
    /// </para>
    /// <para>
    ///     It is required that <see cref="DomainEventDetails"/> be an entity configured in the <see cref="DbContext"/>
    ///     used by the unit of work.
    /// </para>
    /// </summary>
    /// <typeparam name="TDbContext">The <see cref="DbContext"/> type.</typeparam>
    /// <param name="builder">The unit of work builder.</param>
    /// <returns>The same instance of <paramref name="builder"/>.</returns>
    public static IUnitOfWorkBuilder<TDbContext> AddStoreDomainEventAsDetailsService<TDbContext>(
        this IUnitOfWorkBuilder<TDbContext> builder)
        where TDbContext : DbContext
    {
        builder.Services.TryAddTransient<IDomainEventProcessor, StoreDomainEventAsDetails>();
        return builder;
    }
}