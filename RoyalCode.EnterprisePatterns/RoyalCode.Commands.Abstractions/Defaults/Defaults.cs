
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.OperationResult;
using System.Diagnostics.CodeAnalysis;
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

    private IContextBuilder<TContext, TModel> CreateContextBuilder<TContext, TModel>(Type contextFactoryType)
        where TContext : ICommandContext<TModel>
        where TModel : class
    {
        //Microsoft.Extensions.DependencyInjection.IServiceProviderIsService

        return (IContextBuilder<TContext, TModel>)serviceProvider.GetRequiredService(contextFactoryType);
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
    internal Type? TryGenerate<TContext, TModel>()
        where TContext : ICommandContext<TModel>
        where TModel : class
    {
        var contextType = typeof(TContext);
        var modelType = typeof(TModel);





        throw new NotImplementedException();
    }
}