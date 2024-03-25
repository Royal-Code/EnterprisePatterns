using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RoyalCode.Aggregates;
using RoyalCode.DomainEvents;
using RoyalCode.EventDispatcher;
using RoyalCode.Repositories.Abstractions;
using RoyalCode.UnitOfWork.Abstractions;
using Xunit;

namespace RoyalCode.Persistence.Tests.Events;

public class DispatchDomainEventsTests
{
    [Fact]
    public void MustDispatchDomainEvents()
    {
        var mock = CreateMock();
        var eventDispatcher = mock.Object;

        var services = new ServiceCollection();
        services.AddSingleton(eventDispatcher);
        services.AddUnitOfWork<DispatchDomainEventsDbContext>()
            .AddDomainEventHandler()
            .ConfigureDbContextPool(builder =>
            {
                builder.UseInMemoryDatabase(nameof(MustDispatchDomainEvents));
                builder.UseDomainEventHandler();
            })
            .ConfigureRepositories(c => c.Add<DispatchDomainEventsEntity>());

        var sp = services.BuildServiceProvider();

        using (var scope = sp.CreateScope())
        {
            var repository = scope.ServiceProvider.GetService<IRepository<DispatchDomainEventsEntity>>()!;
            var uow = scope.ServiceProvider.GetService<IUnitOfWork>()!;

            var entity = new DispatchDomainEventsEntity();
            repository.Add(entity);
            uow.Save();
        }
        
        Verify(mock);
    }

    private static Mock<IEventDispatcher> CreateMock()
    {
        var mock = new Mock<IEventDispatcher>();

        mock.Setup(m => m.Dispatch(It.IsAny<Type>(), It.IsAny<object>(), DispatchStrategy.InCurrentScope))
            .Verifiable();

        mock.Setup(m => m.Dispatch(It.IsAny<Type>(), It.IsAny<object>(), DispatchStrategy.InSeparetedScope))
            .Verifiable();

        return mock;
    }

    private static void Verify(Mock<IEventDispatcher> mock)
    {
        mock.Verify(m => m.Dispatch(It.IsAny<Type>(), It.IsAny<object>(), DispatchStrategy.InCurrentScope), 
            Times.Once);
        mock.Verify(m => m.Dispatch(It.IsAny<Type>(), It.IsAny<object>(), DispatchStrategy.InSeparetedScope),
            Times.Once);
    }
}

public class DispatchDomainEventsDbContext : DbContext
{
#pragma warning disable CS8618
    
    public DispatchDomainEventsDbContext(DbContextOptions<DispatchDomainEventsDbContext> options)
        : base(options)
    { }

#pragma warning restore CS8618

    public DbSet<DispatchDomainEventsEntity> Entity { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.IgnoreDomainEventCollection();
    }
}

public class DispatchDomainEventsEntity : AggregateRoot<Guid>
{
    public DispatchDomainEventsEntity()
    {
        Id = Guid.NewGuid();
        AddEvent(new DispatchDomainEventsDomainEvent(Id));
    }
}

public class DispatchDomainEventsDomainEvent : DomainEventBase
{
    public Guid EntityId { get; }

    public DispatchDomainEventsDomainEvent(Guid entityId)
    {
        EntityId = entityId;
    }

    public DispatchDomainEventsDomainEvent(Guid id, DateTimeOffset occurred, Guid entityId) : base(id, occurred)
    {
        EntityId = entityId;
    }
}