using Microsoft.EntityFrameworkCore;
using RoyalCode.WorkContext.Abstractions.Querying;

namespace RoyalCode.WorkContext.EntityFramework.Querying.Configurations.Internals;

internal sealed class AsyncQueryHandler<TDbContext, TRequest, TEntity> : IAsyncQueryHandler<TDbContext, TRequest, TEntity>
    where TDbContext : DbContext
    where TRequest : IAsyncQueryRequest<TEntity>
    where TEntity : class
{
    private readonly Func<TRequest, TDbContext, CancellationToken, IAsyncEnumerable<TEntity>> handler;

    public AsyncQueryHandler(Func<TRequest, TDbContext, CancellationToken, IAsyncEnumerable<TEntity>> handler)
    {
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public IAsyncEnumerable<TEntity> HandleAsync(TRequest request, TDbContext db, CancellationToken ct = default)
        => handler(request, db, ct);
}

internal sealed class AsyncQueryHandler<TDbContext, TRequest, TEntity, TModel> : IAsyncQueryHandler<TDbContext, TRequest, TEntity, TModel>
    where TDbContext : DbContext
    where TRequest : IAsyncQueryRequest<TEntity, TModel>
    where TEntity : class
{
    private readonly Func<TRequest, TDbContext, CancellationToken, IAsyncEnumerable<TModel>> handler;

    public AsyncQueryHandler(Func<TRequest, TDbContext, CancellationToken, IAsyncEnumerable<TModel>> handler)
    {
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public IAsyncEnumerable<TModel> HandleAsync(TRequest request, TDbContext db, CancellationToken ct = default)
        => handler(request, db, ct);
}