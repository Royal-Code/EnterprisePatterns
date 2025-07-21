using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Repositories.Configurations;
using RoyalCode.Repositories.EntityFramework.Configurations;
using RoyalCode.SmartSearch.EntityFramework.Configurations;
using RoyalCode.SmartSearch.Linq;

namespace RoyalCode.UnitOfWork.EntityFramework.Configurations;

/// <summary>
/// <para>
///     Default implementation of <see cref="IUnitOfWorkBuilder{TDbContext, TBuilder}"/>.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
public sealed class UnitOfWorkBuilder<TDbContext> : IUnitOfWorkBuilder<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Creates a new builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The lifetime that will be used when register services.</param>
    public UnitOfWorkBuilder(
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
    public IUnitOfWorkBuilder<TDbContext> ConfigureRepositories(Action<IRepositoriesBuilder> configureAction)
    {
        var repositoriesBuilder = new RepositoriesBuilder<TDbContext>(Services, Lifetime);
        configureAction(repositoriesBuilder);
        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureSearches(Action<ISearchConfigurations> configureAction)
    {
        var searchConfigurations = new SearchConfigurations<TDbContext>(Services);
        configureAction(searchConfigurations);
        return this;
    }
}