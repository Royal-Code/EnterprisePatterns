namespace RoyalCode.WorkContext.Querying;

/// <summary>
/// Represents a dispatcher for query requests.
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches a query request to the appropriate query handler and returns the result.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to query.</typeparam>
    /// <param name="request">The query request to be dispatched.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the query.</returns>
    Task<IEnumerable<TEntity>> QueryAsync<TEntity>(IQueryRequest<TEntity> request, CancellationToken ct = default)
        where TEntity : class;

    /// <summary>
    /// Dispatches a query request to the appropriate query handler and returns the result.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to query.</typeparam>
    /// <typeparam name="TModel">The type of the model to return.</typeparam>
    /// <param name="request">The query request to be dispatched.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the query.</returns>
    Task<IEnumerable<TModel>> QueryAsync<TEntity, TModel>(IQueryRequest<TEntity, TModel> request, CancellationToken ct = default)
         where TEntity : class;

    /// <summary>
    /// Dispatches an asynchronous query request to the appropriate query handler and returns the result as an asynchronous stream.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to query.</typeparam>
    /// <param name="request">The asynchronous query request to be dispatched.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>An asynchronous stream that represents the result of the query.</returns>
    IAsyncEnumerable<TEntity> QueryAsync<TEntity>(IAsyncQueryRequest<TEntity> request, CancellationToken ct = default)
         where TEntity : class;

    /// <summary>
    /// Dispatches an asynchronous query request to the appropriate query handler and returns the result as an asynchronous stream.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to query.</typeparam>
    /// <typeparam name="TModel">The type of the model to return.</typeparam>
    /// <param name="request">The asynchronous query request to be dispatched.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>An asynchronous stream that represents the result of the query.</returns>
    IAsyncEnumerable<TModel> QueryAsync<TEntity, TModel>(IAsyncQueryRequest<TEntity, TModel> request, CancellationToken ct = default)
         where TEntity : class;
}