using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoyalCode.Persistence.EntityFramework.Events;
using RoyalCode.Persistence.EntityFramework.Events.Entity;
using RoyalCode.Persistence.EntityFramework.Events.Services;
using RoyalCode.UnitOfWork.EntityFramework;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class PersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Add services to handle domain events.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>Same instance of <paramref name="services"/>.</returns>
    public static IServiceCollection AddDomainEventHandler(this IServiceCollection services)
    {
        services.TryAddTransient<DomainEventHandlerFactory>();
        services.TryAddTransient<IDomainEventProcessorAggregate, DomainEventProcessorAggregate>();

        return services;
    }
    
    /// <summary>
    /// Add services to handle domain events.
    /// </summary>
    /// <param name="builder">The unit of work builder.</param>
    /// <returns>Same instance of <paramref name="builder"/>.</returns>
    public static IUnitOfWorkBuilder<TDbContext> AddDomainEventHandler<TDbContext>(
        this IUnitOfWorkBuilder<TDbContext> builder)
        where TDbContext : DbContext
    {
        builder.Services.AddDomainEventHandler();
        return builder;
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
        services.AddDomainEventHandler();

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
        builder.Services.AddStoreDomainEventAsDetailsService();
        return builder;
    }
}