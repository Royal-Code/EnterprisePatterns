using Microsoft.EntityFrameworkCore;

namespace RoyalCode.WorkContext.EntityFramework.Querying.Configurations;

/// <summary>
/// Provides configuration methods for configure query handlers for the specific context <see cref="DbContext"/>.
/// </summary>
public interface IQueryHandlerConfigurations
{
    /// <summary>
    /// Configures the specified <paramref name="configurator"/> for the given <typeparamref name="TDbContext"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> to configure.</typeparam>
    /// <param name="configurator">The configurer instance to configure query handlers.</param>
    void Configure<TDbContext>(IQueryHandlerConfigurer<TDbContext> configurator)
        where TDbContext : DbContext;
}
