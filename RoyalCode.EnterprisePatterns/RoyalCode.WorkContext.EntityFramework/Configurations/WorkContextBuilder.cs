using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Repositories.Configurations;
using RoyalCode.Repositories.EntityFramework.Configurations;
using RoyalCode.SmartSearch.EntityFramework.Configurations;
using RoyalCode.SmartSearch.Linq;

namespace RoyalCode.WorkContext.EntityFramework.Configurations;

/// <summary>
/// Default implementation of <see cref="IWorkContextBuilder{TDbContext}"/>.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public sealed class WorkContextBuilder<TDbContext> : IWorkContextBuilder<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Creates a new builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The lifetime that will be used when register services.</param>
    public WorkContextBuilder(
        IServiceCollection services,
        ServiceLifetime lifetime)
    {
        Services = services;
        Lifetime = lifetime;
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public ServiceLifetime Lifetime { get; }

    /// <inheritdoc />
    public IWorkContextBuilder<TDbContext> ConfigureRepositories(Action<IRepositoriesBuilder> configureAction)
    {
        var repositoriesBuilder = new RepositoriesBuilder<TDbContext>(Services, Lifetime);
        configureAction(repositoriesBuilder);
        return this;
    }

    /// <inheritdoc />
    public IWorkContextBuilder<TDbContext> ConfigureSearches(Action<ISearchConfigurations> configureAction)
    {
        var searchConfigurations = new SearchConfigurations<TDbContext>(Services);
        configureAction(searchConfigurations);
        return this;
    }
}
