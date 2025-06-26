namespace RoyalCode.WorkContext.Abstractions.Quering;

public interface IAsyncQueryHandler<TEntity>
{
    public IAsyncEnumerable<TEntity> HandleAsync(IQueryRequest<TEntity> request, CancellationToken ct = default);
}

public interface IAsyncQueryHandler<TEntity, TModel>
{
    public IAsyncEnumerable<TModel> HandleAsync(IQueryRequest<TEntity, TModel> request, CancellationToken ct = default);
}