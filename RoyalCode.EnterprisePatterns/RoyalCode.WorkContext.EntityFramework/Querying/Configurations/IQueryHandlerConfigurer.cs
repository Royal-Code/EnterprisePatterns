using Microsoft.EntityFrameworkCore;
using RoyalCode.WorkContext.Querying;

namespace RoyalCode.WorkContext.EntityFramework.Querying.Configurations;

/// <summary>
/// Configures query handlers for a specific <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
public interface IQueryHandlerConfigurer<out TDbContext> : IQueryHandlerConfigurer<TDbContext, IQueryHandlerConfigurer<TDbContext>>
    where TDbContext : DbContext
{ }

/// <summary>
/// Provides methods to configure query handlers for a specific <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
/// <typeparam name="TConfigure">The type of the configurer.</typeparam>
public interface IQueryHandlerConfigurer<out TDbContext, out TConfigure>
    where TDbContext : DbContext
    where TConfigure : IQueryHandlerConfigurer<TDbContext, TConfigure>
{
    /// <summary>
    /// Registers a handler for queries returning entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TRequest">The request type implementing <see cref="IQueryRequest{TEntity}"/>.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="handler">The handler function.</param>
    /// <returns>The configurer instance.</returns>
    TConfigure Handle<TRequest, TEntity>(
        Func<TRequest, TDbContext, CancellationToken, Task<IEnumerable<TEntity>>> handler)
        where TRequest : IQueryRequest<TEntity>
        where TEntity : class;

    /// <summary>
    /// Registers a handler for queries returning models of type <typeparamref name="TModel"/> from entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TRequest"> The request type implementing <see cref="IQueryRequest{TEntity, TModel}"/>.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="handler">The handler function.</param>
    /// <returns>The configurer instance.</returns>
    TConfigure Handle<TRequest, TEntity, TModel>(
        Func<TRequest, TDbContext, CancellationToken, Task<IEnumerable<TModel>>> handler)
        where TRequest : IQueryRequest<TEntity, TModel>
        where TEntity : class;

    /// <summary>
    /// Registers an asynchronous handler for queries returning entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TRequest"> The request type implementing <see cref="IAsyncQueryRequest{TEntity}"/>.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="handler">The asynchronous handler function.</param>
    /// <returns>The configurer instance.</returns>
    TConfigure AsyncHandle<TRequest, TEntity>(
        Func<TRequest, TDbContext, CancellationToken, IAsyncEnumerable<TEntity>> handler)
        where TRequest : IAsyncQueryRequest<TEntity>
        where TEntity : class;

    /// <summary>
    /// Registers an asynchronous handler for queries returning models of type <typeparamref name="TModel"/> from entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TRequest"> The request type implementing <see cref="IAsyncQueryRequest{TEntity, TModel}"/>.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="handler">The asynchronous handler function.</param>
    /// <returns>The configurer instance.</returns>
    TConfigure AsyncHandle<TRequest, TEntity, TModel>(
        Func<TRequest, TDbContext, CancellationToken, IAsyncEnumerable<TModel>> handler)
        where TRequest : IAsyncQueryRequest<TEntity, TModel>
        where TEntity : class;
}