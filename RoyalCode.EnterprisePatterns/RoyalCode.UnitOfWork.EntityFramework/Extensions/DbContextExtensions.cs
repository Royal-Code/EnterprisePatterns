using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.UnitOfWork.EntityFramework.Configurations;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extension methods for <see cref="DbContext"/>.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Configures a <see cref="ModelBuilder"/> using <see cref="IConfigureModel{TDbContext}"/> services
    /// obtained from the <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
    /// <param name="context">The DbContext.</param>
    /// <param name="modelBuilder">The ModelBuilder of the DbContext.</param>
    public static void ConfigureModelWithServices<TDbContext>(
        this TDbContext context, ModelBuilder modelBuilder)
        where TDbContext : DbContext
    {
        var configurations = context.GetApplicationService<IEnumerable<IConfigureModel<TDbContext>>>();
        if (configurations is null)
            return;

        foreach (var configuration in configurations)
            configuration.Configure(modelBuilder);
    }

    /// <summary>
    /// Configures a <see cref="ModelConfigurationBuilder"/> using 
    /// <see cref="IConfigureConventions{TDbContext}"/> services obtained from the <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
    /// <param name="context">The DbContext.</param>
    /// <param name="configurationBuilder">The configuration builder of the DbContext.</param>
    public static void ConfigureConventionsWithServices<TDbContext>(
        this TDbContext context, ModelConfigurationBuilder configurationBuilder)
        where TDbContext : DbContext
    {
        var configurations = context.GetApplicationService<IEnumerable<IConfigureConventions<TDbContext>>>();
        if (configurations is null)
            return;

        foreach (var configuration in configurations)
            configuration.Configure(configurationBuilder);
    }

    /// <summary>
    /// Gets a service registered in the application's service provider associated with the <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TService">The type of service to obtain.</typeparam>
    /// <param name="accessor">Access to the service provider of the DbContext.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If there is no service of type <typeparamref name="TService"/> registered
    ///     in the application's service provider associated with the <see cref="DbContext"/>,
    ///     or when the application's service provider is not configured for the <see cref="DbContext"/>.
    /// </exception>
    public static TService GetApplicationService<TService>(this IInfrastructure<IServiceProvider> accessor)
        where TService : class
    {
        var sp = accessor.Instance;

        return (sp.GetService<IDbContextOptions>()
                ?.Extensions.OfType<CoreOptionsExtension>().FirstOrDefault()
                ?.ApplicationServiceProvider
                ?.GetRequiredService<TService>()) 
                ?? throw new InvalidOperationException(
                    $"No service of type '{typeof(TService).FullName}' is registered in the DbContext.");
    }
}