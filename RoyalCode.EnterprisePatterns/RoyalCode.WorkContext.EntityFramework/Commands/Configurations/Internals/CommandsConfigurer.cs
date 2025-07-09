using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RoyalCode.WorkContext.EntityFramework.Commands.Configurations.Internals;

internal sealed class CommandsConfigurer<TDbContext> : ICommandsConfigurer
    where TDbContext : DbContext
{
    public CommandsConfigurer(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public IServiceCollection Services { get; }

    public ICommandsConfigurer AddHandlersFromAssembly(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // foreach class in the assembly
        foreach (var type in assembly.GetTypes())
        {
            // if is a concrete class
            if (type.IsClass && !type.IsAbstract)
            {
                // check interfaces
                foreach (var iface in type.GetInterfaces())
                {
                    // check if implements:
                    // - ICommandHandler<TDbContext, TCommand>
                    // - ICommandHandler<TDbContext, TCommand, TResponse>
                    if (iface.IsGenericType &&
                        (iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                         iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,,>)))
                    {
                        // register the type
                        Services.Add(new ServiceDescriptor(iface, type, lifetime));
                    }
                }
            }
        }

        return this;
    }

    public ICommandsConfigurer AddHandler<THandler>(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var type = typeof(THandler);
        var wasRegistered = false;

        // if is a concrete class
        if (type.IsClass && !type.IsAbstract)
        {
            // check interfaces
            foreach (var iface in type.GetInterfaces())
            {
                // check if implements:
                // - ICommandHandler<TDbContext, TCommand>
                // - ICommandHandler<TDbContext, TCommand, TResponse>
                if (iface.IsGenericType &&
                    (iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                     iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,,>)))
                {
                    // register the type
                    Services.Add(new ServiceDescriptor(iface, type, lifetime));
                    wasRegistered = true;
                }
            }
        }

        if (!wasRegistered)
        {
            throw new InvalidOperationException($"The type {type.FullName} does not implement any command handler interface.");
        }

        return this;
    }

    
}
