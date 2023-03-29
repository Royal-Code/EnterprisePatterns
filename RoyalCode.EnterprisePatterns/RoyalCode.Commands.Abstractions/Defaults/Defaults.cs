
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.OperationResult;
using RoyalCode.Repositories.Abstractions;
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
        if (!contextModelProperties.All(p => p.IsMatchValid()))
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
            typeof(TModel),
            typeof(TContext),
            modelProperty,
            contextModelProperties,
            contextPropertiesWithoutModel);

        // gera a expressão
        var expression = GenerateExpression(resolution);

        throw new NotImplementedException();
    }

    /// <summary>
    /// Aqui será gerada uma expression do tipo:
    /// Expression{Func{TModel, IServiceProvider, CancellationToken, Task{IOperationResult{TContext}}}}
    /// 
    /// Para cada propriedade match da resolution, deve ser obtido o repositório do tipo da propridade
    /// do contexto e usado o método FindAsync para obter a entidade do contexto.
    /// 
    /// Se a propriedade do model for nula, então deve ser atribuído null para a propriedade do contexto.
    /// Quando a propriedade não é nula e o valor retornado do repositório for nulo,
    /// então deve ser gerado um erro de entidade não encontrada.
    /// 
    /// Para cada propriedade que é um serviço, deve ser obtido o serviço do tipo da propriedade.
    /// </summary>
    /// <param name="resolution">A resolução com as propriedades.</param>
    /// <returns>Uma expressão que cria o contexto e gera um OperationResult.</returns>
    private Expression GenerateExpression(GeneratorPropertiesResolution resolution)
    {
        // parâmetros da expressão
        var modelParameter = Expression.Parameter(resolution.ModelType, "model");
        var spParameter = Expression.Parameter(typeof(IServiceProvider), "sp");
        var ctParameter = Expression.Parameter(typeof(CancellationToken), "ct");

        // body expressions
        var bodyExpressions = new List<Expression>();
        // variables
        var variables = new List<ParameterExpression>();

        // expression da variável result do tipo BaseResult
        var resultVariable = Expression.Variable(typeof(BaseResult), "result");

        // para cada match, deve ser gerado um bloco de código que faz a busca da entidade no repositório.
        foreach (var match in resolution.PropertyMatches)
        {
            GenerateFindExpressions(match, modelParameter, spParameter, ctParameter, resultVariable, variables, bodyExpressions);
        }

        throw new NotImplementedException();
    }

    /// <summary>
    /// Gera as expressões para a busca de uma entidade no repositório, adicionando os comandos a lista de expressões.
    /// </summary>
    /// <remarks>
    /// O código gerado deverá ser assim quando a propriedade model é value type e não nullable:
    /// 
    /// var repository = sp.GetService(typeof(IRepository{EntityType})) as IRepository{EntityType};
    /// var entity = await repository.FindAsync(model.PropertyId, ct);
    /// if (entity == null)
    ///     result.NotFound(CommandsErrorMessages.CreateNotFoundMessage{EntityType}(id), "ModelPropertyInfoName");
    /// 
    /// ----------------------------
    /// 
    /// O código gerado deverá ser assim quando a propriedade model é value type e nullable:
    /// 
    /// var entity = null;
    /// if (model.PropertyId.HasValue)
    /// {
    ///     var repository = sp.GetService(typeof(IRepository{EntityType})) as IRepository{EntityType};
    ///     entity = await repository.FindAsync(model.PropertyId.Value, ct);
    ///     if (entity == null)
    ///         result.NotFound(CommandsErrorMessages.CreateNotFoundMessage{EntityType}(id), "ModelPropertyInfoName");
    /// }
    /// 
    /// ----------------------------
    /// 
    /// O código gerado deverá ser assim quando a propriedade model não é value type:
    /// 
    /// var entity = null;
    /// if (model.PropertyId != null)
    /// {
    ///     var repository = sp.GetService(typeof(IRepository{EntityType})) as IRepository{EntityType};
    ///     entity = await repository.FindAsync(model.PropertyId, ct);
    ///     if (entity == null)
    ///         result.NotFound(CommandsErrorMessages.CreateNotFoundMessage{EntityType}(id), "ModelPropertyInfoName");
    /// }
    /// 
    /// </remarks>
    private void GenerateFindExpressions(
        ContextModelPropertyMatch match,
        ParameterExpression modelParameter,
        ParameterExpression spParameter,
        ParameterExpression ctParameter,
        ParameterExpression resultVariable,
        List<ParameterExpression> variables,
        List<Expression> bodyExpressions)
    {
        // declaração da variável da entidade
        var entityVariable = Expression.Variable(match.ContextProperty.PropertyType, "entity");
        variables.Add(entityVariable);

        // obter IRepository do service provider
        var repositoryType = typeof(IRepository<>).MakeGenericType(match.ContextProperty.PropertyType);
        var repositoryVariable = Expression.Variable(repositoryType, "repository");
        variables.Add(repositoryVariable);
        var repositoryExpression = Expression.Call(spParameter, typeof(IServiceProvider).GetMethod("GetService")!, Expression.Constant(repositoryType));
        bodyExpressions.Add(Expression.Assign(repositoryVariable, repositoryExpression));

        // obter a propriedade do model
        var modelProperty = Expression.Property(modelParameter, match.ModelProperty);
        // se o tipo do model é nullable, então obter o valor da propriedade Value
        var modelPropertyValue = match.ModelProperty.PropertyType.IsNullable()
            ? Expression.Property(modelProperty, "Value")
            : modelProperty;

        // expressão para obter a entidade do repositório
        var findExpression = Expression.Call(repositoryVariable, 
            repositoryType.GetMethod("FindAsync")!, modelPropertyValue, ctParameter);

        // atribuir a entidade para a variável


        // obter a propriedade do contexto
        var contextProperty = Expression.Property(resultVariable, match.ContextProperty);



        throw new NotImplementedException();
    }
}

internal record GeneratorPropertiesResolution(
    Type ModelType,
    Type ContextType,
    PropertyInfo ModelProperty, 
    IEnumerable<ContextModelPropertyMatch> PropertyMatches, 
    IEnumerable<PropertyInfo> ServiceProperties);

internal record ContextModelPropertyMatch(PropertyInfo ContextProperty, PropertyInfo ModelProperty)
{
    public Expression? Expression { get; set; }

    public bool IsMatchValid()
    {
        // tenta obter tipos genéricos das propriedades caso sejam IEnumerables.
        var contextPropertyIsEnumerable = ContextProperty.PropertyType.TryGetEnumerableGenericType(out var contextType);
        var modelPropertyIsEnumerable = ModelProperty.PropertyType.TryGetEnumerableGenericType(out var modelType);

        // valida se ambas são Enumerable ou não, caso uma seja e outra não, retorna false.
        if (contextPropertyIsEnumerable != modelPropertyIsEnumerable)
            return false;

        // se não são Enumerable, então usa os tipos das propriedades.
        contextType ??= ContextProperty.PropertyType;
        modelType ??= ModelProperty.PropertyType;

        // verifica se o tipo da propriedade do contexto possui uma propriedade Id
        // com o mesmo tipo da propriedade do model.
        return contextType.GetProperty("Id")?.PropertyType == modelType;
    }
}

internal record ServiceProperty(PropertyInfo Property)
{
    public Expression? Expression { get; set; }
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