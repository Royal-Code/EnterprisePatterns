using Microsoft.EntityFrameworkCore;
using RoyalCode.UnitOfWork.EntityFramework.Configurations;
using RoyalCode.WorkContext.Configurations;
using RoyalCode.WorkContext.EntityFramework.Commands.Configurations;
using RoyalCode.WorkContext.EntityFramework.Commands.Configurations.Internals;
using RoyalCode.WorkContext.EntityFramework.Querying.Configurations;
using RoyalCode.WorkContext.EntityFramework.Querying.Configurations.Internals;

namespace RoyalCode.WorkContext.EntityFramework.Configurations;

/// <summary>
/// Defines a builder for creating and configuring a work context associated with a specific database context type.
/// </summary>
/// <typeparam name="TDbContext">
///     The type of the database context that the work context will be associated with. Must derive from 
///     <see cref="DbContext"/>.
/// </typeparam>
public interface IWorkContextBuilder<TDbContext> : IWorkContextBuilder<TDbContext, IWorkContextBuilder<TDbContext>>
    where TDbContext : DbContext
{ }

/// <summary>
/// Defines a builder interface for configuring a work context, including query and command services,  for a specified
/// database context type.
/// </summary>
/// <remarks>
///     This interface extends <see cref="IWorkContextBuilderBase{TBuilder}"/> and 
///     <see cref="IUnitOfWorkBuilder{TDbContext, TBuilder}"/>, 
///     providing additional methods for configuring query and command services specific to the work context.
/// </remarks>
/// <typeparam name="TDbContext">
///     The type of the database context that the work context operates on. Must derive from <see cref="DbContext"/>.
/// </typeparam>
/// <typeparam name="TBuilder">
///     The type of the builder implementing this interface, allowing for fluent method chaining.
/// </typeparam>
public interface IWorkContextBuilder<TDbContext, TBuilder> : IWorkContextBuilderBase<TBuilder>, IUnitOfWorkBuilder<TDbContext, TBuilder>
    where TDbContext : DbContext
    where TBuilder : IWorkContextBuilder<TDbContext, TBuilder>
{
    /// <summary>
    /// Configures query-related services for the specified unit of work builder.
    /// </summary>
    /// <param name="configureAction">An action that applies query configurations using the provided <see cref="IQueryConfigurerBase{TDbContext}"/>.</param>
    /// <returns>The same instance, allowing for method chaining.</returns>
    public TBuilder ConfigureQueries(Action<IQueryConfigurer<TDbContext>> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);
        var configurations = new QueryConfigurer<TDbContext>(Services);
        configureAction(configurations);
        return (TBuilder)this;
    }

    /// <summary>
    /// Configures command-related services for the specified unit of work builder.
    /// </summary>
    /// <param name="configureAction">An action that applies command configurations using the provided <see cref="ICommandsConfigurer"/>.</param>
    /// <returns>The same instance, allowing for method chaining.</returns>
    public TBuilder ConfigureCommands(Action<ICommandsConfigurer> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);
        var configurations = new CommandsConfigurer<TDbContext>(Services);
        configureAction(configurations);
        return (TBuilder)this;
    }
}
