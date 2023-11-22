using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.Tests.Entities;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.Searches.Abstractions;
using RoyalCode.UnitOfWork.Abstractions;
using RoyalCode.WorkContext.Abstractions;
using Xunit;

namespace RoyalCode.Persistence.Tests.WorkContext;

public class WorkContextBuilderTests
{
    [Fact]
    public void ConfigureWorkContextAndRepositoryAndSearch()
    {
        ServiceCollection services = new();

        services.AddWorkContext<WorkContextBuilderDbContext>()
            .ConfigureDbContextPool(builder => builder.UseSqlite("DataSource=:memory:"))
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

        var searchable = sp.GetService<ISearchable>();
        Assert.NotNull(searchable);
        Assert.Same(context, searchable);
        
        var repo = sp.GetService<IRepository<Person>>();
        var contextRepo = context!.Repository<Person>();
        Assert.NotNull(repo);
        Assert.NotNull(contextRepo);
        Assert.Same(repo, contextRepo);

        var search = sp.GetService<ISearch<Person>>();
        Assert.NotNull(search);

        var contextSearch = context!.CreateSearch<Person>();
        Assert.NotNull(contextSearch);

        var allPersons = sp.GetService<IAllEntities<Person>>();
        Assert.NotNull(allPersons);

        var contextAllPersons = context!.All<Person>();
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

    }
}

#endregion