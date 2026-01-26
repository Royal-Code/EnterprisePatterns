# RoyalCode Enterprise Patterns – Domínio (Entities e Aggregates)

Este documento descreve as bibliotecas de domínio e foca em como modelar entidades e agregados, separando o conteúdo de persistência (WorkContext/UnitOfWork/Repositories), que está documentado em `.docs/workcontext.md`.

Escopo:
- `RoyalCode.Entities`
- `RoyalCode.Aggregates`

Compatibilidade: múltiplos targets (.NET 8, .NET 9 e .NET 10) via `$(LibTargets)`.

---

## RoyalCode.Entities

Fundação para modelagem de entidades. Fornece contratos e implementações básicas para ID, Code, Guid, estado ativo e exclusão lógica.

Principais tipos
- `IEntity` / `IEntity<TId>`: marca uma entidade e o tipo do seu identificador.
- `Entity<TId>`: base com propriedade `Id` (set protegido).
- `Entity<TId, TCode>`: base com `Id` e `Code`.
- `IHasId<TId>`: expõe `Id` para entidades/DTOs.
- `IHasCode<TCode>`: expõe `Code` (identificador amigável e único, distinto do ID).
- `IHasGuid`: expõe `Guid` global (não substitui o ID; útil para referência cruzada entre bancos/contextos).
- `IActiveState`: expõe `IsActive` para habilitar/desabilitar sem deletar.
- `ISoftDeletable`: expõe `IsDeleted` para exclusão lógica.

Referência rápida de API (como Copilot deve sugerir)
- Criar entidade: `public class Product : Entity<Guid> { /* propriedades */ }`
- Criar DTO associado: `public class ProductDto : IHasId<Guid> { public Guid Id {get;set;} /* ... */ }`
- Entidade com código: `public class Sku : Entity<int, string> { /* Code já incluído */ }`

Quando usar
- Herde de `Entity<TId>` para qualquer entidade de domínio com ID.
- Use `Entity<TId, TCode>` quando existir também um código de negócio único.
- Implemente `IHasGuid`, `IActiveState` e/ou `ISoftDeletable` conforme os requisitos do domínio.

Exemplo básico
```csharp
using RoyalCode.Entities;

public class Person : Entity<int>
{
    public string Name { get; set; } = null!;
}
```

Exemplo com `Entity<TId, TCode>`
```csharp
using RoyalCode.Entities;

public class CatalogItem : Entity<Guid, string>
{
    public string Name { get; set; } = null!;
    // Code é herdado e tem set protegido
    public CatalogItem(Guid id, string code, string name)
    {
        Id = id;
        Code = code;
        Name = name;
    }
}
```

---

## RoyalCode.Aggregates

Modelagem de Agregados (DDD). Define a raiz do agregado e integra a coleta de eventos de domínio.

Principais tipos
- `IAggregateRoot` / `IAggregateRoot<TId>`: marca a raiz do agregado e o tipo do ID.
- `AggregateRoot<TId>`: base que herda de `Entity<TId>` e inclui `IDomainEventCollection? DomainEvents` + método protegido `AddEvent(IDomainEvent)`.
- `AggregateRoot<TId, TCode>`: versão com `Code` além do ID e dos eventos.

Eventos de domínio
- `AggregateRoot<TId>` mantém uma coleção de eventos (`DomainEvents`).
- Use `AddEvent(evt)` para registrar eventos quando invariantes do agregado forem alteradas.
- O despacho/persistência de eventos é responsabilidade das bibliotecas de infraestrutura; aqui apenas coletamos os eventos.

Exemplo
```csharp
using RoyalCode.Aggregates;
using RoyalCode.DomainEvents; // IDomainEvent

public sealed class Order : AggregateRoot<Guid>
{
    public string Number { get; private set; }

    public Order(string number)
    {
        Id = Guid.NewGuid();
        Number = number;
        AddEvent(new OrderCreated(Id, Number));
    }
}

public record OrderCreated(Guid OrderId, string Number) : IDomainEvent;
```

Exemplo com `AggregateRoot<TId, TCode>`
```csharp
using RoyalCode.Aggregates;
using RoyalCode.DomainEvents;

public sealed class ProductAggregate : AggregateRoot<Guid, string>
{
    public string Name { get; private set; }

    public ProductAggregate(string code, string name)
    {
        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        AddEvent(new ProductCreated(Id, Code));
    }
}

public record ProductCreated(Guid ProductId, string Code) : IDomainEvent;
```

---

## Boas práticas de modelagem
- Mantenha invariantes do agregado dentro da raiz (`AggregateRoot`) e dispare eventos com `AddEvent` após mudanças significativas.
- Não exponha `set` público para `Id`/`Code`; proteja modificações via comportamentos.
- Utilize `IHasGuid` quando precisar correlacionar a mesma entidade em múltiplos bancos/contextos.
- Prefira `ISoftDeletable` e `IActiveState` para cenários de (des)ativação e exclusão lógica.

Para integração com persistência, consulte `.docs/workcontext.md`.

