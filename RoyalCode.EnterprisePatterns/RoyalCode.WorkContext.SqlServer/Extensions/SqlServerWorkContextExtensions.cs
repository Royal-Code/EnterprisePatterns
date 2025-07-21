using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using RoyalCode.UnitOfWork.EntityFramework;
using RoyalCode.WorkContext.EntityFramework.Configurations;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/> to add SQL Server-based work context.
/// </summary>
public static class SqlServerWorkContextExtensions
{
    /// <summary>
    /// Adds a SQL Server-based work context for the <see cref="DefaultDbContext"/> to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the work context will be added.</param>
    /// <param name="connectionStringName">
    ///     The name of the connection string, used to get the connection string from <see cref="IConfiguration"/>.
    /// </param>
    /// <param name="lifetime">The services lifetime, by default is scoped.</param>
    /// <returns>
    ///     A unit of work builder to configure the <see cref="DbContext"/> and services like repositories and searches.
    /// </returns>
    public static IWorkContextBuilder<DefaultDbContext> AddSqlServerWorkContextDefault(
        this IServiceCollection services,
        string connectionStringName = "Default",
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        return services.AddSqlServerWorkContext<DefaultDbContext>(connectionStringName, lifetime);
    }

    /// <summary>
    /// Adds a SQL Server-based work context related to a <see cref="DbContext"/>.
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
    public static IWorkContextBuilder<TDbContext> AddSqlServerWorkContext<TDbContext>(
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
                options.UseSqlServer(configuration.GetConnectionString(connectionStringName));
            });
    }

    /// <summary>
    /// Allows you to configure specific SqlServer options for the Entity Framework context.
    /// </summary>
    /// <param name="builder">The work context builder to which the SqlServer options will be added.</param>
    /// <param name="configure">The action to configure the SqlServer options.</param>
    /// <returns>The same builder for chained calls.</returns>
    public static IWorkContextBuilder<TDbContext> ConfigureSqlServerOptions<TDbContext>(
        this IWorkContextBuilder<TDbContext> builder,
        Action<SqlServerDbContextOptionsBuilder> configure)
        where TDbContext : DbContext
    {
        return builder.ConfigureOptions((sp, ob) =>
        {
            configure(new SqlServerDbContextOptionsBuilder(ob));
        });
    }

    /// <summary>
    /// Configures the context to use relational database semantics when comparing null
    /// values. By default, Entity Framework will use C# semantics for null values, and
    /// generate SQL to compensate for differences in how the database handles nulls.
    /// </summary>
    /// <param name="builder">The work context builder to which the SqlServer options will be added.</param>
    /// <returns>The same builder for chained calls.</returns>
    public static IWorkContextBuilder<TDbContext> UseRelationalNulls<TDbContext>(
        this IWorkContextBuilder<TDbContext> builder)
        where TDbContext : DbContext
    {
        return builder.ConfigureSqlServerOptions(b => b.UseRelationalNulls());
    }
}
