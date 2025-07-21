using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using RoyalCode.UnitOfWork.EntityFramework;
using RoyalCode.WorkContext.EntityFramework.Configurations;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/> to add Sqlite-based work context.
/// </summary>
public static class SqliteWorkContextExtensions
{
    /// <summary>
    /// Adds a Sqlite-based work context related to a default <see cref="DbContext"/>,
    /// and configure the <see cref="DbContext"/> with services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionStringName">
    ///     The name of the connection string, used to get the connection string from <see cref="IConfiguration"/>.
    /// </param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IWorkContextBuilder<DefaultDbContext> AddSqliteWorkContextDefault(
        this IServiceCollection services,
        string connectionStringName = "Default",
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddSqliteWorkContext<DefaultDbContext>(connectionStringName, lifetime);
    }

    /// <summary>
    /// Adds a Sqlite-based work context related to a <see cref="DbContext"/>.
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
    public static IWorkContextBuilder<TDbContext> AddSqliteWorkContext<TDbContext>(
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
                options.UseSqlite(configuration.GetConnectionString(connectionStringName));
            });
    }

    /// <summary>
    /// Adds a Sqlite-based work context related to a <see cref="DbContext"/> 
    /// and creates an in-memory database connection to configure the <see cref="DbContext"/> with services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureConnection">An action to configure the connection, optional.</param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IWorkContextBuilder<DefaultDbContext> AddSqliteInMemoryWorkContextDefault(
        this IServiceCollection services,
        Action<SqliteConnection, IServiceProvider>? configureConnection = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddSqliteInMemoryWorkContext<DefaultDbContext>(configureConnection, lifetime);
    }

    /// <summary>
    /// Adds a Sqlite-based work context related to a <see cref="DbContext"/> 
    /// and creates an in-memory database connection to configure the <see cref="DbContext"/> with services.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext used in the work context.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configureConnection">An action to configure the connection, optional.</param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IWorkContextBuilder<TDbContext> AddSqliteInMemoryWorkContext<TDbContext>(
        this IServiceCollection services,
        Action<SqliteConnection, IServiceProvider>? configureConnection = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TDbContext : DbContext
    {
        // creates a single SQLite connection in memory
        var connection = new SqliteConnection("DataSource=:memory:");
        services.AddSingleton(connection);

        return services.AddWorkContext<TDbContext>(lifetime)
            .ConfigureDbContextWithService()
            .ConfigureOptions((provider, options) =>
            {
                var conn = provider.GetRequiredService<SqliteConnection>();

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                    if (configureConnection is not null)
                        configureConnection(connection, provider);
                }

                options.UseSqlite(conn);
            });
    }

    /// <summary>
    /// Allows you to configure specific Sqlite options for the Entity Framework context.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext used in the work context.</typeparam>
    /// <param name="builder">The work context builder to which the Sqlite options will be added.</param>
    /// <param name="configure">The action to configure the Sqlite options.</param>
    /// <returns>The same builder for chained calls.</returns>
    public static IWorkContextBuilder<TDbContext> ConfigureSqliteOptions<TDbContext>(
        this IWorkContextBuilder<TDbContext> builder,
        Action<SqliteDbContextOptionsBuilder> configure)
        where TDbContext : DbContext
    {
        return builder.ConfigureOptions((sp, ob) =>
        {
            configure(new SqliteDbContextOptionsBuilder(ob));
        });
    }
}
