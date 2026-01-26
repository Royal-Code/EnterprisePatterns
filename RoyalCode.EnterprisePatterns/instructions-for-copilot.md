# Instruções para GitHub Copilot – Uso de RoyalCode WorkContext, UnitOfWork e Repositories

Estas regras ajudam o Copilot a gerar código correto usando as bibliotecas RoyalCode:

## Contexto e preferências
- Use `RoyalCode.WorkContext` como API principal para persistência, repositórios, comandos e consultas.
- Evite acessar `DbContext` diretamente; prefira `IWorkContext`.
- Registre repositórios apenas para raízes de agregado quando aplicável.
- Para desenvolvimento/teste, use SQLite in‑memory; produção: configure Npgsql (PostgreSQL) ou SQL Server.

## Setup padrão (DI + EF Core)
- Configure WorkContext com o provider correto:
  - Sqlite in‑memory: `UseSqlite(new SqliteConnection("DataSource=:memory:"))` + `conn.Open()`.
  - Sqlite arquivo: `UseSqlite("Data Source=mydb.sqlite")`.
  - PostgreSQL: `UseNpgsql("Host=...;Database=...;Username=...;Password=...")`.
  - SQL Server: `UseSqlServer("Server=...;Database=...;Integrated Security=True")`.
- Após configurar, chame `EnsureDatabaseCreated()` apenas em dev/test.
- Use `ConfigureModel(...)` e/ou `ConfigureMappingsFromAssembly(...)` para aplicar mapeamentos.
- Registre componentes por assembly:
  - `AddRepositories(assembly)`
  - `ConfigureSearches(assembly)`
  - `ConfigureCommands(assembly)`
  - `ConfigureQueries(assembly)`

## Operações de domínio
- Entidade: herde de `Entity<TId>`. Ex.: `public class Person : Entity<int> { /* ... */ }`.
- Agregado: herde de `AggregateRoot<TId>` e dispare eventos com `AddEvent(...)`.

## Operações de persistência
- Repositório: `var repo = context.Repository<Person>();`
- Criar e salvar: `await repo.AddAsync(entity); await context.SaveAsync();`
- Buscar por ID: `await repo.FindAsync(id);`
- Criteria: `var criteria = context.Criteria<Person>(); var list = criteria.Collect();`

## Commands
- Defina requests/handlers:
  - `class CreatePerson : ICommandRequest { public string Name { get; set; } }`
  - `class CreatePersonHandler : ICommandHandler<CreatePerson> { /* chama context.Add e SaveAsync */ }`
- Envie: `await context.SendAsync(new CreatePerson { Name = "John" }, ct);`
- Se usar dispatcher: registre `AddCommandDispatcher()`.

## Queries
- Lista: `IQueryRequest<TEntity>` e handler com `ToListAsync`.
- Stream: `IAsyncQueryRequest<TEntity>` e handler com `AsAsyncEnumerable`.
- Execução: `await context.QueryAsync(request, ct)` ou `await foreach (var item in context.QueryAsync(request, ct)) { }`.

## Boas práticas e anti‑padrões
- Centralize regras de escrita em Commands; consultas em Queries.
- Não salve fora de `IWorkContext.SaveAsync()`.
- Não acesse `DbContext` diretamente em serviços de aplicação.
- Mantenha mapeamentos e registros por assembly para modularidade.

## Troubleshooting
- Handler não registrado: verifique `ConfigureCommands/ConfigureQueries` e o assembly correto.
- Banco não criado em dev/test: chame `EnsureDatabaseCreated()`.
- Conexão SQLite in‑memory: mantenha a conexão aberta durante o uso do contexto.
- DI: confirme `AddWorkContext<TDbContext>()` e construa o provedor após configurações.
