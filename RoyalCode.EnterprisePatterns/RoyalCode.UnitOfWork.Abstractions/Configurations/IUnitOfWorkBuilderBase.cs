using Microsoft.Extensions.DependencyInjection;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.Repositories.Configurations;
using RoyalCode.SmartSearch.Linq;

namespace RoyalCode.UnitOfWork.Configurations;

/// <summary>
/// <para>
///     Interface to configure one unit of work.
/// </para>
/// <para>
///     It is designed to work with dependency injection.
/// </para>
/// </summary>
/// <typeparam name="TBuilder"> The type of the builder for the unit of work.</typeparam>
public interface IUnitOfWorkBuilderBase<out TBuilder>
    where TBuilder : IUnitOfWorkBuilderBase<TBuilder>
{
    /// <summary>
    /// The service collection.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// The <see cref="ServiceLifetime"/> used for register the services.
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// Configure the repositories for the unit of work.
    /// </summary>
    /// <param name="configureAction">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureRepositories(Action<IRepositoriesBuilder> configureAction);

    /// <summary>
    /// Configure the searches for the unit of work.
    /// </summary>
    /// <param name="configureAction">Action to configure.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureSearches(Action<ISearchConfigurations> configureAction);

    /// <summary>
    /// Allows the configuration of hints for repository operations.
    /// </summary>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The same instance.</returns>
    public TBuilder ConfigureOperationHints(Action<IHintHandlerRegistry> configure)
        => ConfigureRepositories(builder => builder.ConfigureOperationHints(configure));
}
