using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Commands.Abstractions.Defaults;
using RoyalCode.Commands.AspNetCore;
using System.Linq.Expressions;
using System.Reflection;

namespace RoyalCode.Commands.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CreationAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ContextFactoryAttribute : Attribute { }

public static class Registration // extension methods
{
    public static void AddCreateCommand<T>(this IServiceCollection services)
        where T : class
    {
        services.AddCreateCommand(typeof(T));
    }

    public static void AddCreateCommand(this IServiceCollection services, Type classToScan)
    {
        // get all methods with the CreationAttribute
        var methods = classToScan.GetMethods()
            .Where(m => m.GetCustomAttribute<CreationAttribute>() is not null)
            .ToList();

        if (methods.Count == 0)
            return;

        // check if the classToScan was registred before, if not, add as transient.
        var serviceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == classToScan);
        if (serviceDescriptor == null)
            services.AddTransient(classToScan);

        foreach (var method in methods)
        {
            services.AddCreateCommand(classToScan, method);
        }
    }

    public static void AddCreateCommand(this IServiceCollection services, Type serviceType, MethodInfo handlerMethod)
    {
        var description = CreateCommandDescription.Create(serviceType, handlerMethod);
        services.AddCreateCommand(description);
    }

    private static void AddCreateCommand(this IServiceCollection services, CreateCommandDescription description)
    {
        switch (description.ModelDescription.CommandModelType)
        {
            case CommandModelType.ModelOnly:
                services.AddCreateCommandModelOnly(description);
                break;
            case CommandModelType.ContextModel:
                services.AddCreateCommandContextModel(description);
                break;
            case CommandModelType.ContextEntityModel:
                services.AddCreateCommandContextEntityModel(description);
                break;
        }
    }

    private static void AddCreateCommandModelOnly(this IServiceCollection services,
        CreateCommandDescription description)
    {
        // registrar:
        //   DefaultCreationHandler<TService, TEntity, TModel> : ICreationHandler<TEntity, TModel>
        //   Func<TService, TModel, TEntity>

        var creationHandlerImplType = typeof(DefaultCreationHandler<,,>)
            .MakeGenericType(description.ServiceType, description.ModelDescription.ModelType, description.EntityType);

        var functionServiceType = typeof(Func<,,>)
            .MakeGenericType(description.ServiceType, description.ModelDescription.ModelType, description.EntityType);

        // create lambda expression for the function execute the service method
        var serviceParameter = Expression.Parameter(description.ServiceType, "service");
        var modelParameter = Expression.Parameter(description.ModelDescription.ModelType, "model");
        var callExpression = Expression.Call(serviceParameter, description.HandlerMethod, modelParameter);
        var lambdaExpression = Expression.Lambda(functionServiceType, callExpression, serviceParameter, modelParameter);

        var functionServiceInstance = lambdaExpression.Compile();

        // add services
        services.AddCreationCommand(creationHandlerImplType);
        services.AddSingleton(functionServiceType, functionServiceInstance);
    }

    private static void AddCreateCommandContextModel(this IServiceCollection services,
        CreateCommandDescription description)
    {
        // registrar:
        //   DefaultCreationHandler<TService, TEntity, TModel> : ICreationHandler<TEntity, TModel>
        //   Func<TService, TContext, TEntity>

        var creationHandlerImplType = typeof(DefaultCreationHandler<,,>)
            .MakeGenericType(description.ServiceType, description.ModelDescription.ContextType!, description.EntityType);

        var functionServiceType = typeof(Func<,,>)
            .MakeGenericType(description.ServiceType, description.ModelDescription.ContextType!, description.EntityType);

        // create lambda expression for the function execute the service method
        var serviceParameter = Expression.Parameter(description.ServiceType, "service");
        var contextParameter = Expression.Parameter(description.ModelDescription.ContextType!, "context");
        var callExpression = Expression.Call(serviceParameter, description.HandlerMethod, contextParameter);
        var lambdaExpression = Expression.Lambda(functionServiceType, callExpression, serviceParameter, contextParameter);

        var functionServiceInstance = lambdaExpression.Compile();

        // add services
        services.AddCreationCommand(creationHandlerImplType);
        services.AddSingleton(functionServiceType, functionServiceInstance);
    }

    private static void AddCreateCommandContextEntityModel(this IServiceCollection services,
        CreateCommandDescription description)
    {
        throw new NotImplementedException();
    }
}

public class CreateCommandDescription // descriptions namespace?
{
    private CreateCommandDescription(Type serviceType, Type entityType, CommandModelDescription modelDescription, MethodInfo handlerMethod)
    {
        ServiceType = serviceType;
        EntityType = entityType;
        ModelDescription = modelDescription;
        HandlerMethod = handlerMethod;
    }

    public Type ServiceType { get; }

    public MethodInfo HandlerMethod { get; }
    
    public Type EntityType { get; }

    public CommandModelDescription ModelDescription { get; }

    public static CreateCommandDescription Create(Type serviceType, MethodInfo method)
    {
        ValidateMethod(method);

        Type entityType = method.ReturnType;
        Type modelType = method.GetParameters()[0].ParameterType;
        var modelDescription = CommandModelDescription.Create(modelType);

        var description = new CreateCommandDescription(serviceType, entityType, modelDescription, method);
        return description;
    }

    private static void ValidateMethod(MethodInfo method)
    {
        // assert return type is not void
        if (method.ReturnType == typeof(void))
            throw new InvalidOperationException("The method must not return void.");

        // assert return type is not Task
        if (method.ReturnType == typeof(Task))
            throw new InvalidOperationException("The method must not return Task.");

        // assert the method has one parameter
        var parameters = method.GetParameters();
        if (parameters.Length is not 1)
            throw new InvalidOperationException("The method must have one parameter.");
    }
}

public class CommandModelDescription
{
    private CommandModelDescription(
        CommandModelType commandModelType,
        Type modelType,
        Type? rootEntityType,
        Type? rootIdType,
        Type? contextType)
    {
        if (rootEntityType is not null && rootIdType is null)
            throw new InvalidOperationException($"The root entity '{rootEntityType.FullName}' must have a property with name 'Id'");

        CommandModelType = commandModelType;
        ModelType = modelType;
        RootEntityType = rootEntityType;
        RootIdType = rootIdType;
        ContextType = contextType;
    }

    public CommandModelType CommandModelType { get; }

    public Type ModelType { get; }
    
    public Type? ContextType { get; }

    public Type? RootEntityType { get; }

    public Type? RootIdType { get; }

    public static CommandModelDescription Create(Type type)
    {
        Type modelType = type;
        Type? contextType = null;
        Type? rootEntityType = null;
        CommandModelType commandType = CommandModelType.ModelOnly;

        // check if model implements ICommandContext<> or ICommandContext<,>
        var interfaces = type.GetInterfaces().Where(i => i.IsGenericType).Select(i => i.GetGenericTypeDefinition());
        foreach (var i in interfaces)
        {
            if (i == typeof(ICreationContext<>))
            {
                commandType = CommandModelType.ContextModel;
                contextType = type;
                modelType = type.GetGenericArguments()[0];
                break;
            }
            else if (i == typeof(ICreationContext<,>))
            {
                commandType = CommandModelType.ContextEntityModel;
                contextType = type;
                rootEntityType = type.GetGenericArguments()[0];
                modelType = type.GetGenericArguments()[1];
                break;
            }
        }

        Type? rootIdType = rootEntityType?.GetProperty("Id")?.PropertyType;

        var description = new CommandModelDescription(commandType, modelType, rootEntityType, rootIdType, contextType);
        return description;
    }
}

public enum CommandModelType
{
    ModelOnly,
    ContextModel,
    ContextEntityModel
}