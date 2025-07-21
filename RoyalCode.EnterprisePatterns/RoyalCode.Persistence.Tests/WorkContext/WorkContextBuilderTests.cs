using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.Tests.Entities;
using RoyalCode.Repositories;
using RoyalCode.SmartSearch;
using RoyalCode.UnitOfWork;
using RoyalCode.WorkContext;
using Xunit;

namespace RoyalCode.Persistence.Tests.WorkContext;

public class WorkContextBuilderTests
{
    [Fact]
    public void ConfigureWorkContextAndRepositoryAndSearch()
    {
        ServiceCollection services = new();

        services.AddWorkContext<WorkContextBuilderDbContext>()
            .ConfigureDbContextPool((sp, builder) =>
            {
                SqliteConnection conn = new SqliteConnection("DataSource=:memory:");
                conn.Open();
                builder.UseSqlite(conn);
            })
            .ConfigureRepositories(c =>
            {
                c.Add<Person>();
            })
            .ConfigureSearches(c =>
            {
                c.Add<Person>();
            });

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetService<WorkContextBuilderDbContext>();
        Assert.NotNull(db);

        db!.Database.EnsureCreated();

        var context = sp.GetService<IWorkContext>();
        Assert.NotNull(context);

        var uow = sp.GetService<IUnitOfWork>();
        Assert.NotNull(uow);
        Assert.Same(context, uow);

        var entityManager = sp.GetService<IEntityManager>();
        Assert.NotNull(entityManager);
        Assert.Same(context, entityManager);

        var searchable = sp.GetService<ISearchManager>();
        Assert.NotNull(searchable);
        Assert.Same(context, searchable);
        
        var repo = sp.GetService<IRepository<Person>>();
        var contextRepo = context!.Repository<Person>();
        Assert.NotNull(repo);
        Assert.NotNull(contextRepo);

        var criteria = sp.GetService<ICriteria<Person>>();
        Assert.NotNull(criteria);

        var contextCriteria = context!.Criteria<Person>();
        Assert.NotNull(contextCriteria);

        var contextAllPersons = contextCriteria.Collect();
        Assert.NotNull(contextAllPersons);

        scope.Dispose();
    }
}


#region Test classes

class WorkContextBuilderDbContext : DbContext
{
    public WorkContextBuilderDbContext(DbContextOptions<WorkContextBuilderDbContext> options)
        : base(options)
    { 
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Person>().ToTable("Persons");
    }
}

#endregion