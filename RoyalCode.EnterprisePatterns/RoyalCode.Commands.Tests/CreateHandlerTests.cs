using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoyalCode.Commands.Abstractions.Attributes;
using RoyalCode.Commands.Handlers;
using RoyalCode.Entities;
using System.Data.Common;

namespace RoyalCode.Commands.Tests;

public class CreateHandlerTests
{
    [Fact]
    public void RegisterAndRetriveService()
    {
        // arrange
        var services = new ServiceCollection();
        AddWorkContext(services);

        services.AddCreateCommand<SimpleHandlers>();

        var provider = services.BuildServiceProvider();
        EnsureDatabaseCreated(provider);

        // act
        var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        var handler = sp.GetRequiredService<CreateCommandHandler<SimpleEntity, SimpleDto>>();

        // assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task CreateSimple()
    {
        // arrange
        var services = new ServiceCollection();
        AddWorkContext(services);
        services.AddCreateCommand<SimpleHandlers>();
        var provider = services.BuildServiceProvider();
        EnsureDatabaseCreated(provider);

        // act
        var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        var handler = sp.GetRequiredService<CreateCommandHandler<SimpleEntity, SimpleDto>>();
        var dto = new SimpleDto { Name = "Test" };
        var result = await handler.HandleAsync(dto, default);
        scope.Dispose();
        bool success = result.HasValue(out var entity);

        // assert
        Assert.True(success);
        Assert.NotNull(entity);
        Assert.Equal(dto.Name, entity.Name);

        scope = provider.CreateScope();
        sp = scope.ServiceProvider;
        var db = sp.GetRequiredService<LocalDbContext>();
        var dbEntity = await db.Set<SimpleEntity>().FindAsync(entity.Id);
        Assert.NotNull(dbEntity);
        scope.Dispose();
    }

    private static void AddWorkContext(IServiceCollection services)
    {
        DbConnection conn = new SqliteConnection("Data Source=:memory:");
        conn.Open();
        services.TryAddSingleton(conn);
        services.AddWorkContext<LocalDbContext>()
            .ConfigureDbContextPool(builder => builder.UseSqlite(conn))
            .ConfigureRepositories(c =>
            {
                c.Add<SimpleEntity>();
            });
    }

    private static void EnsureDatabaseCreated(IServiceProvider root)
    {
        using var scope = root.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetService<LocalDbContext>();
        Assert.NotNull(db);

        db.Database.EnsureCreated();
    }

    private class LocalDbContext : TestDbContext
    {
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SimpleEntity>(cf =>
            {
                cf.ToTable("simple", "t");
                cf.HasKey(e => e.Id);
                cf.Property(e => e.Name).HasMaxLength(50);
            });
        }
    }
}

#nullable disable

file class SimpleDto
{
    public string Name { get; set; }
}

public class SimpleEntity : Entity<int>
{
    public SimpleEntity()
    {
        // new randon int
        Id = new Random().Next();
    }

    public string Name { get; set; }
}

#nullable enable

file class SimpleHandlers
{
    [Creation]
    public SimpleEntity CreateSimple(SimpleDto dto)
    {
        return new SimpleEntity { Name = dto.Name };
    }
}