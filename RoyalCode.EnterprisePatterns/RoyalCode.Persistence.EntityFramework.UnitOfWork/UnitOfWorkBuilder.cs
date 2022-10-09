using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork;

/// <summary>
/// <para>
///     Internal default implementation of <see cref="IUnitOfWorkBuilder{TDbContext}"/>.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
internal class UnitOfWorkBuilder<TDbContext> : IUnitOfWorkBuilder<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceCollection services;
    private readonly ServiceLifetime lifetime;

    /// <summary>
    /// Creates a new builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The lifetime that will be used when register services.</param>
    public UnitOfWorkBuilder(IServiceCollection services, ServiceLifetime lifetime)
    {
        this.services = services;
        this.lifetime = lifetime;
    }

    /// <inheritdoc />
    public IServiceCollection Services => services;

    /// <inheritdoc />
    public ServiceLifetime Lifetime => lifetime;

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer)
    {
        services.AddDbContextPool<TDbContext>(builder =>
        {
            builder.UseUnitOfWork();
            configurer(builder);
        });
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        services.AddDbContextPool<TDbContext>((sp, builder) =>
        {
            builder.UseUnitOfWork();
            configurer(sp, builder);
        });
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<DbContextOptionsBuilder> configurer)
    {
        services.AddDbContext<TDbContext>(builder =>
        {
            builder.UseUnitOfWork();
            configurer(builder);
        }, lifetime);
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        services.AddDbContext<TDbContext>((sp, builder) =>
        {
            builder.UseUnitOfWork();
            configurer(sp, builder);
        }, lifetime);
        return this;
    }
}