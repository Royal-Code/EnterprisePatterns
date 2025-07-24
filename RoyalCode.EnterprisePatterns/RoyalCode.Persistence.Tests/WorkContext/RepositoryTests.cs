using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.Tests.Entities;
using RoyalCode.Repositories;
using RoyalCode.SmartProblems.Entities;
using RoyalCode.WorkContext;
using Xunit;

namespace RoyalCode.Persistence.Tests.WorkContext;

public class RepositoryTests
{
    private static ServiceCollection CreateServiceCollectionWithWorkContextConfigured()
    {
        ServiceCollection services = new();

        services.AddSqliteInMemoryWorkContextDefault()
            .EnsureDatabaseCreated()
            .ConfigureModel(builder => builder.Entity<Person>().Property("Id").ValueGeneratedOnAdd())
            .ConfigureRepositories(builder => builder.Add<Person>())
            .SeedDatabase(async (db) =>
            {
                db.Add(new Person { Name = "Alice" });
                db.Add(new Person { Name = "Bob" });
                await db.SaveChangesAsync();
            });

        return services;
    }

    [Fact]
    public void GetRepositoryService()
    {
        // arrange
        var services = CreateServiceCollectionWithWorkContextConfigured();
        var sp = services.BuildServiceProvider();

        // act
        var repository = sp.GetService<IRepository<Person>>();

        // assert
        Assert.NotNull(repository);
    }

    [Fact]
    public void Find()
    {
        // arrange
        var services = CreateServiceCollectionWithWorkContextConfigured();
        var sp = services.BuildServiceProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();

        // act
        var person = repository.Find(1);

        // assert
        Assert.NotNull(person);
    }

    [Fact]
    public async Task FindAsync()
    {
        // arrange
        var services = CreateServiceCollectionWithWorkContextConfigured();
        var sp = services.BuildServiceProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();

        // act
        var person = await repository.FindAsync(1);

        // assert
        Assert.NotNull(person);
    }

    [Fact]
    public async Task FindAsync_Id()
    {
        // arrange
        var services = CreateServiceCollectionWithWorkContextConfigured();
        var sp = services.BuildServiceProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();
        Id<Person, int> id = 1;

        // act
        var result = await repository.FindAsync(id);
        var notFound = result.NotFound(out var problem);

        // assert
        Assert.False(notFound);
        Assert.Null(problem);
        Assert.NotNull(result.Entity);
    }

    [Fact]
    public async Task FindAsync_Id_Dto()
    {
        // arrange
        var services = CreateServiceCollectionWithWorkContextConfigured();
        var sp = services.BuildServiceProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();
        Id<Person, int> id = 1;

        // act
        var result = await repository.FindAsync<PersonDto, int>(id);
        var notFound = result.NotFound(out var problem);

        // assert
        Assert.False(notFound);
        Assert.Null(problem);
        Assert.NotNull(result.Entity);
    }

    [Fact]
    public async Task Add()
    {
        // arrange
        var services = CreateServiceCollectionWithWorkContextConfigured();
        var sp = services.BuildServiceProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();
        var context = sp.GetRequiredService<IWorkContext>();

        // act
        var person = new Person { Name = "John Doe" };
        repository.Add(person);
        await context.SaveAsync();

        // assert
        var foundPerson = await repository.FindAsync(person.Id);
        Assert.NotNull(foundPerson);
    }

    [Fact]
    public async Task AddAsync()
    {
        // arrange
        var services = CreateServiceCollectionWithWorkContextConfigured();
        var sp = services.BuildServiceProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();
        var context = sp.GetRequiredService<IWorkContext>();

        // act
        var person = new Person { Name = "John Doe" };
        await repository.AddAsync(person);
        await context.SaveAsync();

        // assert
        var foundPerson = await repository.FindAsync(person.Id);
        Assert.NotNull(foundPerson);
    }
}
