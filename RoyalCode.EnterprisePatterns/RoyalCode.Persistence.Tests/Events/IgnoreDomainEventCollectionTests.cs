using System;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Aggregates;
using RoyalCode.DomainEvents;
using RoyalCode.EventDispatcher;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.UnitOfWork.Abstractions;
using Xunit;
using Moq;

namespace RoyalCode.Persistence.Tests.Events;

public class IgnoreDomainEventCollectionTests
{
    [Fact]
    public void MustSaveEntityButNotSaveDomainEvents()
    {
        var eventDispatcher = new Mock<IEventDispatcher>().Object;
        
        var services = new ServiceCollection();
        services.AddSingleton(eventDispatcher);
        services.AddUnitOfWork<IgnoreDomainEventCollectionDbContext>()
            .ConfigureDbContextPool(builder =>
            {
                builder.UseInMemoryDatabase(nameof(MustSaveEntityButNotSaveDomainEvents));
                builder.UseDomainEventHandler();
            })
            .ConfigureRepositories(c =>
            {
                c.Add<IgnoreDomainEventCollectionEntity>();
            });

        var sp = services.BuildServiceProvider();

        Guid id;
        using (var scope = sp.CreateScope())
        {
            var repository = scope.ServiceProvider.GetService<IRepository<IgnoreDomainEventCollectionEntity>>()!;
            var uow = scope.ServiceProvider.GetService<IUnitOfWork>()!;

            var entity = new IgnoreDomainEventCollectionEntity(nameof(MustSaveEntityButNotSaveDomainEvents));

            var @event = (IgnoreDomainEventCollectionEntityCreated?) entity.DomainEvents?.FirstOrDefault();
            Assert.NotNull(@event);
            Assert.Equal(entity.Id, @event?.EntityId);

            id = entity.Id;
            
            repository.Add(entity);
            uow.Save();
        }

        using (var scope = sp.CreateScope())
        {
            var repository = scope.ServiceProvider.GetService<IRepository<IgnoreDomainEventCollectionEntity>>()!;
            var entity = repository.Find(id);
            Assert.NotNull(entity);

            var evt = entity!.DomainEvents?.FirstOrDefault();
            Assert.Null(evt);
        }
    }
}

public class IgnoreDomainEventCollectionDbContext : DbContext
{
#pragma warning disable CS8618

    public IgnoreDomainEventCollectionDbContext(DbContextOptions<IgnoreDomainEventCollectionDbContext> options)

        : base(options)
    { }

#pragma warning restore CS8618

    public DbSet<IgnoreDomainEventCollectionEntity> Entity { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.IgnoreDomainEventCollection();
    }
}

public class IgnoreDomainEventCollectionEntity : AggregateRoot<Guid>
{
    public IgnoreDomainEventCollectionEntity(string description)
    {
        Id = Guid.NewGuid();
        Description = description;
        AddEvent(new IgnoreDomainEventCollectionEntityCreated(Id));
    }

#pragma warning disable CS8618
    
    protected IgnoreDomainEventCollectionEntity() { }

#pragma warning restore CS8618 

    public string Description { get; }
}

public class IgnoreDomainEventCollectionEntityCreated : DomainEventBase
{
    public IgnoreDomainEventCollectionEntityCreated(Guid entityId)
    {
        EntityId = entityId;
    }

    [JsonConstructor]
    public IgnoreDomainEventCollectionEntityCreated(Guid id, DateTimeOffset occurred, Guid entityId) 
        : base(id, occurred)
    {
        EntityId = entityId;
    }

    public Guid EntityId { get; }
}