using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.UnitOfWork.Configurations;
using RoyalCode.UnitOfWork.EntityFramework.Services;

namespace RoyalCode.UnitOfWork.EntityFramework.Configurations;

/// <summary>
/// <para>
///     Interface to configure one unit of work with the DbContext.
/// </para>
/// <para>
///     It is designed to work with dependency injection.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of DbContext for the unit of work.</typeparam>
public interface IUnitOfWorkBuilder<out TDbContext> 
    : IUnitOfWorkBuilder<TDbContext, IUnitOfWorkBuilder<TDbContext>>
    where TDbContext : DbContext
{ }

/// <summary>
/// <para>
///     Interface to configure one unit of work with the DbContext.
/// </para>
/// <para>
///     It is designed to work with dependency injection.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of DbContext for the unit of work.</typeparam>
/// <typeparam name="TBuilder"> The type of the builder for the unit of work.</typeparam>
public interface IUnitOfWorkBuilder<out TDbContext, out TBuilder> : IUnitOfWorkBuilderBase<TBuilder>
    where TDbContext : DbContext
    where TBuilder : IUnitOfWorkBuilder<TDbContext, TBuilder>
{
    /// <summary>
    /// <para>
    ///     Configure the <see cref="DbContext"/> for the unit of work.
    /// </para>
    /// <para>
    ///     The configuration is done by the <see cref="IConfigureDbContextService{TDbContext}"/>
    ///     registered in the services.
    /// </para>
    /// <para>
    ///     When the <see cref="IConfigureDbContextService{TDbContext}"/> is not registered, an
    ///     <see cref="InvalidOperationException"/> is thrown.
    /// </para>
    /// </summary>
    /// <returns>The same instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     The <see cref="IConfigureDbContextService{TDbContext}"/> is not registered.
    /// </exception>
    public TBuilder ConfigureWithService()
    {
        Services.AddDbContext<TDbContext>((sp, builder) =>
        {
            var configurator = sp.GetService<IConfigureDbContextService<TDbContext>>();

            if (configurator is null)
                throw new InvalidOperationException(
                    "The IConfigureDbContextService is not registered. " +
                    "When using the ConfigureWithService method, it is necessary to register the " +
                    "IConfigureDbContextService<TDbContext>.");

            configurator.ConfigureDbContext(builder);
        }, Lifetime);
        return (TBuilder)this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work as pooled.
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContextPool<TDbContext>(configurer);
        return (TBuilder)this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work as pooled.
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContextPool<TDbContext>(configurer);
        return (TBuilder)this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work..
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureDbContext(Action<DbContextOptionsBuilder>? configurer = null)
    {
        Services.AddDbContext<TDbContext>(configurer, Lifetime);
        return (TBuilder)this;
    }

    /// <summary>
    /// Configure the <see cref="DbContext"/> for the unit of work..
    /// </summary>
    /// <param name="configurer">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContext<TDbContext>(configurer, Lifetime);
        return (TBuilder)this;
    }
}