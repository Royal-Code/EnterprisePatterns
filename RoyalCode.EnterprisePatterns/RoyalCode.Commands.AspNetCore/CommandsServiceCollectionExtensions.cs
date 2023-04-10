
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Commands.Abstractions;
using RoyalCode.Commands.Handlers;

namespace RoyalCode.Commands.AspNetCore;

public static class CommandsServiceCollectionExtensions
{
    
    public static void AddCreationCommand<TCreationHandler>(this IServiceCollection services)
    {
        AddCreationCommand(services, typeof(TCreationHandler));
    }

    public static void AddCreationCommand<TCreationHandler, TId>(this IServiceCollection services)
    {
        AddCreationCommand(services, typeof(TCreationHandler), typeof(TId));
    }

    public static void AddCreationCommand(this IServiceCollection services, Type creationHandlerType)
    {
        var interfaces = creationHandlerType.GetInterfaces()
            .Where(i => i.IsGenericType);
        foreach (var interfaceType in interfaces)
        {
            var genericType = interfaceType.GetGenericTypeDefinition();

            if (genericType == typeof(ICreationHandler<,>))
            {
                AddCreationHandler2(services, creationHandlerType, interfaceType);
                return;
            }
            else if (genericType == typeof(ICreationHandler<,,>))
            {
                AddCreationHandler3(services, creationHandlerType, interfaceType);
                return;
            }
            else if (genericType == typeof(ICreationHandler<,,,>))
            {
                throw new ArgumentException("The type ICreationHandler<,,,> requires a type for the id.");
            }
        }

        throw new ArgumentException($"The type {creationHandlerType} do not implements the ICreationHandler");
    }

    public static void AddCreationCommand(this IServiceCollection services, Type creationHandlerType, Type idType)
    {
        var interfaces = creationHandlerType.GetInterfaces()
            .Where(i => i.IsGenericType);
        foreach (var interfaceType in interfaces)
        {
            var genericType = interfaceType.GetGenericTypeDefinition();

            if (genericType == typeof(ICreationHandler<,>))
            {
                throw new ArgumentException("The type ICreationHandler<,> do not requires a type for the id.");
            }
            else if (genericType == typeof(ICreationHandler<,,>))
            {
                throw new ArgumentException("The type ICreationHandler<,,> do not requires a type for the id.");
            }
            else if (genericType == typeof(ICreationHandler<,,,>))
            {
                AddCreationHandler4(services, creationHandlerType, interfaceType, idType);
            }
        }

        throw new ArgumentException($"The type {creationHandlerType} do not implements the ICreationHandler");
    }

    private static void AddCreationHandler2(IServiceCollection services, Type creationHandlerType, Type interfaceType)
    {
        // registrar:
        // creationHandlerType como interfaceType
        // registra CreateCommandHandler<,>

        services.AddTransient(interfaceType, creationHandlerType);

        var arguments = interfaceType.GetGenericArguments();
        var commandHandlerType = typeof(CreateCommandHandler<,>).MakeGenericType(arguments[1], arguments[0]);
        services.AddTransient(commandHandlerType);
    }
    
    private static void AddCreationHandler3(IServiceCollection services, Type creationHandlerType, Type interfaceType)
    {
        // registrar:
        // creationHandlerType como interfaceType
        // registra CreateCommandHandler<,,>

        services.AddTransient(interfaceType, creationHandlerType);

        var arguments = interfaceType.GetGenericArguments();
        var commandHandlerType = typeof(CreateCommandHandler<,,>).MakeGenericType(arguments[2], arguments[1], arguments[0]);
        services.AddTransient(commandHandlerType);
    }

    private static void AddCreationHandler4(IServiceCollection services, Type creationHandlerType, Type interfaceType, Type idType)
    {
        // registrar:
        // creationHandlerType como interfaceType
        // registra CreateCommandHandler<,,,,>

        services.AddTransient(interfaceType, creationHandlerType);

        var arguments = interfaceType.GetGenericArguments();
        var commandHandlerType = typeof(CreateCommandHandler<,,,,>)
            .MakeGenericType(arguments[2], idType, arguments[3], arguments[1], arguments[0]);
        services.AddTransient(commandHandlerType);
    }
}
