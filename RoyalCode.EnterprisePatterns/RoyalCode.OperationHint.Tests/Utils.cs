using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoyalCode.Persistence.EntityFramework.UnitOfWork;
using System.Data.Common;

namespace RoyalCode.OperationHint.Tests;

internal static class Utils
{
    public static void AddWorkContext(IServiceCollection services,
        Action<IUnitOfWorkBuilder<LocalDbContext>>? configureBuilder = null)
    {
        DbConnection conn = new SqliteConnection("Data Source=:memory:");
        conn.Open();
        services.TryAddSingleton(conn);
        var builder = services.AddWorkContext<LocalDbContext>()
            .ConfigureDbContextPool(builder => builder.UseSqlite(conn))
            .ConfigureRepositories(c =>
            {
                c.Add<SimpleEntity>();
                c.Add<ComplexEntity>();
            });

        configureBuilder?.Invoke(builder);
    }
}
