
using RoyalCode.OperationResult;

namespace RoyalCode.Commands.Abstractions;

public interface ICommandContextFactory
{
    Task<IOperationResult<TContext>> CreateAsync<TContext, TModel>(TModel model)
        where TContext : ICommandContext<TModel>
        where TModel : class;

    Task<IOperationResult<TContext>> CreateAsync<TContext, TRootEntity, TModel>(TRootEntity entity, TModel model)
        where TContext : ICommandContext<TRootEntity, TModel>
        where TRootEntity : class
        where TModel : class;
}

public interface IContextBuilder<TContext, TModel>
    where TContext : ICommandContext<TModel>
    where TModel : class
{
    Task<IOperationResult<TContext>> BuildAsync(TModel model);
}