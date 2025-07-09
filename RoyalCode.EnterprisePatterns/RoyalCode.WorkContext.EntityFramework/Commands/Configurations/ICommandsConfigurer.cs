using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RoyalCode.WorkContext.EntityFramework.Commands.Configurations;

/// <summary>
/// Provides access to the service collection for command configuration.
/// </summary>
public interface ICommandsConfigurer
{
    /// <summary>
    /// Gets the service collection used for dependency injection.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Registers all command handlers found in the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for command handlers.</param>
    /// <param name="lifetime">The service lifetime for the handlers. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <returns>The current <see cref="ICommandsConfigurer"/> instance.</returns>
    ICommandsConfigurer AddHandlersFromAssembly(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped);

    /// <summary>
    /// Registers all command handlers found in the assembly of the specified type.
    /// </summary>
    /// <typeparam name="T">A type from the target assembly.</typeparam>
    /// <param name="lifetime">The service lifetime for the handlers. Default is <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <returns>The current <see cref="ICommandsConfigurer"/> instance.</returns>
    ICommandsConfigurer AddHandlersFromAssemblyOfType<T>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return AddHandlersFromAssembly(typeof(T).Assembly, lifetime);
    }

    /// <summary>
    /// Registers a command handler for the specified type.
    /// </summary>
    /// <typeparam name="THandler">The type of the command handler to register.</typeparam>
    /// <param name="lifetime">
    ///     The service lifetime for the handler. Default is <see cref="ServiceLifetime.Transient"/>.
    /// </param>
    /// <returns>The current <see cref="ICommandsConfigurer"/> instance.</returns>
    ICommandsConfigurer AddHandler<THandler>(ServiceLifetime lifetime = ServiceLifetime.Transient);
}
