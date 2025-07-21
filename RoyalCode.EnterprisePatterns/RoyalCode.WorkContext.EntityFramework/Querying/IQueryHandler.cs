using Microsoft.EntityFrameworkCore;
using RoyalCode.WorkContext.Querying;

namespace RoyalCode.WorkContext.EntityFramework.Querying;

/// <summary>
/// Handles query requests for entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TRequest">The type of the query request.</typeparam>
/// <typeparam name="TEntity">The type of the entity to query.</typeparam>
public interface IQueryHandler<TDbContext, TRequest, TEntity>
    where TDbContext : DbContext
    where TRequest : IQueryRequest<TEntity>
    where TEntity: class
{
    /// <summary>
    /// Handles the query request and returns the result as a collection of entities.
    /// </summary>
    /// <param name="request">The query request to be handled.</param>
    /// <param name="db">The database context.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the query.</returns>
    Task<IEnumerable<TEntity>> HandleAsync(TRequest request, TDbContext db, CancellationToken ct = default);
}

/// <summary>
/// Handles query requests for entities of type <typeparamref name="TEntity"/> 
/// returning models of type <typeparamref name="TModel"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TRequest">The type of the query request.</typeparam>
/// <typeparam name="TEntity">The type of the entity to query.</typeparam>
/// <typeparam name="TModel">The type of the model to return.</typeparam>
public interface IQueryHandler<TDbContext, TRequest, TEntity, TModel>
    where TDbContext : DbContext
    where TRequest : IQueryRequest<TEntity, TModel>
    where TEntity : class
{
    /// <summary>
    /// Handles the query request and returns the result as a collection of models.
    /// </summary>
    /// <param name="request">The query request to be handled.</param>
    /// <param name="db">The database context.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the query.</returns>
    Task<IEnumerable<TModel>> HandleAsync(TRequest request, TDbContext db, CancellationToken ct = default);
}