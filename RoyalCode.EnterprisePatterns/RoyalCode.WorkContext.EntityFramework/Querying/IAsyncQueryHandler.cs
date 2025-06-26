using Microsoft.EntityFrameworkCore;

namespace RoyalCode.WorkContext.Abstractions.Querying;

/// <summary>
/// Handles asynchronous query requests for entities of type <typeparamref name="TEntity"/> 
/// using a database context of type <typeparamref name="TDbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TRequest">The type of the query request.</typeparam>
/// <typeparam name="TEntity">The type of the entity to query.</typeparam>
public interface IAsyncQueryHandler<TDbContext, TRequest, out TEntity>
    where TDbContext : DbContext
    where TRequest : IAsyncQueryRequest<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Handles the asynchronous query request and returns the result as an asynchronous stream.
    /// </summary>
    /// <param name="request">The query request to be handled.</param>
    /// <param name="db">The database context.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>An asynchronous stream of entities resulting from the query.</returns>
    IAsyncEnumerable<TEntity> HandleAsync(TRequest request, TDbContext db, CancellationToken ct = default);
}

/// <summary>
/// Handles asynchronous query requests for entities of type <typeparamref name="TEntity"/> 
/// returning models of type <typeparamref name="TModel"/> 
/// using a database context of type <typeparamref name="TDbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TRequest">The type of the query request.</typeparam>
/// <typeparam name="TEntity">The type of the entity to query.</typeparam>
/// <typeparam name="TModel">The type of the model to return.</typeparam>
public interface IAsyncQueryHandler<TDbContext, TRequest, TEntity, out TModel>
    where TDbContext : DbContext
    where TRequest : IAsyncQueryRequest<TEntity, TModel>
    where TEntity : class
{
    /// <summary>
    /// Handles the asynchronous query request and returns the result as an asynchronous stream.
    /// </summary>
    /// <param name="request">The query request to be handled.</param>
    /// <param name="db">The database context.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>An asynchronous stream of models resulting from the query.</returns>
    IAsyncEnumerable<TModel> HandleAsync(TRequest request, TDbContext db, CancellationToken ct = default);
}