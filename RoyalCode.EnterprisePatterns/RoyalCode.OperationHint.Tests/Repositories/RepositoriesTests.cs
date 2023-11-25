using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.OperationHint.Abstractions;
using RoyalCode.Persistence.EntityFramework.UnitOfWork;
using RoyalCode.Repositories.Abstractions;

namespace RoyalCode.OperationHint.Tests.Repositories;

/// <summary>
/// Teste EF Core implementation of Operation Hint using repositories.
/// </summary>
public class RepositoriesTests
{
    private static ServiceCollection Create(Action<IUnitOfWorkBuilder<LocalDbContext>>? configureBuilder = null)
    {
        var services = new ServiceCollection();
        Utils.AddWorkContext(services, builder =>
        {
            builder.ConfigureOperationHints(regitry =>
            {
                regitry.AddIncludesHandler<ComplexEntity, TestHints>((hint, includes) =>
                {
                    switch (hint)
                    {
                        case TestHints.TestSingleRelation:
                            includes.IncludeReference(e => e.SingleRelation);
                            break;
                        case TestHints.TestMultipleRelation:
                            includes.IncludeCollection(e => e.MultipleRelation);
                            break;
                        case TestHints.TestAllRelations:
                            includes
                                .IncludeReference(e => e.SingleRelation)
                                .IncludeCollection(e => e.MultipleRelation);
                            break;
                    }
                });
            });

            configureBuilder?.Invoke(builder);
        });

        return services;
    }

    private static void InitializeDatabase(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
        
        context.Database.EnsureCreated();

        context.ComplexEntities.Add(new ComplexEntity
        {
            Name = "ComplexEntity",
            SingleRelation = new SimpleEntity
            {
                Name = "SingleRelation"
            },
            MultipleRelation = new List<SimpleEntity>
            {
                new() {
                    Name = "MultipleRelation1"
                },
                new() {
                    Name = "MultipleRelation2"
                }
            }
        });

        context.SaveChanges();
    }

    private static int FirstComplex(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
        return context.ComplexEntities.First().Id;
    }

    [Fact]
    public void Must_Not_Includes_When_NoneHintAdded()
    {
        // Arrange
        var services = Create();
        var provider = services.BuildServiceProvider();
        InitializeDatabase(provider);
        var id = FirstComplex(provider);

        // Act
        using var scope = provider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<ComplexEntity>>();
        var entity = repository.Find(id);

        // Assert
        entity.Should().NotBeNull();
        entity!.SingleRelation.Should().BeNull();
        entity.MultipleRelation.Should().BeNull();
    }

    [Fact]
    public void Must_Includes_SingleRelation_When_TestSingleRelationHintAdded()
    {
        // Arrange
        var services = Create();
        var provider = services.BuildServiceProvider();
        InitializeDatabase(provider);
        var id = FirstComplex(provider);

        // Act
        using var scope = provider.CreateScope();

        var container = scope.ServiceProvider.GetRequiredService<IHintsContainer>();
        container.AddHint(TestHints.TestSingleRelation);

        var repository = scope.ServiceProvider.GetRequiredService<IRepository<ComplexEntity>>();
        var entity = repository.Find(id);

        // Assert
        entity.Should().NotBeNull();
        entity!.SingleRelation.Should().NotBeNull();
        entity.MultipleRelation.Should().BeNull();
    }

    [Fact]
    public void Must_Includes_MultipleRelation_When_TestMultipleRelationHintAdded()
    {
        // Arrange
        var services = Create();
        var provider = services.BuildServiceProvider();
        InitializeDatabase(provider);
        var id = FirstComplex(provider);

        // Act
        using var scope = provider.CreateScope();

        var container = scope.ServiceProvider.GetRequiredService<IHintsContainer>();
        container.AddHint(TestHints.TestMultipleRelation);

        var repository = scope.ServiceProvider.GetRequiredService<IRepository<ComplexEntity>>();
        var entity = repository.Find(id);

        // Assert
        entity.Should().NotBeNull();
        entity!.SingleRelation.Should().BeNull();
        entity.MultipleRelation.Should().NotBeNull();
    }

    [Fact]
    public void Must_Includes_AllRelations_When_TestAllRelationsHintAdded()
    {
        // Arrange
        var services = Create();
        var provider = services.BuildServiceProvider();
        InitializeDatabase(provider);
        var id = FirstComplex(provider);

        // Act
        using var scope = provider.CreateScope();

        var container = scope.ServiceProvider.GetRequiredService<IHintsContainer>();
        container.AddHint(TestHints.TestAllRelations);

        var repository = scope.ServiceProvider.GetRequiredService<IRepository<ComplexEntity>>();
        var entity = repository.Find(id);

        // Assert
        entity.Should().NotBeNull();
        entity!.SingleRelation.Should().NotBeNull();
        entity.MultipleRelation.Should().NotBeNull();
    }

    [Fact]
    public void Must_Includes_AllRelations_When_TestSingleRelationHintAdded_And_TestMultipleRelationHintAdded()
    {
        // Arrange
        var services = Create();
        var provider = services.BuildServiceProvider();
        InitializeDatabase(provider);
        var id = FirstComplex(provider);

        // Act
        using var scope = provider.CreateScope();

        var container = scope.ServiceProvider.GetRequiredService<IHintsContainer>();
        container.AddHint(TestHints.TestSingleRelation);
        container.AddHint(TestHints.TestMultipleRelation);

        var repository = scope.ServiceProvider.GetRequiredService<IRepository<ComplexEntity>>();
        var entity = repository.Find(id);

        // Assert
        entity.Should().NotBeNull();
        entity!.SingleRelation.Should().NotBeNull();
        entity.MultipleRelation.Should().NotBeNull();
    }

    [Fact]
    public void Must_Includes_AllRelations_When_AllHintsAdded()
    {
        // Arrange
        var services = Create();
        var provider = services.BuildServiceProvider();
        InitializeDatabase(provider);
        var id = FirstComplex(provider);

        // Act
        using var scope = provider.CreateScope();

        var container = scope.ServiceProvider.GetRequiredService<IHintsContainer>();
        container.AddHint(TestHints.TestSingleRelation);
        container.AddHint(TestHints.TestMultipleRelation);
        container.AddHint(TestHints.TestAllRelations);

        var repository = scope.ServiceProvider.GetRequiredService<IRepository<ComplexEntity>>();
        var entity = repository.Find(id);

        // Assert
        entity.Should().NotBeNull();
        entity!.SingleRelation.Should().NotBeNull();
        entity.MultipleRelation.Should().NotBeNull();
    }
}
