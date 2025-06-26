using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.WorkContext.Abstractions.Querying;
using RoyalCode.WorkContext.EntityFramework.Querying;
using System.Collections.Concurrent;

#pragma warning disable S2743 // Static fields should not be used in generic types

namespace RoyalCode.WorkContext.EntityFramework.Internal;

internal static class QueryRequestHandler<TDbContext>
    where TDbContext : DbContext
{
    private static readonly ConcurrentDictionary<Type, object> handlers = new();

    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(
        IQueryRequest<TEntity> request, 
        TDbContext db,
        IServiceProvider sp, 
        CancellationToken ct = default)
        where TEntity : class
    {
        var requestType = request.GetType();

        var handler = (QueryRequestHandler<TDbContext, TEntity>)handlers.GetOrAdd(requestType, type =>
            {
                var handlerType = typeof(DefaultQueryRequestHandler<,,>).MakeGenericType(typeof(TDbContext), type, typeof(TEntity));
                return Activator.CreateInstance(handlerType) ?? 
                       throw new InvalidOperationException(
                           $"Cannot create an instance of query request handler for {typeof(TEntity)}");
            });

        return handler.QueryAsync(request, db, sp, ct);
    }

    public static Task<IEnumerable<TModel>> QueryAsync<TEntity, TModel>(
        IQueryRequest<TEntity, TModel> request,
        TDbContext db,
        IServiceProvider sp,
        CancellationToken ct = default)
        where TEntity : class
    {
        var requestType = request.GetType();

        var handler = (QueryRequestHandler<TDbContext, TEntity, TModel>)handlers.GetOrAdd(requestType, type =>
            {
                var handlerType = typeof(DefaultQueryRequestHandler<,,,>).MakeGenericType(typeof(TDbContext), type, typeof(TEntity), typeof(TModel));
                return Activator.CreateInstance(handlerType) ??
                       throw new InvalidOperationException(
                           $"Cannot create an instance of query request handler for {typeof(TEntity)} and {typeof(TModel)}");
            });

        return handler.QueryAsync(request, db, sp, ct);
    }

    public static IAsyncEnumerable<TEntity> QueryAsync<TEntity>(
        IAsyncQueryRequest<TEntity> request,
        TDbContext db,
        IServiceProvider sp,
        CancellationToken ct = default)
        where TEntity : class
    {
        var requestType = request.GetType();
        var handler = (AsyncQueryRequestHandler<TDbContext, TEntity>)handlers.GetOrAdd(requestType, type =>
            {
                var handlerType = typeof(DefaultAsyncQueryRequestHandler<,,>).MakeGenericType(typeof(TDbContext), type, typeof(TEntity));
                return Activator.CreateInstance(handlerType) ??
                       throw new InvalidOperationException(
                           $"Cannot create an instance of async query request handler for {typeof(TEntity)}");
            });
        return handler.QueryAsync(request, db, sp, ct);
    }

    public static IAsyncEnumerable<TModel> QueryAsync<TEntity, TModel>(
        IAsyncQueryRequest<TEntity, TModel> request,
        TDbContext db,
        IServiceProvider sp,
        CancellationToken ct = default)
        where TEntity : class
    {
        var requestType = request.GetType();
        var handler = (AsyncQueryRequestHandler<TDbContext, TEntity, TModel>)handlers.GetOrAdd(requestType, type =>
            {
                var handlerType = typeof(DefaultAsyncQueryRequestHandler<,,,>).MakeGenericType(typeof(TDbContext), type, typeof(TEntity), typeof(TModel));
                return Activator.CreateInstance(handlerType) ??
                       throw new InvalidOperationException(
                           $"Cannot create an instance of async query request handler for {typeof(TEntity)} and {typeof(TModel)}");
            });
        return handler.QueryAsync(request, db, sp, ct);
    }
}

internal abstract class QueryRequestHandler<TDbContext, TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    public abstract Task<IEnumerable<TEntity>> QueryAsync(
        IQueryRequest<TEntity> request, TDbContext db, IServiceProvider sp, CancellationToken ct = default);
}

internal sealed class DefaultQueryRequestHandler<TDbContext, TRequest, TEntity> : QueryRequestHandler<TDbContext, TEntity>
    where TDbContext : DbContext
    where TRequest : IQueryRequest<TEntity>
    where TEntity : class
{
    public override Task<IEnumerable<TEntity>> QueryAsync(
        IQueryRequest<TEntity> request, TDbContext db, IServiceProvider sp, CancellationToken ct = default)
    {
        var handler = sp.GetService<IQueryHandler<TDbContext, TRequest, TEntity>>()
            ?? throw new InvalidOperationException(
                $"The query handler for query request {typeof(TRequest)} was not configured for the work context");

        return handler.HandleAsync((TRequest)request, db, ct);
    }
}

internal abstract class QueryRequestHandler<TDbContext, TEntity, TModel>
    where TDbContext : DbContext
    where TEntity : class
{
    public abstract Task<IEnumerable<TModel>> QueryAsync(
        IQueryRequest<TEntity, TModel> request, TDbContext db, IServiceProvider sp, CancellationToken ct = default);
}

internal sealed class DefaultQueryRequestHandler<TDbContext, TRequest, TEntity, TModel> : QueryRequestHandler<TDbContext, TEntity, TModel>
    where TDbContext : DbContext
    where TRequest : IQueryRequest<TEntity, TModel>
    where TEntity : class
{
    public override Task<IEnumerable<TModel>> QueryAsync(
        IQueryRequest<TEntity, TModel> request, TDbContext db, IServiceProvider sp, CancellationToken ct = default)
    {
        var handler = sp.GetService<IQueryHandler<TDbContext, TRequest, TEntity, TModel>>()
            ?? throw new InvalidOperationException(
                $"The query handler for query request {typeof(TRequest)} was not configured for the work context");

        return handler.HandleAsync((TRequest)request, db, ct);
    }
}

internal abstract class AsyncQueryRequestHandler<TDbContext, TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    public abstract IAsyncEnumerable<TEntity> QueryAsync(
        IAsyncQueryRequest<TEntity> request, TDbContext db, IServiceProvider sp, CancellationToken ct = default);
}

internal sealed class DefaultAsyncQueryRequestHandler<TDbContext, TRequest, TEntity> : AsyncQueryRequestHandler<TDbContext, TEntity>
    where TDbContext : DbContext
    where TRequest : IAsyncQueryRequest<TEntity>
    where TEntity : class
{
    public override IAsyncEnumerable<TEntity> QueryAsync(
        IAsyncQueryRequest<TEntity> request, TDbContext db, IServiceProvider sp, CancellationToken ct = default)
    {
        var handler = sp.GetService<IAsyncQueryHandler<TDbContext, TRequest, TEntity>>()
            ?? throw new InvalidOperationException(
                $"The async query handler for query request {typeof(TRequest)} was not configured for the work context");
        
        return handler.HandleAsync((TRequest)request, db, ct);
    }
}

internal abstract class AsyncQueryRequestHandler<TDbContext, TEntity, TModel>
    where TDbContext : DbContext
    where TEntity : class
{
    public abstract IAsyncEnumerable<TModel> QueryAsync(
        IAsyncQueryRequest<TEntity, TModel> request, TDbContext db, IServiceProvider sp, CancellationToken ct = default);
}

internal sealed class DefaultAsyncQueryRequestHandler<TDbContext, TRequest, TEntity, TModel> : AsyncQueryRequestHandler<TDbContext, TEntity, TModel>
    where TDbContext : DbContext
    where TRequest : IAsyncQueryRequest<TEntity, TModel>
    where TEntity : class
{
    public override IAsyncEnumerable<TModel> QueryAsync(
        IAsyncQueryRequest<TEntity, TModel> request, TDbContext db, IServiceProvider sp, CancellationToken ct = default)
    {
        var handler = sp.GetService<IAsyncQueryHandler<TDbContext, TRequest, TEntity, TModel>>()
            ?? throw new InvalidOperationException(
                $"The async query handler for query request {typeof(TRequest)} was not configured for the work context");

        return handler.HandleAsync((TRequest)request, db, ct);
    }
}