using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.WorkContext.Querying;

namespace RoyalCode.WorkContext.EntityFramework.Querying.Configurations.Internals;

internal sealed class QueryHandlerConfigurer<TDbContext>(IServiceCollection services) : IQueryHandlerConfigurer<TDbContext>
    where TDbContext : DbContext
{
    public IQueryHandlerConfigurer<TDbContext> Handle<TRequest, TEntity>(
        Func<TRequest, TDbContext, CancellationToken, Task<IEnumerable<TEntity>>> handler)
        where TRequest : IQueryRequest<TEntity>
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(handler);
        var queryHandler = new QueryHandler<TDbContext, TRequest, TEntity>(handler);
        services.AddSingleton<IQueryHandler<TDbContext, TRequest, TEntity>>(queryHandler);
        return this;
    }

    public IQueryHandlerConfigurer<TDbContext> Handle<TRequest, TEntity, TModel>(
        Func<TRequest, TDbContext, CancellationToken, Task<IEnumerable<TModel>>> handler)
        where TRequest : IQueryRequest<TEntity, TModel>
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(handler);
        var queryHandler = new QueryHandler<TDbContext, TRequest, TEntity, TModel>(handler);
        services.AddSingleton<IQueryHandler<TDbContext, TRequest, TEntity, TModel>>(queryHandler);
        return this;
    }

    public IQueryHandlerConfigurer<TDbContext> AsyncHandle<TRequest, TEntity>(
        Func<TRequest, TDbContext, CancellationToken, IAsyncEnumerable<TEntity>> handler)
        where TRequest : IAsyncQueryRequest<TEntity>
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(handler);
        var asyncQueryHandler = new AsyncQueryHandler<TDbContext, TRequest, TEntity>(handler);
        services.AddSingleton<IAsyncQueryHandler<TDbContext, TRequest, TEntity>>(asyncQueryHandler);
        return this;
    }

    public IQueryHandlerConfigurer<TDbContext> AsyncHandle<TRequest, TEntity, TModel>(
        Func<TRequest, TDbContext, CancellationToken, IAsyncEnumerable<TModel>> handler)
        where TRequest : IAsyncQueryRequest<TEntity, TModel>
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(handler);
        var asyncQueryHandler = new AsyncQueryHandler<TDbContext, TRequest, TEntity, TModel>(handler);
        services.AddSingleton<IAsyncQueryHandler<TDbContext, TRequest, TEntity, TModel>>(asyncQueryHandler);
        return this;
    }
}

