using System;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RoyalCode.Aggregates;
using RoyalCode.DomainEvents;
using RoyalCode.EventDispatcher;
using RoyalCode.Persistence.EntityFramework.Events.Entity;
using RoyalCode.Persistence.EntityFramework.Repositories.Extensions;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.UnitOfWork.Abstractions;
using Xunit;

namespace RoyalCode.Persistence.Tests.Events;

public class SaveDomainEventDetailsTests
{
    [Fact]
    public void MustSaveDomainEventDetailsOfACreatingEvent()
    {
        var eventDispatcher = new Mock<IEventDispatcher>().Object;
        
        var services = new ServiceCollection();
        services.AddSingleton(eventDispatcher);
        services.AddUnitOfWork<SaveDomainEventDetailsDbContext>()
            .ConfigureDbContextPool(builder =>
            {
                builder.UseInMemoryDatabase(nameof(MustSaveDomainEventDetailsOfACreatingEvent));
                builder.UseDomainEventHandler();
                builder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            })
            .AddRepositories(c => c.AddRepository<SaveDomainEventDetailsEntity>())
            .AddStoreDomainEventAsDetailsService();

        var sp = services.BuildServiceProvider();

        int id = 0;
        using (var scope = sp.CreateScope())
        {
            var repository = scope.ServiceProvider.GetService<IRepository<SaveDomainEventDetailsEntity>>()!;
            var uow = scope.ServiceProvider.GetService<IUnitOfWorkContext>()!;

            var entity = new SaveDomainEventDetailsEntity();
            repository.Add(entity);
            uow.Save();
            
            Assert.NotEqual(0, entity.Id);
            
            var @event = (SaveDomainEventDetailsCreated?) entity.DomainEvents?.FirstOrDefault();
            Assert.NotNull(@event);
            Assert.Equal(entity.Id, @event?.EntityId);

            id = entity.Id;
        }

        using (var scope = sp.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<SaveDomainEventDetailsDbContext>()!;

            var evtDetail = db.Events.FirstOrDefault();
            Assert.NotNull(evtDetail);

            var evt = evtDetail!.Deserialize<SaveDomainEventDetailsCreated>();
            Assert.NotNull(evt);
            Assert.Equal(id, evt?.EntityId);
        }
    }
}

public class SaveDomainEventDetailsDbContext : DbContext
{
#pragma warning disable CS8618 

    public SaveDomainEventDetailsDbContext(DbContextOptions<SaveDomainEventDetailsDbContext> options)
        : base (options)
    { }

#pragma warning restore CS8618

    public DbSet<SaveDomainEventDetailsEntity> Entity { get; set; }
    
    public DbSet<DomainEventDetails> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SaveDomainEventDetailsEntity>()
            .Property("Id").ValueGeneratedOnAdd();

        var entityTypeBuilder = modelBuilder.Entity<DomainEventDetails>();
        entityTypeBuilder.Property(d => d.Json).HasColumnType("Text");
        entityTypeBuilder.Property(d => d.TypeFullName).HasColumnType("Text");
        entityTypeBuilder.Property(d => d.Occurred).HasColumnType("datetime2");
        
        base.OnModelCreating(modelBuilder);

        modelBuilder.IgnoreDomainEventCollection();
    }
}

public class SaveDomainEventDetailsEntity : AggregateRoot<int>
{
    public SaveDomainEventDetailsEntity()
    {
        AddEvent(new SaveDomainEventDetailsCreated(this));
    }
}

public class SaveDomainEventDetailsCreated : DomainEventBase, ICreationEvent
{
    private readonly SaveDomainEventDetailsEntity? entity;

    public SaveDomainEventDetailsCreated(SaveDomainEventDetailsEntity entity)
    {
        this.entity = entity;
    }

    [JsonConstructor]
    public SaveDomainEventDetailsCreated(Guid id, DateTimeOffset occurred, int entityId) : base(id, occurred)
    {
        EntityId = entityId;
    }

    public int EntityId { get; private set; }
    
    public void Saved()
    {
        EntityId = entity!.Id;
    }
}