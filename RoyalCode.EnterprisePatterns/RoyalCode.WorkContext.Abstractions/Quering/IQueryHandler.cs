namespace RoyalCode.WorkContext.Abstractions.Quering;

public interface IQueryHandler<TEntity>
{
    public Task<IEnumerable<TEntity>> HandleAsync(IQueryRequest<TEntity> request, CancellationToken ct = default);
}

public interface IQueryHandler<TEntity, TModel>
{
    public Task<IEnumerable<TModel>> HandleAsync(IQueryRequest<TEntity, TModel> request, CancellationToken ct = default);
}