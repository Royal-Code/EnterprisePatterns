using Microsoft.EntityFrameworkCore.Infrastructure;
using RoyalCode.UnitOfWork.EntityFramework.Configurations;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Métodos de extensão para <see cref="DbContext"/>.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Configura um <see cref="ModelBuilder"/> usando serviços de <see cref="IConfigureModel{TDbContext}"/>
    /// obtidos do <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TDbContext">O tipo do DbContext.</typeparam>
    /// <param name="context">O DbContext.</param>
    /// <param name="modelBuilder">O ModelBuilder do DbContext.</param>
    public static void ConfigureModelWithServices<TDbContext>(
        this TDbContext context, ModelBuilder modelBuilder)
        where TDbContext : DbContext
    {
        var configurations = context.GetService<IEnumerable<IConfigureModel<TDbContext>>>();
        if (configurations is null)
            return;

        foreach (var configuration in configurations)
            configuration.Configure(modelBuilder);
    }

    /// <summary>
    /// Configura um <see cref="ModelConfigurationBuilder"/> usando serviços 
    /// de <see cref="IConfigureConventions{TDbContext}"/> obtidos do <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TDbContext">O tipo do DbContext.</typeparam>
    /// <param name="context">O DbContext.</param>
    /// <param name="configurationBuilder">As configurações do DbContext.</param>
    public static void ConfigureConventionsWithServices<TDbContext>(
        this TDbContext context, ModelConfigurationBuilder configurationBuilder)
        where TDbContext : DbContext
    {
        var configurations = context.GetService<IEnumerable<IConfigureConventions<TDbContext>>>();
        if (configurations is null)
            return;

        foreach (var configuration in configurations)
            configuration.Configure(configurationBuilder);
    }
}