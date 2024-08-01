using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RoyalCode.DomainEvents;
using RoyalCode.Outbox.Abstractions.Services;

namespace RoyalCode.Outbox.EntityFramework.Services;

/// <summary>
/// A service that monitors domain events and writes them as messages to the Outbox using EntityFrameworkCore.
/// </summary>
public sealed class OutboxTracker : IDisposable
{
    private readonly Func<IOutboxService> getOutboxService;
    private Action disposing;
    private List<IDomainEvent>? domainEvents;

    /// <summary>
    /// Initialises a new instance of <see cref="OutboxTracker"/> for the <see cref="DbContext"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <returns>A new instance of <see cref="OutboxTracker"/>.</returns>
    public static OutboxTracker Initialize(DbContext dbContext)
    {
        return new(dbContext, dbContext.GetService<IOutboxService>);
    }

    /// <summary>
    /// Initialises a new instance of <see cref="OutboxTracker"/> for the <see cref="DbContext"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <param name="outboxService">The <see cref="IOutboxService"/>.</param>
    /// <returns>A new instance of <see cref="OutboxTracker"/>.</returns>
    public static OutboxTracker Initialize(DbContext dbContext, IOutboxService outboxService)
    {
        return new(dbContext, () => outboxService);
    }

    private OutboxTracker(DbContext dbContext, Func<IOutboxService> getOutboxService)
    {
        this.getOutboxService = getOutboxService;
        
        EventHandler<EntityTrackingEventArgs> trackDomainEvents = TrackDomainEvents;
        EventHandler<SavingChangesEventArgs> writeEventsToOutbox = WriteEventsToOutbox;
        
        dbContext.ChangeTracker.Tracking += trackDomainEvents;
        dbContext.SavingChanges += writeEventsToOutbox;
        
        disposing = () =>
        {
            dbContext.ChangeTracker.Tracking -= trackDomainEvents;
            dbContext.SavingChanges -= writeEventsToOutbox;
        };
    }

    private void TrackDomainEvents(object? sender, EntityTrackingEventArgs e)
    {
        if (e.Entry.Entity is IHasEvents hasEvents)
        {
            hasEvents.DomainEvents ??= new DomainEventCollection();
            var domainEventAdded = DomainEventAdded;
            hasEvents.DomainEvents.Observe(domainEventAdded);
            disposing += () => hasEvents.DomainEvents.RemoveObserver(domainEventAdded);
        }
    }

    private void DomainEventAdded(IDomainEvent @event)
    {
        domainEvents ??= [];
        domainEvents.Add(@event);
    }

    private void WriteEventsToOutbox(object? sender, SavingChangesEventArgs e)
    {
        if (domainEvents is null)
            return;

        var service = getOutboxService() ?? throw new InvalidOperationException(R.FailedWriteToOutbox);

        foreach (var evt in domainEvents)
            service.Write(evt);

        domainEvents.Clear();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        disposing();
        domainEvents?.Clear();
    }
}
