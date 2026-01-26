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
}