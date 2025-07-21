using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.WorkContext.Commands;
using RoyalCode.WorkContext.EntityFramework.Internal;
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
            TryAddHandler(type, lifetime);

        return this;
    }

    public ICommandsConfigurer AddHandler<THandler>(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var type = typeof(THandler);
        var wasRegistered = TryAddHandler(type, lifetime);

        if (!wasRegistered)
            throw new InvalidOperationException($"The type {type.FullName} does not implement any command handler interface.");

        return this;
    }

    private bool TryAddHandler(Type type, ServiceLifetime lifetime)
    {
        var wasRegistered = false;

        // if is a concrete class
        if (type.IsClass && !type.IsAbstract)
        {
            // check interfaces
            foreach (var iface in type.GetInterfaces())
            {
                if (!iface.IsGenericType)
                    continue;

                bool register = false;

                if (iface.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                {
                    register = true;
                    var iDispatcherType = typeof(IServiceCommandRequestDispatcher<>)
                        .MakeGenericType(iface.GenericTypeArguments[0]);
                    var dispatcherType = typeof(DefaultServiceCommandRequestDispatcher<,>)
                        .MakeGenericType(typeof(TDbContext), iface.GenericTypeArguments[0]);
                    Services.Add(new ServiceDescriptor(iDispatcherType, dispatcherType, ServiceLifetime.Singleton));
                }

                if (iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                {
                    register = true;
                    var iDispatcherType = typeof(IServiceCommandRequestDispatcher<,>)
                        .MakeGenericType(iface.GenericTypeArguments[0], iface.GenericTypeArguments[1]);
                    var dispatcherType = typeof(DefaultServiceCommandRequestDispatcher<,,>)
                        .MakeGenericType(typeof(TDbContext), iface.GenericTypeArguments[0], iface.GenericTypeArguments[1]);
                    Services.Add(new ServiceDescriptor(iDispatcherType, dispatcherType, ServiceLifetime.Singleton));
                }

                if (register)
                {
                    // register the type
                    Services.Add(new ServiceDescriptor(iface, type, lifetime));
                    wasRegistered = true;
                }
            }
        }

        return wasRegistered;
    }
}
