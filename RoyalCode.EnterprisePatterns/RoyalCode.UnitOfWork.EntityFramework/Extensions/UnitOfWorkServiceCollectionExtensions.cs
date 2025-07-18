
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoyalCode.UnitOfWork.EntityFramework;
using RoyalCode.UnitOfWork.Abstractions;
using RoyalCode.UnitOfWork.EntityFramework.Internals;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class UnitOfWorkServiceCollectionExtensions
{
    /// <summary>
    /// Adds a unit of work related to a <see cref="DbContext"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <typeparam name="TDbContext">The type of the DbContext used in the unit of work.</typeparam>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IUnitOfWorkBuilder<TDbContext> AddUnitOfWork<TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IUnitOfWork<TDbContext>), 
            typeof(UnitOfWork<>).MakeGenericType(typeof(TDbContext)),
            lifetime));

        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IUnitOfWork),
            sp => sp.GetService<IUnitOfWork<TDbContext>>()!,
            lifetime));

        return new UnitOfWorkBuilder<TDbContext>(services, lifetime);
    }

    /// <summary>
    /// Adds a unit of work related to a <see cref="DbContext"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <typeparam name="TUnitOfWork">Specific <see cref="IUnitOfWork{TDbContext}"/> implementation.</typeparam>
    /// <typeparam name="TDbContext">The type of the DbContext used in the unit of work.</typeparam>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IUnitOfWorkBuilder<TDbContext> AddUnitOfWork<TUnitOfWork, TDbContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TUnitOfWork : class, IUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        // if TUnitOfWork is registered, it will be used instead of the default UnitOfWork<TDbContext>

        if (services.Any(sd => sd.ServiceType == typeof(TUnitOfWork)))
        {
            services.TryAdd(ServiceDescriptor.Describe(
                typeof(IUnitOfWork<TDbContext>),
                sp => sp.GetService<TUnitOfWork>()!,
                lifetime));

            services.TryAdd(ServiceDescriptor.Describe(
                typeof(IUnitOfWork),
                sp => sp.GetService<TUnitOfWork>()!,
                lifetime));
        }
        else
        {
            services.TryAdd(ServiceDescriptor.Describe(
                typeof(IUnitOfWork<TDbContext>),
                typeof(TUnitOfWork),
                lifetime));

            services.TryAdd(ServiceDescriptor.Describe(
                typeof(IUnitOfWork),
                sp => sp.GetService<IUnitOfWork<TDbContext>>()!,
                lifetime));
        }

        return new UnitOfWorkBuilder<TDbContext>(services, lifetime);
    }
}