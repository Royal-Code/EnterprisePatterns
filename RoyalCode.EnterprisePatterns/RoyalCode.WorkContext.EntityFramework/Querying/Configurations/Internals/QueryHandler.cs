using Microsoft.EntityFrameworkCore;
using RoyalCode.WorkContext.Abstractions.Querying;

namespace RoyalCode.WorkContext.EntityFramework.Querying.Configurations.Internals;

internal sealed class QueryHandler<TDbContext, TRequest, TEntity> : IQueryHandler<TDbContext, TRequest, TEntity>
    where TDbContext : DbContext
    where TRequest : IQueryRequest<TEntity>
    where TEntity : class
{
    private readonly Func<TRequest, TDbContext, CancellationToken, Task<IEnumerable<TEntity>>> handler;

    public QueryHandler(Func<TRequest, TDbContext, CancellationToken, Task<IEnumerable<TEntity>>> handler)
    {
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public Task<IEnumerable<TEntity>> HandleAsync(TRequest request, TDbContext db, CancellationToken ct = default)
        => handler(request, db, ct);
}

internal sealed class QueryHandler<TDbContext, TRequest, TEntity, TModel> : IQueryHandler<TDbContext, TRequest, TEntity, TModel>
    where TDbContext : DbContext
    where TRequest : IQueryRequest<TEntity, TModel>
    where TEntity : class
{
    private readonly Func<TRequest, TDbContext, CancellationToken, Task<IEnumerable<TModel>>> handler;

    public QueryHandler(Func<TRequest, TDbContext, CancellationToken, Task<IEnumerable<TModel>>> handler)
    {
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public Task<IEnumerable<TModel>> HandleAsync(TRequest request, TDbContext db, CancellationToken ct = default)
        => handler(request, db, ct);
}
