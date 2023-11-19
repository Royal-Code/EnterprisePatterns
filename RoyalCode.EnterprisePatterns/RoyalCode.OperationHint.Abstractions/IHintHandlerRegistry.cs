
namespace RoyalCode.OperationHint.Abstractions;

/// <summary>
/// A registry for hint handlers.
/// </summary>
public interface IHintHandlerRegistry
{
    /// <summary>
    /// Adds a hint handler to the registry.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="THint">The type of the hint.</typeparam>
    /// <param name="handler">The hint handler to add.</param>
    /// <returns>
    ///     The same instance of the registry for chaining.
    /// </returns>
    IHintHandlerRegistry Add<TEntity, THint>(IHintHandler<TEntity, THint> handler)
        where TEntity : class
        where THint : Enum;

    /// <summary>
    /// Get all hint handlers for the given entity type and hint type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="THint">The type of the hint.</typeparam>
    /// <returns>The hint handlers for the given entity type and hint type.</returns>
    IEnumerable<IHintHandler<TEntity, THint>> GetHandlers<TEntity, THint>()
        where TEntity : class
        where THint : Enum;
}
