
using RoyalCode.OperationResult;

namespace RoyalCode.Commands.Abstractions;

public interface ICreateCommandHandler<TEntity, in TModel>
    where TEntity : class
    where TModel : class
{
    Task<IOperationResult<TEntity>> HandleAsync(TModel model, CancellationToken token);
}


public interface ICreateCommandHandler<TEntity, TId, in TModel>
    where TEntity : class
    where TModel : class
{
    Task<IOperationResult<TEntity>> HandleAsync(TId id, TModel model, CancellationToken token);
}
