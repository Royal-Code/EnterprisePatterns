

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