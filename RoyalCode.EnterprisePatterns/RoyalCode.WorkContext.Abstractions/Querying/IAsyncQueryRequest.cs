namespace RoyalCode.WorkContext.Abstractions.Querying;

#pragma warning disable S2326 // TEntity and TModel should be used to specify a type

/// <summary>
/// Represents an asynchronous query request for entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to query.</typeparam>
public interface IAsyncQueryRequest<TEntity> { }

/// <summary>
/// Represents an asynchronous query request for entities of type <typeparamref name="TEntity"/> returning models of type <typeparamref name="TModel"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to query.</typeparam>
/// <typeparam name="TModel">The type of the model to return.</typeparam>
public interface IAsyncQueryRequest<TEntity, TModel> { }