
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IUnitOfWorkContext), 
            typeof(UnitOfWorkContext<>).MakeGenericType(typeof(TDbContext)),
            lifetime));

        return new UnitOfWorkBuilder<TDbContext>(services, lifetime);
    }
}