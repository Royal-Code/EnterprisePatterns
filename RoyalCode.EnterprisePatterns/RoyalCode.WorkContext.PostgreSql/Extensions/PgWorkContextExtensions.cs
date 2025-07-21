
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using RoyalCode.UnitOfWork.EntityFramework;
using RoyalCode.WorkContext.EntityFramework.Configurations;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/> to add PostgreSQL-based work context.
/// </summary>
public static class PgWorkContextExtensions
{
    /// <summary>
    /// Adds a PostgreSQL-based work context for the <see cref="DefaultDbContext"/> to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the work context will be added.</param>
    /// <param name="connectionStringName">
    ///     The name of the connection string, used to get the connection string from <see cref="IConfiguration"/>.
    /// </param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IWorkContextBuilder<DefaultDbContext> AddPostgreWorkContextDefault(
        this IServiceCollection services,
        string connectionStringName = "Default",
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddPostgreWorkContext<DefaultDbContext>(connectionStringName, lifetime);
    }

    /// <summary>
    /// Adds a PostgreSQL-based work context related to a <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext used in the work context.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionStringName">
    ///     The name of the connection string, used to get the connection string from <see cref="IConfiguration"/>.
    /// </param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IWorkContextBuilder<TDbContext> AddPostgreWorkContext<TDbContext>(
        this IServiceCollection services,
        string connectionStringName = "Default",
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        return services.AddWorkContext<TDbContext>(lifetime)
            .ConfigureDbContextWithService()
            .ConfigureOptions((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options.UseNpgsql(configuration.GetConnectionString(connectionStringName));
            });
    }

    /// <summary>
    /// Allows you to configure specific Npgsql options for the Entity Framework context.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext used in the work context.</typeparam>
    /// <param name="builder">The work context builder to which the Npgsql options will be added.</param>
    /// <param name="configure">The action to configure the Npgsql options.</param>
    /// <returns>The same builder for chained calls.</returns>
    public static IWorkContextBuilder<TDbContext> ConfigureNpgsqlOptions<TDbContext>(
        this IWorkContextBuilder<TDbContext> builder, 
        Action<NpgsqlDbContextOptionsBuilder> configure)
        where TDbContext : DbContext
    {
        return builder.ConfigureOptions((sp, ob) =>
        {
            configure(new NpgsqlDbContextOptionsBuilder(ob));
        });
    }
}
