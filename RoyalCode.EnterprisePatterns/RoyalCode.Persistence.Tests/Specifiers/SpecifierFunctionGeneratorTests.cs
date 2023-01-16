using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Filter;
using RoyalCode.Searches.Abstractions;
using System;
using Xunit;

namespace RoyalCode.Persistence.Tests.Specifiers;

public class SpecifierFunctionGeneratorTests
{
    [Fact]
    public void Generate_Must_GenerateTheFilterFunction_For_SimpleModel()
    {
        var generator = new DefaultSpecifierFunctionGenerator();

        var function = generator.Generate<SimpleModel, SimpleFilter>();

        Assert.NotNull(function);
    }

    [Fact]
    public void Generate_Must_GenerateTheFilterFunction_For_NullableProperties()
    {
        var generator = new DefaultSpecifierFunctionGenerator();

        var function = generator.Generate<SimpleModel, NullablePropertiesFilter>();

        Assert.NotNull(function);
    }
    
    [Fact]
    public void Generate_Must_GenerateTheFilter_When_ConfiguredByUnitOfWork()
    {
        // arrange
        ServiceCollection services = new();
        services.AddUnitOfWork<LocalDbContext>()
            .ConfigureDbContext(builder => builder.UseInMemoryDatabase("test"))
            .ConfigureSearches(s => s.Add<SimpleModel>());

        ServiceProvider provider = services.BuildServiceProvider();

        // act
        var search = provider.GetService<ISearch<SimpleModel>>();

        // assert
        Assert.NotNull(search);

        // act
        search!.FilterBy(new SimpleFilter());
        var resultList = search.ToList();

        // assert
        Assert.NotNull(resultList);
    }
}

public class SimpleModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Guid Guid { get; set; }

    public DateTime? Date { get; set; }
}

public class SimpleFilter
{
    public string Name { get; set; }
}

file class NullablePropertiesFilter
{
    public string? Name { get; set; }

    public Guid? Guid { get; set; }
}

file class LocalDbContext : DbContext
{
    public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SimpleModel>();
    }
}