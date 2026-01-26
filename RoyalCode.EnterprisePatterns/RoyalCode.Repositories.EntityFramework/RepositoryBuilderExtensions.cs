using Microsoft.EntityFrameworkCore;
using RoyalCode.Repositories.EntityFramework.Configurations;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering repositories in the dependency injection container.
/// </summary>
public static class RepositoryBuilderExtensions
{
    /// <summary>
    /// Registers repositories for the specified <typeparamref name="TDbContext"/> in the service collection.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/> to associate with the repositories.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the repositories to.</param>
    /// <param name="configureAction">An action to configure the repositories using an <see cref="IRepositoriesBuilder{TDbContext}"/>.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> for the registered repositories. Defaults to <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <returns>
    ///     The same <see cref="IServiceCollection"/> instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="configureAction"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddRepositories<TDbContext>(
        this IServiceCollection services,
        Action<IRepositoriesBuilder<TDbContext>> configureAction,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configureAction);
        var repositoryConfigurer = new RepositoriesBuilder<TDbContext>(services, lifetime);
        configureAction(repositoryConfigurer);
        return services;
    }
}
