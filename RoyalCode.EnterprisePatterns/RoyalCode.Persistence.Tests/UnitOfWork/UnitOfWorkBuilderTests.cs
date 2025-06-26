using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.Tests.Entities;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.UnitOfWork.Abstractions;
using Xunit;

namespace RoyalCode.Persistence.Tests.UnitOfWork;

public class UnitOfWorkBuilderTests
{
    [Fact]
    public void ConfigureUnitOfWorkContextAndRepository()
    {
        ServiceCollection services = new();

        services.AddUnitOfWork<UnitOfWorkBuilderDbContext>()
            .ConfigureDbContextPool(builder => builder.UseSqlite("DataSource=:memory:"))
            .ConfigureRepositories(c =>
            {
                c.Add<Person>();
            });

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetService<UnitOfWorkBuilderDbContext>();
        Assert.NotNull(db);

        db!.Database.EnsureCreated();
        
        var uow = sp.GetService<IUnitOfWork>();
        Assert.NotNull(uow);

        var repo = sp.GetService<IRepository<Person>>();
        Assert.NotNull(repo);
        
        scope.Dispose();
    }
}

#region Test classes

class UnitOfWorkBuilderDbContext : DbContext
{
    public UnitOfWorkBuilderDbContext(DbContextOptions<UnitOfWorkBuilderDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Person>().ToTable("Persons");
    }
}

#endregion