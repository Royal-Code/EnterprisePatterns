using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.Tests.Entities;
using RoyalCode.WorkContext;
using RoyalCode.WorkContext.Querying;
using Xunit;

namespace RoyalCode.Persistence.Tests.WorkContext;

public class QueryRequestTests
{
    [Fact]
    public async Task QueryRequestHandler_ThrowsIfNotConfigured()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddWorkContext<WorkContextBuilderDbContext>()
            .ConfigureDbContextPool((sp, builder) =>
            {
                SqliteConnection conn = new("DataSource=:memory:");
                conn.Open();
                builder.UseSqlite(conn);
            })
            .ConfigureQueries(c =>
            {
                // Intentionally not adding any query handlers to test the exception
            });

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetService<WorkContextBuilderDbContext>();
        var context = sp.GetService<IWorkContext>();

        Assert.NotNull(db);
        Assert.NotNull(context);

        // Act 
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var queryRequest = new GetPersonsQuery { Name = "John" };
            await context.QueryAsync(queryRequest, default);
        });
    }

    [Fact]
    public async Task QueryRequestHandler_ReturnsResults()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddWorkContext<WorkContextBuilderDbContext>()
            .ConfigureDbContextPool((sp, builder) =>
            {
                SqliteConnection conn = new("DataSource=:memory:");
                conn.Open();
                builder.UseSqlite(conn);
            })
            .ConfigureQueries(c =>
            {
                c.Handle<GetPersonsQuery, Person>(async (request, db, ct) =>
                {
                    return await db.Set<Person>()
                        .Where(p => p.Name == request.Name)
                        .ToListAsync(ct);
                });
            });

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetService<WorkContextBuilderDbContext>();
        var context = sp.GetService<IWorkContext>();

        Assert.NotNull(db);
        Assert.NotNull(context);

        // Seed data
        db.Set<Person>().Add(new Person { Name = "John" });
        db.Set<Person>().Add(new Person { Name = "Jane" });
        await db.SaveChangesAsync();

        // Act
        var queryRequest = new GetPersonsQuery { Name = "John" };
        var results = await context.QueryAsync(queryRequest, default);

        // Assert
        Assert.Single(results);
        Assert.Equal("John", results.First().Name);
    }

    [Fact]
    public async Task QueryRequestHandler_WithMap_ReturnsResults()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddWorkContext<WorkContextBuilderDbContext>()
            .ConfigureDbContextPool((sp, builder) =>
            {
                SqliteConnection conn = new("DataSource=:memory:");
                conn.Open();
                builder.UseSqlite(conn);
            })
            .ConfigureQueries(c =>
            {
                c.Handle<GetPersonsDtoQuery, Person, PersonDto>(async (request, db, ct) =>
                {
                    return await db.Set<Person>()
                        .Where(p => p.Name == request.Name)
                        .Select(p => new PersonDto { Id = p.Id, Name = p.Name })
                        .ToListAsync(ct);
                });
            });

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetService<WorkContextBuilderDbContext>();
        var context = sp.GetService<IWorkContext>();

        Assert.NotNull(db);
        Assert.NotNull(context);

        // Seed data
        db.Set<Person>().Add(new Person { Name = "John" });
        db.Set<Person>().Add(new Person { Name = "Jane" });
        await db.SaveChangesAsync();

        // Act
        var queryRequest = new GetPersonsDtoQuery { Name = "John" };
        var results = await context.QueryAsync(queryRequest, default);

        // Assert
        Assert.Single(results);
        Assert.Equal("John", results.First().Name);
    }

    [Fact]
    public async Task AsyncQueryRequestHandler_ReturnsResults()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddWorkContext<WorkContextBuilderDbContext>()
            .ConfigureDbContextPool((sp, builder) =>
            {
                SqliteConnection conn = new("DataSource=:memory:");
                conn.Open();
                builder.UseSqlite(conn);
            })
            .ConfigureQueries(c =>
            {
                c.AsyncHandle<AsyncGetPersonsQuery, Person>((request, db, ct) =>
                {
                    return db.Set<Person>()
                        .Where(p => p.Name == request.Name)
                        .AsAsyncEnumerable();
                });
            });

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetService<WorkContextBuilderDbContext>();
        var context = sp.GetService<IWorkContext>();

        Assert.NotNull(db);
        Assert.NotNull(context);

        // Seed data
        db.Set<Person>().Add(new Person { Name = "John" });
        db.Set<Person>().Add(new Person { Name = "Jane" });
        await db.SaveChangesAsync();

        // Act
        var queryRequest = new AsyncGetPersonsQuery { Name = "John" };
        var results = context.QueryAsync(queryRequest, default);

        var resultsList = new List<Person>();
        await foreach (var person in results)
        {
            resultsList.Add(person);
        }

        // Assert
        Assert.Single(results);
        Assert.Equal("John", resultsList[0].Name);
    }

    [Fact]
    public async Task AsyncQueryRequestHandler_WithMap_ReturnsResults()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddWorkContext<WorkContextBuilderDbContext>()
            .ConfigureDbContextPool((sp, builder) =>
            {
                SqliteConnection conn = new("DataSource=:memory:");
                conn.Open();
                builder.UseSqlite(conn);
            })
            .ConfigureQueries(c =>
            {
                c.AsyncHandle<AsyncGetPersonsDtoQuery, Person, PersonDto>((request, db, ct) =>
                {
                    return db.Set<Person>()
                        .Where(p => p.Name == request.Name)
                        .Select(p => new PersonDto { Id = p.Id, Name = p.Name })
                        .AsAsyncEnumerable();
                });
            });

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetService<WorkContextBuilderDbContext>();
        var context = sp.GetService<IWorkContext>();

        Assert.NotNull(db);
        Assert.NotNull(context);

        // Seed data
        db.Set<Person>().Add(new Person { Name = "John" });
        db.Set<Person>().Add(new Person { Name = "Jane" });
        await db.SaveChangesAsync();

        // Act
        var queryRequest = new AsyncGetPersonsDtoQuery { Name = "John" };
        var results = context.QueryAsync(queryRequest, default);

        var resultsList = new List<PersonDto>();
        await foreach (var person in results)
        {
            resultsList.Add(person);
        }

        // Assert
        Assert.Single(resultsList);
        Assert.Equal("John", resultsList[0].Name);
    }
}

#region Test classes

class GetPersonsQuery : IQueryRequest<Person>
{
    public string? Name { get; set; }
}

class GetPersonsDtoQuery : IQueryRequest<Person, PersonDto>
{
    public string? Name { get; set; }
}

class AsyncGetPersonsQuery : IAsyncQueryRequest<Person>
{
    public string? Name { get; set; }
}

class AsyncGetPersonsDtoQuery : IAsyncQueryRequest<Person, PersonDto>
{
    public string? Name { get; set; }
}

#endregion