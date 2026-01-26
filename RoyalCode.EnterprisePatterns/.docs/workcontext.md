# RoyalCode Enterprise Patterns – WorkContext, UnitOfWork e Repositories

A `RoyalCode.WorkContext` é a API recomendada para uso. Ela estende o padrão Unit of Work e agrega funcionalidades de Repositórios, Busca (SmartSearch), Commands e Queries, oferecendo uma experiência unificada.

Este documento descreve:
- `RoyalCode.WorkContext.Abstractions`: contratos e builder do Work Context.
- `RoyalCode.UnitOfWork.Abstractions`: contratos do padrão Unit of Work consumidos pelo WorkContext.
- `RoyalCode.Repositories.Abstractions`: contratos do padrão Repository consumidos pelo WorkContext.
- Implementações EF Core: `RoyalCode.WorkContext.EntityFramework`, `RoyalCode.WorkContext.PostgreSql`, `RoyalCode.WorkContext.Sqlite`, `RoyalCode.WorkContext.SqlServer`.

Targets: .NET 8, .NET 9, .NET 10 (via `$(LibTargets)`).

---

## WorkContext (Abstrações)

Conceitos principais
- `IWorkContext`: contexto unificado que expõe operações de UoW, acesso a repositórios, buscas, envio de comandos e execução de consultas.
- `IWorkContextBuilder<TDbContext>`: builder fluente para configurar o WorkContext, DbContext (EF Core), repositórios, buscas, commands e queries.
- Commands/Queries: padrões de request/response integrados ao WorkContext.

Principais extensões de configuração (EntityFramework)
- `ConfigureDbContextPool(...)`: registra o `DbContext` e configura o provider EF Core.
- `ConfigureModel(...)` e `ConfigureMappingsFromAssembly(...)`: aplica configurações de modelagem/mapeamentos.
- `EnsureDatabaseCreated()`: garante que o banco seja criado (apenas para cenários de teste/dev).
- `SeedDatabase(...)`: semeia dados iniciais.
- `ConfigureRepositories(...)` e `AddRepositories(...)`: registra repositórios para entidades.
- `ConfigureSearches(...)`: registra pesquisas (SmartSearch).
- `ConfigureCommands(...)` e `AddCommandDispatcher()`: registra handlers de comandos e o dispatcher opcional.
- `ConfigureQueries(...)`: registra handlers de consultas síncronas e assíncronas.

Mapa de provedores EF Core (como Copilot deve sugerir)
- Sqlite in‑memory: `UseSqlite(new SqliteConnection("DataSource=:memory:"))` + `conn.Open()`
- Sqlite arquivo: `UseSqlite("Data Source=mydb.sqlite")`
- PostgreSQL (Npgsql): `UseNpgsql("Host=...;Database=...;Username=...;Password=...")`
- SQL Server: `UseSqlServer("Server=...;Database=...;Integrated Security=True")`

Referência rápida de API
- Builder: `AddWorkContext<TDbContext>()` → `ConfigureDbContextPool(...)` → `ConfigureModel(...)` → `ConfigureRepositories(...)` → `ConfigureSearches(...)` → `ConfigureCommands(...)` → `ConfigureQueries(...)`
- Operações: `Repository<TEntity>()`, `Criteria<TEntity>()`, `SaveAsync()`, `SendAsync(command)`, `QueryAsync(request)`
- Registros por assembly: `AddRepositories(assembly)`, `ConfigureSearches(assembly)`, `ConfigureCommands(assembly)`, `ConfigureQueries(assembly)`

Operações principais
- `Repository<TEntity>()`: obtém o repositório da entidade.
- `Criteria<TEntity>()`: obtém critérios de pesquisa (SmartSearch) para a entidade.
- `Save()` / `SaveAsync()`: persiste alterações (Unit of Work).
- `SendAsync(command)`: executa comandos.
- `QueryAsync(request)`: executa consultas síncronas (lista) ou assíncronas (stream).

Tarefas comuns (snippets que Copilot deve sugerir)
- CRUD básico com repositório
```csharp
var person = new Person { Name = "John" };
await context.Repository<Person>().AddAsync(person);
await context.SaveAsync();
var found = await context.Repository<Person>().FindAsync(person.Id);
```

- Query síncrona
```csharp
var result = await context.QueryAsync(new GetPersons { Name = "John" }, ct);
```

- Query assíncrona (stream)
```csharp
await foreach (var p in context.QueryAsync(new StreamPersons { Name = "John" }, ct)) { /* ... */ }
```

- Command
```csharp
var res = await context.SendAsync(new CreatePerson { Name = "John" }, ct);
```

---

## UnitOfWork (Abstrações)

- Define contratos para controle transacional e persistência.
- O WorkContext implementa/estende `IUnitOfWork`, oferecendo uma API coesa.

Principais operações
- `Save()` / `SaveAsync()`: salvamento de alterações.
- Integração com Repositórios (Add/Update/Delete/Find) e com Commands/Queries.

---

## Repositories (Abstrações)

- Contratos para o padrão Repository orientado a entidades.
- Consumidos pelo WorkContext para operações CRUD e consulta por ID.

Operações típicas
- `Add(entity)` / `AddAsync(entity)`
- `Find(id)` / `FindAsync(id)`
- Suporte a ID fortemente tipado (`Id<TEntity, TId>`), mapeamentos e DTOs.

---

## Implementações com Entity Framework

As seguintes bibliotecas fornecem integrações com EF Core e provedores específicos:
- `RoyalCode.WorkContext.EntityFramework`: integração base com EF Core.
- `RoyalCode.WorkContext.PostgreSql`: configuração orientada ao provider Npgsql.
- `RoyalCode.WorkContext.Sqlite`: configuração orientada ao provider SQLite.
- `RoyalCode.WorkContext.SqlServer`: configuração orientada ao provider SQL Server.

Recursos comuns
- Builders e extensões para configurar o `DbContext` e registrar serviços.
- Utilitários de criação/semente (dev/test) e aplicação de mapeamentos por assembly.
- Registro de repositórios, buscas, comandos e queries.

Exemplos de testes do repositório (adaptado)
```csharp
// Configuração em memória SQLite
services.AddSqliteInMemoryWorkContextDefault()
    .EnsureDatabaseCreated()
    .ConfigureModel(b => b.ApplyConfigurationsFromAssembly(typeof(PersonMapping).Assembly))
    .ConfigureRepositories(c => c.Add<Person>())
    .ConfigureSearches(c => c.Add<Person>())
    .SeedDatabase(async db => { /* dados iniciais */ await db.SaveChangesAsync(); });

// Uso
var context = sp.GetRequiredService<IWorkContext>();
var repo = context.Repository<Person>();
await repo.AddAsync(new Person { Name = "John" });
await context.SaveAsync();
```

---

## Tutorial: Configuração completa do WorkContext

Abaixo um exemplo de módulo que aplica configurações completas (modelo, repositórios, buscas, comandos e queries) reutilizáveis:

```csharp
using Microsoft.EntityFrameworkCore;
using RoyalCode.WorkContext;

public static class MyModuleConfigureWorkContext
{
    public static IWorkContextBuilder<TDbContext> ConfigureMyModule<TDbContext>(this IWorkContextBuilder<TDbContext> builder)
        where TDbContext : DbContext
    {
        return builder.ConfigureModel(modelBuilder => modelBuilder.MapMyModule())
            .AddRepositories(typeof(MyModuleConfigureWorkContext).Assembly)
            .ConfigureSearches(typeof(MyModuleConfigureWorkContext).Assembly)
            .ConfigureCommands(typeof(MyModuleConfigureWorkContext).Assembly)
            .ConfigureQueries(typeof(MyModuleConfigureWorkContext).Assembly);
    }
}
```

Uso com SQLite In-Memory (dev/test)

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.WorkContext;

ServiceCollection services = new();

services.AddWorkContext<MyDbContext>()
    .ConfigureDbContextPool((sp, builder) =>
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();
        builder.UseSqlite(conn);
    })
    .ConfigureMyModule()
    .EnsureDatabaseCreated();

var sp = services.BuildServiceProvider();
var ctx = sp.GetRequiredService<IWorkContext>();
```

Prompts úteis para Copilot
- "Configure WorkContext com SQLite in‑memory, registre repositórios e crie um handler `CreatePerson`."
- "Mapeie uma query para listar `Person` por `Name` usando `ConfigureQueries`."
- "Adicione CommandDispatcher e envie o comando `CreatePerson`."
```

Uso com PostgreSQL
```csharp
services.AddWorkContext<MyDbContext>()
    .ConfigureDbContextPool((sp, builder) => builder.UseNpgsql("Host=...;Database=...;Username=...;Password=..."))
    .ConfigureMyModule();
```

Uso com SQL Server
```csharp
services.AddWorkContext<MyDbContext>()
    .ConfigureDbContextPool((sp, builder) => builder.UseSqlServer("Server=...;Database=...;Integrated Security=True"))
    .ConfigureMyModule();
```

Uso com SQLite (arquivo)
```csharp
services.AddWorkContext<MyDbContext>()
    .ConfigureDbContextPool((sp, builder) => builder.UseSqlite("Data Source=mydb.sqlite"))
    .ConfigureMyModule();
```

---

## Commands e Queries (uso básico)

Commands
```csharp
using RoyalCode.WorkContext.Commands;

public sealed class CreatePerson : ICommandRequest
{
    public string Name { get; set; } = null!;
}

public sealed class CreatePersonHandler : ICommandHandler<CreatePerson>
{
    public async Task<Result> HandleAsync(CreatePerson request, IWorkContext context, CancellationToken ct)
    {
        context.Add(new Person { Name = request.Name });
        return await context.SaveAsync(ct);
    }
}

// Envio
var result = await context.SendAsync(new CreatePerson { Name = "John" }, default);
```

Nota: `Result` e `Result<T>` (incluindo operações como `HasProblems`, `MapAsync`) pertencem a outra biblioteca. Consulte a documentação específica dessa biblioteca para detalhes.
```

Queries (lista)
```csharp
using Microsoft.EntityFrameworkCore;
using RoyalCode.WorkContext.Querying;

public sealed class GetPersons : IQueryRequest<Person>
{
    public string? Name { get; set; }
}

// Registro inline com builder
builder.ConfigureQueries(c =>
{
    c.Handle<GetPersons, Person>(async (req, db, ct) =>
        await db.Set<Person>().Where(p => p.Name == req.Name).ToListAsync(ct));
});

// Execução
var persons = await context.QueryAsync(new GetPersons { Name = "John" }, default);
```

### Queries com IQueryHandler (handlers tipados)

Implementando handlers tipados com Entity Framework (Entities)
```csharp
using Microsoft.EntityFrameworkCore;
using RoyalCode.WorkContext.Querying;
using RoyalCode.WorkContext.EntityFramework.Querying;

public sealed class GetPersons : IQueryRequest<Person>
{
    public string? Name { get; set; }
}

public sealed class GetPersonsHandler : IQueryHandler<MyDbContext, GetPersons, Person>
{
    public async Task<IEnumerable<Person>> HandleAsync(GetPersons request, MyDbContext db, CancellationToken ct = default)
    {
        return await db.Set<Person>()
            .Where(p => request.Name == null || p.Name == request.Name)
            .ToListAsync(ct);
    }
}
```

Registrando handlers por assembly
```csharp
services.AddWorkContext<MyDbContext>()
    .ConfigureDbContextPool((sp, b) => b.UseSqlite("DataSource=:memory:"))
    .ConfigureQueries(typeof(GetPersonsHandler).Assembly); // faz scan de IQueryHandler<,...>
```

Implementando handlers com DTO (IQueryRequest<TEntity, TModel>)
```csharp
using Microsoft.EntityFrameworkCore;
using RoyalCode.WorkContext.Querying;
using RoyalCode.WorkContext.EntityFramework.Querying;

public sealed class PersonDto { public int Id { get; set; } public string Name { get; set; } = null!; }

public sealed class GetPersonsDto : IQueryRequest<Person, PersonDto>
{
    public string? Name { get; set; }
}

public sealed class GetPersonsDtoHandler : IQueryHandler<MyDbContext, GetPersonsDto, Person, PersonDto>
{
    public async Task<IEnumerable<PersonDto>> HandleAsync(GetPersonsDto request, MyDbContext db, CancellationToken ct = default)
    {
        return await db.Set<Person>()
            .Where(p => request.Name == null || p.Name == request.Name)
            .Select(p => new PersonDto { Id = p.Id, Name = p.Name })
            .ToListAsync(ct);
    }
}
```

Execução
```csharp
var list = await context.QueryAsync(new GetPersons { Name = "John" }, ct);
var dtos = await context.QueryAsync(new GetPersonsDto { Name = "John" }, ct);
```

Queries (stream assíncrono)
```csharp
using RoyalCode.WorkContext.Querying;

public sealed class StreamPersons : IAsyncQueryRequest<Person>
{
    public string? Name { get; set; }
}

builder.ConfigureQueries(c =>
{
    c.AsyncHandle<StreamPersons, Person>((req, db, ct) =>
        db.Set<Person>().Where(p => p.Name == req.Name).AsAsyncEnumerable());
});

await foreach (var p in context.QueryAsync(new StreamPersons { Name = "John" }, default))
{
    // consumir stream
}
```

Troubleshooting
- Handler não registrado: verifique `ConfigureCommands`/`ConfigureQueries` e assembly (`typeof(Handler).Assembly`).
- Banco não criado em dev/test: use `EnsureDatabaseCreated()` após configurar o provider.
- Conexão SQLite in‑memory vazando: certifique‑se de manter a conexão aberta no escopo de vida do `DbContext`.
- Problemas de DI: valide `AddWorkContext<TDbContext>()` foi chamado e `BuildServiceProvider()` após todas as configurações.
```

---

## Boas práticas
- Centralize regras de negócio em Commands; use Queries para leitura eficiente.
- Configure mapeamentos via assemblies para manter organização modular.
- Use `EnsureDatabaseCreated` e `SqliteInMemory` apenas em dev/test.
- Evite acessar o `DbContext` diretamente; prefira `IWorkContext` e seus serviços.
- Registre repositórios somente para raízes de agregado quando aplicável.

## Referência rápida
- Configurar WorkContext: `AddWorkContext<TDbContext>().ConfigureDbContextPool(...).ConfigureModel(...).ConfigureRepositories(...).ConfigureSearches(...).ConfigureCommands(...).ConfigureQueries(...)`.
- Obter serviços: `IWorkContext`, `IRepository<TEntity>`, `ISearchManager`, `ICommandDispatcher`.
- Persistir: `SaveAsync()`.
- Enviar comando: `SendAsync(cmd)`.
- Executar consulta: `QueryAsync(request)`.
