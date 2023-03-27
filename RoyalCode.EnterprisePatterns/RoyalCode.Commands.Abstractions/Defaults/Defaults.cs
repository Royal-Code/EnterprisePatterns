
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.OperationResult;
using RoyalCode.WorkContext.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoyalCode.Commands.Abstractions.Defaults;

internal class DefaultCommandContextFactory : ICommandContextFactory
{
    private readonly CommandContextFactoryMap map;
    private readonly IServiceProvider serviceProvider;
    private readonly ContextBuilderGenerator generator;

    public async Task<IOperationResult<TContext>> CreateAsync<TContext, TModel>(TModel model)
        where TContext : ICommandContext<TModel>
        where TModel : class
    {
        // tenta obter o tipo do context builder.
        if (!map.ContainsKey((typeof(TContext), typeof(TModel)), out Type? contextBuilderType))
        {
            if (TryGetContextBuilderType<TContext, TModel>(out contextBuilderType))
                map.Add((typeof(TContext), typeof(TModel)), contextBuilderType);
            else
                throw new InvalidOperationException($"Context builder for {typeof(TContext)} not found.");
        }

        var builder = (IContextBuilder<TContext, TModel>)serviceProvider.GetRequiredService(contextBuilderType);
        return await builder.BuildAsync(model);
    }

    private bool TryGetContextBuilderType<TContext, TModel>([NotNullWhen(true)] out Type? contextBuilderType)
        where TContext : ICommandContext<TModel>
        where TModel : class
    {
        var type = typeof(IContextBuilder<TContext, TModel>);
        if(serviceProvider.GetService<IServiceProviderIsService>()?.IsService(type) ?? false)
        {
            contextBuilderType = type;
            return true;
        }

        contextBuilderType = generator.TryGenerate<TContext, TModel>();
        return contextBuilderType is not null;
    }

    public Task<IOperationResult<TContext>> CreateAsync<TContext, TRootEntity, TModel>(TRootEntity entity, TModel model)
        where TContext : ICommandContext<TRootEntity, TModel>
        where TRootEntity : class
        where TModel : class
    {
        throw new NotImplementedException();
    }
}


internal sealed class CommandContextFactoryMap
{
    public static CommandContextFactoryMap Instance { get; } = new();

    private readonly Dictionary<(Type, Type), Type> contextFactoriesTypes = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool ContainsKey((Type, Type) value, [NotNullWhen(true)] out Type? contextFactoryType)
    {
        return contextFactoriesTypes.TryGetValue(value, out contextFactoryType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Add((Type, Type) key, Type contextBuilderType)
    {
        lock(contextFactoriesTypes)
        {
            contextFactoriesTypes[key] = contextBuilderType;
        }
    }
}


internal sealed class ContextBuilderGenerator
{
    private readonly IServiceProviderIsService serviceProviderIsService;

    internal Type? TryGenerate<TContext, TModel>()
        where TContext : ICommandContext<TModel>
        where TModel : class
    {
        var contextType = typeof(TContext);
        var modelType = typeof(TModel);

        // obter todas propriedades do contexto
        var contextProperties = contextType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

        // separar das propriedades a propriedade do model
        var modelProperty = contextProperties.FirstOrDefault(p => p.Name == nameof(ICommandContext<TModel>.Model))!;
        contextProperties.Remove(modelProperty);

        // obter todas propriedades do model
        var modelProperties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

        // entre as propriedades do model, procurar as quais estão relacionadas com propriedades do contexto,
        // onde o nome da propriedade do contexto é igual ao nome da propriedade do model + 'Id',
        // então deve selecionar o par de propriedades (contexto + model).
        var contextModelProperties = contextProperties
            .Join(modelProperties,
                contextProperty => contextProperty.Name,
                modelProperty => modelProperty.Name + "Id",
                (contextProperty, modelProperty) => new ContextModelPropertyMatch(contextProperty, modelProperty))
            .ToList();
        
        // verificar se os tipos das propriedades do contexto possuem uma propriedade Id
        // que é do mesmo tipo da propriedade do model.
        // se alguma é inválida, então não é possível gerar o builder.
        if (!contextModelProperties.All(p => p.IsMachValid()))
            return null;

        // obter as propriedades do contexto que não estão relacionadas com propriedades do model.
        var contextPropertiesWithoutModel = contextProperties
            .Except(contextModelProperties.Select(p => p.ContextProperty))
            .ToList();

        // verifica se as propriedades do contexto que não estão relacionadas com propriedades do model
        // são propriedades que podem ser injetadas pelo serviço de injeção de dependência.
        var contextPropertiesWithoutModelCanBeInjected = contextPropertiesWithoutModel
            .All(p => serviceProviderIsService.IsService(p.PropertyType));

        // se alguma é inválida, então não é possível gerar o builder.
        if (!contextPropertiesWithoutModelCanBeInjected)
            return null;

        // cria a resolução das propriedades
        var resolution = new GeneratorPropertiesResolution(
            modelProperty,
            contextModelProperties,
            contextPropertiesWithoutModel);

        // gera a expressão
        var expression = GenerateExpression(resolution);

        throw new NotImplementedException();
    }

    private Expression GenerateExpression(GeneratorPropertiesResolution resolution)
    {
        throw new NotImplementedException();
    }
}

internal record GeneratorPropertiesResolution(
    PropertyInfo ModelProperty, 
    IEnumerable<ContextModelPropertyMatch> PropertyMatches, 
    IEnumerable<PropertyInfo> ServiceProperties);

internal record ContextModelPropertyMatch(PropertyInfo ContextProperty, PropertyInfo ModelProperty)
{
    public bool IsMachValid()
    {
        // aqui, não está sendo atendido se as propriedades são coleções !!!

        return ContextProperty.PropertyType.GetProperty("Id")?.PropertyType == ModelProperty.PropertyType;
    }
}

internal sealed class ContextBuilderMap
{
    private readonly Dictionary<(Type, Type), object> modelBuilders = new();

    public Func<TModel, IServiceProvider, Task<IOperationResult<TContext>>>? GetBuilder<TContext, TModel>()
        where TContext : ICommandContext<TModel>
        where TModel : class
    {
        return modelBuilders.TryGetValue((typeof(TContext), typeof(TModel)), out var builder)
            ? (Func<TModel, IServiceProvider, Task<IOperationResult<TContext>>>)builder
            : null;
    }

    // add builder
    public void AddBuilder<TContext, TModel>(Func<TModel, IServiceProvider, Task<IOperationResult<TContext>>> builder)
        where TContext : ICommandContext<TModel>
        where TModel : class
    {
        lock(modelBuilders)
        {
            modelBuilders[(typeof(TContext), typeof(TModel))] = builder;
        }
    }
}

internal sealed class DefaultContextBuilder<TContext, TModel> : IContextBuilder<TContext, TModel>
    where TContext : ICommandContext<TModel>
    where TModel : class
{
    private readonly IServiceProvider serviceProvider;
    private readonly ContextBuilderMap contextBuilderMap;

    public DefaultContextBuilder(IServiceProvider serviceProvider, ContextBuilderMap contextBuilderMap)
    {
        this.serviceProvider = serviceProvider;
        this.contextBuilderMap = contextBuilderMap;
    }

    public Task<IOperationResult<TContext>> BuildAsync(TModel model)
    {
        var builderFunction = contextBuilderMap.GetBuilder<TContext, TModel>()
            ?? throw new InvalidOperationException(
                $"No builder is available for context '{typeof(TContext).FullName}' and model '{typeof(TModel).FullName}'.");

        return builderFunction(model, serviceProvider);
    }
}

internal sealed class DefaultValidableContextBuilder<TContext, TModel> : IContextBuilder<TContext, TModel>
    where TContext : ICommandContext<TModel>, IValidableContext
    where TModel : class
{


    public Task<IOperationResult<TContext>> BuildAsync(TModel model)
    {
        throw new NotImplementedException();
    }
}