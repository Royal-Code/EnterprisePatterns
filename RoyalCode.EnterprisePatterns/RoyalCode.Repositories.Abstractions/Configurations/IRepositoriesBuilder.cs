using RoyalCode.OperationHint.Abstractions;

namespace RoyalCode.Repositories.Configurations;

/// <summary>
///    Configure repositories for the unit of work.
/// </summary>
public interface IRepositoriesBuilder
{
    /// <summary>
    /// Add a repository for an entity as a service, related to builder used by the unit of work.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>The same instance.</returns>
    IRepositoriesBuilder Add<TEntity>()
        where TEntity : class;

    /// <summary>
    /// Add a repository for an entity type as a service, related to builder used by the unit of work.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The same instance.</returns>
    IRepositoriesBuilder Add(Type entityType);

    /// <summary>
    /// Allows the configuration of hints for repository operations.
    /// </summary>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The same instance.</returns>
    IRepositoriesBuilder ConfigureOperationHints(Action<IHintHandlerRegistry> configure);
}
