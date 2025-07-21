using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RoyalCode.WorkContext.EntityFramework.Querying.Configurations;

/// <summary>
/// Provides access to the service collection for query configuration.
/// </summary>
/// <typeparam name="TConfigurer">The type of the configurer.</typeparam>
public interface IQueryConfigurerBase<TConfigurer>
    where TConfigurer : IQueryConfigurerBase<TConfigurer>
{
    /// <summary>
    /// Gets the service collection used for dependency injection.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Registers all query handlers found in the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for query handlers.</param>
    /// <param name="lifetime">The service lifetime for the handlers. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <returns>The current <see cref="IQueryConfigurerBase{TDbContext}"/> instance.</returns>
    TConfigurer AddHandlersFromAssembly(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped);

    /// <summary>
    /// Registers all query handlers found in the assembly of the specified type.
    /// </summary>
    /// <typeparam name="T">A type from the target assembly.</typeparam>
    /// <param name="lifetime">The service lifetime for the handlers. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <returns>The current <see cref="IQueryConfigurerBase{TDbContext}"/> instance.</returns>
    TConfigurer AddHandlersFromAssemblyOfType<T>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return AddHandlersFromAssembly(typeof(T).Assembly, lifetime);
    }

    /// <summary>
    /// Registers a query handler for the specified type.
    /// </summary>
    /// <typeparam name="THandler">The type of the query handler to register.</typeparam>
    /// <param name="lifetime">
    ///     The service lifetime for the handler. Default is <see cref="ServiceLifetime.Transient"/>.
    /// </param>
    /// <returns>The current <see cref="IQueryConfigurerBase{TDbContext}"/> instance.</returns>
    TConfigurer AddHandler<THandler>(ServiceLifetime lifetime = ServiceLifetime.Transient);

    /// <summary>
    /// Configures query handler registrations using the specified configuration type.
    /// </summary>
    /// <typeparam name="TConfigurations">The type implementing <see cref="IQueryHandlerConfigurations"/>.</typeparam>
    /// <returns>The current <see cref="IQueryConfigurerBase{TDbContext}"/> instance.</returns>
    TConfigurer Configure<TConfigurations>()
        where TConfigurations : class, IQueryHandlerConfigurations, new();

    /// <summary>
    /// Configures query handler registrations using the specified configuration instance.
    /// </summary>
    /// <param name="configurations">The configuration instance.</param>
    /// <returns>The current <see cref="IQueryConfigurerBase{TDbContext}"/> instance.</returns>
    TConfigurer Configure(IQueryHandlerConfigurations configurations);
}

/// <summary>
/// Provides methods to configure query handlers for a specific <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
/// <typeparam name="TConfigurer">The type of the configurer.</typeparam>
public interface IQueryConfigurer<out TDbContext, TConfigurer> : IQueryConfigurerBase<TConfigurer>, IQueryHandlerConfigurer<TDbContext, TConfigurer>
    where TDbContext : DbContext
    where TConfigurer : IQueryConfigurer<TDbContext, TConfigurer>
{ }

/// <summary>
/// Provides methods to configure query handlers for a specific <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
public interface IQueryConfigurer<TDbContext> : IQueryConfigurer<TDbContext, IQueryConfigurer<TDbContext>>
    where TDbContext : DbContext
{ }