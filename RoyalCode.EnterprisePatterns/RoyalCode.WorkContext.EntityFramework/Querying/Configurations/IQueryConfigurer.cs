using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.WorkContext.EntityFramework.Querying.Configurations.Internals;
using System.Reflection;

namespace RoyalCode.WorkContext.EntityFramework.Querying.Configurations;

/// <summary>
/// Provides access to the service collection for query configuration.
/// </summary>
public interface IQueryConfigurer
{
    /// <summary>
    /// Gets the service collection used for dependency injection.
    /// </summary>
    IServiceCollection Services { get; }
}

/// <summary>
/// Provides methods to configure query handlers for a specific <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
public interface IQueryConfigurer<TDbContext> : IQueryConfigurer, IQueryHandlerConfigurer<TDbContext, IQueryConfigurer<TDbContext>>
    where TDbContext : DbContext
{
    /// <summary>
    /// Registers all query handlers found in the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for query handlers.</param>
    /// <param name="lifetime">The service lifetime for the handlers. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <returns>The current <see cref="IQueryConfigurer{TDbContext}"/> instance.</returns>
    IQueryConfigurer<TDbContext> AddHandlersFromAssembly(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped);

    /// <summary>
    /// Registers all query handlers found in the assembly of the specified type.
    /// </summary>
    /// <typeparam name="T">A type from the target assembly.</typeparam>
    /// <param name="lifetime">The service lifetime for the handlers. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <returns>The current <see cref="IQueryConfigurer{TDbContext}"/> instance.</returns>
    IQueryConfigurer<TDbContext> AddHandlersFromAssemblyOfType<T>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return AddHandlersFromAssembly(typeof(T).Assembly, lifetime);
    }

    /// <summary>
    /// Configures query handler registrations using the specified configuration type.
    /// </summary>
    /// <typeparam name="TConfigurations">The type implementing <see cref="IQueryHandlerConfigurations"/>.</typeparam>
    /// <returns>The current <see cref="IQueryConfigurer{TDbContext}"/> instance.</returns>
    IQueryConfigurer<TDbContext> Configure<TConfigurations>()
        where TConfigurations : class, IQueryHandlerConfigurations, new()
    {
        var configurations = new TConfigurations();
        var configurator = new QueryHandlerConfigurer<TDbContext>(Services);

        configurations.Configure(configurator);

        return this;
    }

    /// <summary>
    /// Configures query handler registrations using the specified configuration instance.
    /// </summary>
    /// <param name="configurations">The configuration instance.</param>
    /// <returns>The current <see cref="IQueryConfigurer{TDbContext}"/> instance.</returns>
    IQueryConfigurer<TDbContext> Configure(IQueryHandlerConfigurations configurations)
    {
        ArgumentNullException.ThrowIfNull(configurations);

        var configurator = new QueryHandlerConfigurer<TDbContext>(Services);
        configurations.Configure(configurator);

        return this;
    }
}