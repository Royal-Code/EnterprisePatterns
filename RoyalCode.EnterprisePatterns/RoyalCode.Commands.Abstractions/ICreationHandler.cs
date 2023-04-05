using RoyalCode.OperationResult;

namespace RoyalCode.Commands.Abstractions;

/// <summary>
/// <para>
///     A handler that is responsible for creating a new entity from a request.
/// </para>
/// </summary>
/// <typeparam name="TRequest">The type of the request that contains the data to create the entity.</typeparam>
/// <typeparam name="TEntity">The type of the entity to be created.</typeparam>
public interface ICreationHandler<in TRequest, out TEntity>
{
    /// <summary>
    /// Create a new entity from the request.
    /// </summary>
    /// <param name="request">The request that contains the data to create the entity.</param>
    /// <returns>The new entity.</returns>
    TEntity Create(TRequest request);
}

/// <summary>
/// <para>
///     A handler that is responsible for creating a new entity from a request.
/// </para>
/// </summary>
/// <typeparam name="TContext">The type of the context used to store other loaded data to be used in the creation of the entity.</typeparam>
/// <typeparam name="TRequest">The type of the request that contains the data to create the entity.</typeparam>
/// <typeparam name="TEntity">The type of the entity to be created.</typeparam>
public interface ICreationHandler<TContext, TRequest, out TEntity>
    where TContext : ICreationContext<TRequest>
{
    /// <summary>
    /// <para>
    ///     Create a new context from the request, loading the data necessary for the creation of the entity.
    /// </para>
    /// </summary>
    /// <param name="request">The request that contains the data to create the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The operation result with the context.</returns>
    Task<IOperationResult<TContext>> CreateContextAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Create a new entity from the context.
    /// </summary>
    /// <param name="context">The context that contains the data necessary for the creation of the entity.</param>
    /// <returns>The new entity.</returns>
    TEntity Create(TContext context);
}

/// <summary>
/// <para>
///     A handler that is responsible for creating a new entity from a request.
/// </para>
/// </summary>
/// <typeparam name="TContext">The type of the context used to store other loaded data to be used in the creation of the entity.</typeparam>
/// <typeparam name="TRequest">The type of the request that contains the data to create the entity.</typeparam>
/// <typeparam name="TRootEntity">The type of the root entity.</typeparam>
/// <typeparam name="TEntity">The type of the entity to be created.</typeparam>
public interface ICreationHandler<TContext, TRequest, TRootEntity, out TEntity>
    where TContext : ICreationContext<TRequest, TRootEntity>
{
    /// <summary>
    /// <para>
    ///     Create a new context from the request, loading the data necessary for the creation of the entity.
    /// </para>
    /// </summary>
    /// <param name="request">The request that contains the data to create the entity.</param>
    /// <param name="rootEntity">The root entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The operation result with the context.</returns>
    Task<IOperationResult<TContext>> CreateContextAsync(
        TRequest request, TRootEntity rootEntity, CancellationToken cancellationToken);

    /// <summary>
    /// Create a new entity from the context.
    /// </summary>
    /// <param name="context">The context that contains the data necessary for the creation of the entity.</param>
    /// <returns>The new entity.</returns>
    TEntity Create(TContext context);
}