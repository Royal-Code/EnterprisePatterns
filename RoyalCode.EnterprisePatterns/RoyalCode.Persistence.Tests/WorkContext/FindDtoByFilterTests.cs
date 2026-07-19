using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.Tests.Entities;
using RoyalCode.Repositories;
using RoyalCode.Repositories.EntityFramework;
using RoyalCode.SmartProblems;
using RoyalCode.SmartProblems.Entities;
using RoyalCode.UnitOfWork.EntityFramework;
using Xunit;

namespace RoyalCode.Persistence.Tests.WorkContext;

/// <summary>
/// Tests for the DTO projection by predicate of <see cref="IFinder{TEntity}"/>:
/// the projection runs in the provider (single query, DTO columns only, no tracking)
/// and the not-found problem names the entity, using the criteria in declaration order.
/// </summary>
public class FindDtoByFilterTests
{
    private static ServiceProvider CreateProvider(bool withQueryFilter = false)
    {
        ServiceCollection services = new();

        // o modelo do EF é cacheado por tipo de contexto; o cenário com query filter usa um
        // contexto próprio para não herdar o modelo (sem filtro) cacheado pelo DefaultDbContext.
        if (withQueryFilter)
        {
            services.AddSqliteInMemoryWorkContext<FilteredDbContext>()
                .EnsureDatabaseCreated()
                .ConfigureModel(mb =>
                {
                    var person = mb.Entity<Person>();
                    person.Property("Id").ValueGeneratedOnAdd();
                    person.HasQueryFilter(p => p.Name != "Hidden");
                })
                .ConfigureRepositories(c => c.Add<Person>())
                .SeedDatabase(SeedAsync);

            return services.BuildServiceProvider();
        }

        services.AddSqliteInMemoryWorkContextDefault()
            .EnsureDatabaseCreated()
            .ConfigureModel(mb => mb.Entity<Person>().Property("Id").ValueGeneratedOnAdd())
            .ConfigureRepositories(c => c.Add<Person>())
            .SeedDatabase(SeedAsync);

        return services.BuildServiceProvider();
    }

    private static async Task SeedAsync(DbContext db)
    {
        db.Add(new Person { Name = "Alice" });
        db.Add(new Person { Name = "Bob" });
        db.Add(new Person { Name = "Bob" }); // duplicata proposital
        db.Add(new Person { Name = "Hidden" });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task FindDto_ByFilter_Found()
    {
        // arrange
        var sp = CreateProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();

        // act
        var result = await repository.FindAsync<PersonDto>(p => p.Name == "Alice");

        // assert
        Assert.True(result.Found);
        Assert.Equal("Alice", result.Entity!.Name);
        Assert.True(result.Entity.Id > 0);
    }

    [Fact]
    public async Task FindDto_ByFilter_WithExplicitCriteria_Found()
    {
        // arrange
        var sp = CreateProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();

        // act
        var result = await repository.FindAsync<PersonDto>(
            p => p.Name == "Alice",
            [new FindCriterion(nameof(Person.Name), "Alice")]);

        // assert
        Assert.True(result.Found);
        Assert.Equal("Alice", result.Entity!.Name);
    }

    [Fact]
    public async Task FindDto_NotFound_NamesEntity_WithCriteriaInDeclarationOrder()
    {
        // arrange
        var sp = CreateProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();

        // act: chave composta (dois critérios) não encontrada, variante explícita
        var result = await repository.FindAsync<PersonDto>(
            p => p.Name == "Nobody" && p.Id == 99,
            [new FindCriterion(nameof(Person.Name), "Nobody"), new FindCriterion(nameof(Person.Id), 99)]);

        // assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal(ProblemCategory.NotFound, problem!.Category);
        Assert.Equal("The record of 'Person' with Name 'Nobody', Id '99' was not found", problem.Detail);
        Assert.Equal(nameof(Person), problem.Extensions!["entity"]);
        Assert.Equal("Nobody", problem.Extensions!["Name"]);
        Assert.Equal(99, problem.Extensions!["Id"]);
    }

    [Fact]
    public async Task FindDto_NotFound_ConvenienceOverload_ExtractsCriteriaFromExpression()
    {
        // arrange
        var sp = CreateProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();

        // act: sem criteria explícitos; análise best-effort da expressão
        var result = await repository.FindAsync<PersonDto>(p => p.Name == "Nobody");

        // assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal("The record of 'Person' with Name 'Nobody' was not found", problem!.Detail);
    }

    [Fact]
    public async Task FindDto_NotFound_ConvenienceOverload_DegradesToGenericMessage_OnUnsupportedFilter()
    {
        // arrange
        var sp = CreateProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();

        // act: OR não é conversível em critérios; degrada para a mensagem genérica
        var result = await repository.FindAsync<PersonDto>(p => p.Name == "Nobody" || p.Id == 99);

        // assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal("The record for 'Person' was not found", problem!.Detail);
    }

    [Fact]
    public async Task FindDto_NullableValue_ProducesCriterionWithNull()
    {
        // arrange
        var sp = CreateProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();
        string? name = null;

        // act
        var result = await repository.FindAsync<PersonDto>(
            p => p.Name == name,
            [new FindCriterion(nameof(Person.Name), name)]);

        // assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal("The record of 'Person' with Name '' was not found", problem!.Detail);
    }

    [Fact]
    public async Task FindDto_Duplicates_ReturnsFirstMatch_WithoutError()
    {
        // arrange
        var sp = CreateProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();

        // act: duas linhas com Name == "Bob"; FirstOrDefault, sem exceção
        var result = await repository.FindAsync<PersonDto>(p => p.Name == "Bob");

        // assert
        Assert.True(result.Found);
        Assert.Equal("Bob", result.Entity!.Name);
    }

    [Fact]
    public async Task FindDto_Cancellation_Propagates()
    {
        // arrange
        var sp = CreateProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // act & assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => repository.FindAsync<PersonDto>(p => p.Name == "Alice", cts.Token));
    }

    [Fact]
    public async Task FindDto_QueryFilter_IsPreserved()
    {
        // arrange
        var sp = CreateProvider(withQueryFilter: true);
        var repository = sp.GetRequiredService<IRepository<Person>>();

        // act: a linha existe, mas o query filter global a exclui
        var result = await repository.FindAsync<PersonDto>(p => p.Name == "Hidden");

        // assert
        Assert.True(result.NotFound(out _));
    }

    [Fact]
    public async Task FindDto_DoesNotTrackEntities()
    {
        // arrange
        var sp = CreateProvider();
        using var scope = sp.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Person>>();
        var db = scope.ServiceProvider.GetRequiredService<DefaultDbContext>();

        // act
        var result = await repository.FindAsync<PersonDto>(p => p.Name == "Alice");

        // assert
        Assert.True(result.Found);
        Assert.Empty(db.ChangeTracker.Entries());
    }

    [Fact]
    public void FindDto_Sql_SelectsOnlyDtoColumns_InSingleQuery()
    {
        // arrange
        var sp = CreateProvider();
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DefaultDbContext>();

        // act: a mesma consulta usada pelo repositório
        var sql = SelectDto<Person, PersonDto>.SelectWhere(db, p => p.Name == "Alice")
            .Take(1)
            .ToQueryString();

        // assert: uma única consulta, projetando apenas as colunas do DTO
        var selectClause = sql[..sql.IndexOf("FROM", StringComparison.OrdinalIgnoreCase)];
        Assert.Contains("\"Id\"", selectClause);
        Assert.Contains("\"Name\"", selectClause);
        Assert.Equal(sql.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase),
            sql.LastIndexOf("SELECT", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task FindDto_MissingSelector_ThrowsClearError()
    {
        // arrange
        var sp = CreateProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();

        // act & assert: não há seletor de Person para BrokenDto (propriedade sem correspondência)
        var ex = await Assert.ThrowsAnyAsync<Exception>(
            () => repository.FindAsync<BrokenDto>(p => p.Name == "Alice"));

        Assert.Contains(nameof(BrokenDto), ex.Message);
    }

    [Fact]
    public async Task FindDtoById_NotFound_NamesEntity_NotDto()
    {
        // arrange: DF16 — o caminho por ID também nomeia a entidade, não o DTO
        var sp = CreateProvider();
        var repository = sp.GetRequiredService<IRepository<Person>>();
        Id<Person, int> id = 999;

        // act
        var result = await repository.FindAsync<PersonDto, int>(id);

        // assert
        Assert.True(result.NotFound(out var problem));
        Assert.Equal("The record of 'Person' with id '999' was not found", problem!.Detail);
        Assert.Equal(nameof(Person), problem.Extensions!["entity"]);
        Assert.Equal(999, problem.Extensions!["id"]);
    }

    public class BrokenDto
    {
        public string PropertyThatDoesNotExist { get; set; } = string.Empty;
    }

    public sealed class FilteredDbContext(DbContextOptions<FilteredDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.ConfigureModelWithServices(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            this.ConfigureConventionsWithServices(configurationBuilder);
            base.ConfigureConventions(configurationBuilder);
        }
    }
}
