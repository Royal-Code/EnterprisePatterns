using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.Tests.Entities;
using RoyalCode.WorkContext.Abstractions;
using RoyalCode.WorkContext.Abstractions.Commands;
using RoyalCode.SmartProblems;
using Xunit;
using RoyalCode.UnitOfWork.Abstractions;

namespace RoyalCode.Persistence.Tests.WorkContext;

public class CommandRequestTests
{
    [Fact]
    public async Task CommandRequestHandler_ThrowsIfNotConfigured()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddWorkContext<CommandsDbContext>()
            .ConfigureDbContextPool((sp, builder) =>
            {
                SqliteConnection conn = new("DataSource=:memory:");
                conn.Open();
                builder.UseSqlite(conn);
            })
            .ConfigureRepositories(c => c.Add<Person>())
            .ConfigureCommands(c => { /* Não registra handler */ });

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetService<CommandsDbContext>();
        var context = sp.GetService<IWorkContext>();

        Assert.NotNull(db);
        Assert.NotNull(context);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var cmd = new CreatePersonCommand { Name = "John" };
            await context.SendAsync(cmd, default);
        });
    }

    [Fact]
    public async Task CommandRequestHandler_ReturnsSuccess()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddWorkContext<CommandsDbContext>()
            .ConfigureDbContextPool((sp, builder) =>
            {
                SqliteConnection conn = new("DataSource=:memory:");
                conn.Open();
                builder.UseSqlite(conn);
            })
            .ConfigureRepositories(c => c.Add<Person>())
            .ConfigureCommands(c =>
            {
                c.AddHandler<CreatePersonHandler>();
            });

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetService<CommandsDbContext>();
        var context = sp.GetService<IWorkContext>();

        Assert.NotNull(db);
        Assert.NotNull(context);

        // Act
        var cmd = new CreatePersonCommand { Name = "John" };
        var result = await context.SendAsync(cmd, default);
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.False(hasProblems);
        Assert.NotNull(db.Set<Person>().FirstOrDefault(p => p.Name == "John"));
    }

    [Fact]
    public async Task CommandRequestHandler_WithResponse_ReturnsExpectedResult()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddWorkContext<CommandsDbContext>()
            .ConfigureDbContextPool((sp, builder) =>
            {
                SqliteConnection conn = new("DataSource=:memory:");
                conn.Open();
                builder.UseSqlite(conn);
            })
            .ConfigureRepositories(c => c.Add<Person>())
            .ConfigureCommands(c =>
            {
                c.AddHandler<CreatePersonWithResponseHandler>();
            });

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetService<CommandsDbContext>();
        var context = sp.GetService<IWorkContext>();

        Assert.NotNull(db);
        Assert.NotNull(context);

        // Act
        var cmd = new CreatePersonWithResponseCommand { Name = "Jane" };
        var result = await context.SendAsync(cmd, default);
        var hasProblems = result.HasProblemsOrGetValue(out var problems, out var person);

        // Assert
        Assert.False(hasProblems);
        Assert.NotNull(person);
        Assert.Equal("Jane", person.Name);
        Assert.Equal(1, person.Id);
    }

    [Fact]
    public async Task CommandRequestHandler_WithCommandDispatcherService()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddWorkContext<CommandsDbContext>()
            .ConfigureDbContextPool((sp, builder) =>
            {
                SqliteConnection conn = new("DataSource=:memory:");
                conn.Open();
                builder.UseSqlite(conn);
            })
            .ConfigureRepositories(c => c.Add<Person>())
            .ConfigureCommands(c =>
            {
                c.AddHandler<CreatePersonHandler>();
            })
            .AddCommandDispatcher();

        var root = services.BuildServiceProvider();
        var scope = root.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetService<CommandsDbContext>();
        var dispatcher = sp.GetService<ICommandDispatcher>();

        Assert.NotNull(db);
        Assert.NotNull(dispatcher);

        // Act
        var cmd = new CreatePersonCommand { Name = "John" };
        var result = await dispatcher.SendAsync(cmd, default);
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.False(hasProblems);
        Assert.NotNull(db.Set<Person>().FirstOrDefault(p => p.Name == "John"));
    }
}

#region Test classes

class CommandsDbContext : DbContext
{
    public CommandsDbContext(DbContextOptions<CommandsDbContext> options) 
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

class CreatePersonCommand : ICommandRequest
{
    public string Name { get; set; } = null!;
}

class CreatePersonWithResponseCommand : ICommandRequest<Person>
{
    public string Name { get; set; } = null!;
}

class CreatePersonHandler : ICommandHandler<CreatePersonCommand>
{
    public async Task<Result> HandleAsync(CreatePersonCommand request, IWorkContext context, CancellationToken ct = default)
    {
        context.Add(new Person { Name = request.Name });
        return await context.SaveAsync(ct);
    }
}

class CreatePersonWithResponseHandler : ICommandHandler<CreatePersonWithResponseCommand, Person>
{
    public async Task<Result<Person>> HandleAsync(
        CreatePersonWithResponseCommand request,
        IWorkContext context,
        CancellationToken ct = default)
    {
        var person = new Person { Name = request.Name };
        context.Add(person);
        return await context.SaveAsync(ct).MapAsync(person);
    }
}

#endregion
