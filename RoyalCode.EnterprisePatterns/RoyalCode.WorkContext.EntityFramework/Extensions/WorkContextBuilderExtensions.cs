using Microsoft.EntityFrameworkCore;
using RoyalCode.Entities;
using System.Reflection;

namespace RoyalCode.WorkContext.EntityFramework.Configurations;

/// <summary>
/// Exntesion methods for <see cref="IWorkContextBuilder{TDbContext}"/>.
/// </summary>
public static class WorkContextBuilderExtensions
{
    /// <summary>
    /// Apply configuration to the work context builder.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    /// <param name="builder">The work context builder.</param>
    /// <param name="configure">The configuration to apply.</param>
    /// <returns>The work context builder.</returns>
    public static IWorkContextBuilder<TDbContext> Configure<TDbContext>(
        this IWorkContextBuilder<TDbContext> builder, IConfigureWorkContext<TDbContext> configure)
        where TDbContext : DbContext
    {
        return configure.ConfigureWorkContext(builder);
    }

    /// <summary>
    /// Configures the work context builder using a new instance of the specified work context configurer.
    /// </summary>
    /// <typeparam name="TDbContext">
    ///     The type of the Entity Framework database context to be used in the work context.
    /// </typeparam>
    /// <typeparam name="TConfigurer">
    ///     The type of the work context configurer to apply. Must implement <see cref="IConfigureWorkContext{TDbContext}"/>
    ///     and have a parameterless constructor.
    /// </typeparam>
    /// <param name="builder">The work context builder to configure.</param>
    /// <returns>The configured work context builder instance.</returns>
    public static IWorkContextBuilder<TDbContext> Configure<TDbContext, TConfigurer>(
        this IWorkContextBuilder<TDbContext> builder)
        where TDbContext : DbContext
        where TConfigurer : IConfigureWorkContext<TDbContext>, new()
    {
        var configure = new TConfigurer();
        return configure.ConfigureWorkContext(builder);
    }

    /// <summary>
    /// Configures the work context builder to register command handlers from the specified assembly.
    /// </summary>
    /// <typeparam name="TDbContext">
    ///     The type of the database context used by the work context builder. Must inherit from <see cref="DbContext"/>.
    /// </typeparam>
    /// <param name="builder">The work context builder to configure with command handlers.</param>
    /// <param name="assembly">The assembly from which command handler types will be discovered and registered.</param>
    /// <returns>The configured work context builder instance, enabling further configuration or building of the work context.</returns>
    public static IWorkContextBuilder<TDbContext> ConfigureCommands<TDbContext>(
        this IWorkContextBuilder<TDbContext> builder, Assembly assembly)
        where TDbContext : DbContext
    {
        return builder.ConfigureCommands(c => c.AddHandlersFromAssembly(assembly));
    }

    /// <summary>
    /// Configures search criteria for all entity types in the specified assembly that implement the <see cref="IEntity"/> interface.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the Entity Framework database context.</typeparam>
    /// <param name="builder">The work context builder to configure with search criteria.</param>
    /// <param name="assembly">The assembly containing entity types to register for search criteria. Only types implementing <see cref="IEntity"/> are
    /// considered.</param>
    /// <returns>The same work context builder instance, configured with search criteria for the discovered entity types.</returns>
    public static IWorkContextBuilder<TDbContext> ConfigureSearches<TDbContext>(
        this IWorkContextBuilder<TDbContext> builder, Assembly assembly)
        where TDbContext : DbContext
    {
        // Configure ICriteria for all entities from the specified assembly
        // read all entity classes from the assembly and register search criteria
        // the entity class must implement the IEntity interface
        var entityTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i == typeof(IEntity)))
            .ToList();

        return builder.ConfigureSearches(s =>
        {
            foreach (var entityType in entityTypes)
            {
                s.Add(entityType);
            }
        });
    }

    /// <summary>
    /// Configures repositories for all entity types in the specified assembly that implement the <see cref="IEntity"/> interface.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context for which repositories are being configured. Must inherit from DbContext.</typeparam>
    /// <param name="builder">The work context builder used to configure repositories for the specified database context.</param>
    /// <param name="assembly">The assembly containing entity types to be registered as repositories. Only types implementing <see cref="IEntity"/> are
    /// considered.</param>
    /// <returns>The same work context builder instance, configured with repositories for all applicable entity types found in
    /// the assembly.</returns>
    public static IWorkContextBuilder<TDbContext> ConfigureRepositories<TDbContext>(
        this IWorkContextBuilder<TDbContext> builder, Assembly assembly)
        where TDbContext : DbContext
    {
        // Configure repositories for all entities from the specified assembly
        // read all entity classes from the assembly and register corresponding repositories
        // the entity class must implement the IEntity interface
        var entityTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i == typeof(IEntity)))
            .ToList();
        
        return builder.ConfigureRepositories(r =>
        {
            foreach (var entityType in entityTypes)
            {
                r.Add(entityType);
            }
        });
    }
}
