using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RoyalCode.DomainEvents;
using RoyalCode.Outbox.Abstractions.Services;

namespace RoyalCode.Outbox.EntityFramework.Services;

/// <summary>
/// A service that monitors domain events and writes them as messages to the Outbox using EntityFrameworkCore.
/// </summary>
public sealed class OutboxTracker
{
    private readonly Func<IOutboxService> getOutboxService;
    private List<IDomainEvent>? domainEvents;

    /// <summary>
    /// Initialises a new instance of <see cref="OutboxTracker"/> for the <see cref="DbContext"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <returns>A new instance of <see cref="OutboxTracker"/>.</returns>
    public static OutboxTracker Initialize(DbContext dbContext)
    {
        return InitializeCore(dbContext, dbContext.GetService<IOutboxService>);
    }

    /// <summary>
    /// Initialises a new instance of <see cref="OutboxTracker"/> for the <see cref="DbContext"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <param name="outboxService">The <see cref="IOutboxService"/>.</param>
    /// <returns>A new instance of <see cref="OutboxTracker"/>.</returns>
    public static OutboxTracker Initialize(DbContext dbContext, IOutboxService outboxService)
    {
        return InitializeCore(dbContext, () => outboxService);
    }

    private static OutboxTracker InitializeCore(DbContext dbContext, Func<IOutboxService> getOutboxService)
    {
        var outboxTracker = new OutboxTracker(getOutboxService);

        dbContext.ChangeTracker.Tracking += outboxTracker.TrackDomainEvents;
        dbContext.SavingChanges += outboxTracker.WriteEventsToOutbox;

        return outboxTracker;
    }

    private OutboxTracker(Func<IOutboxService> getOutboxService)
    {
        this.getOutboxService = getOutboxService;
    }

    private void TrackDomainEvents(object? sender, EntityTrackingEventArgs e)
    {
        if (e.Entry.Entity is IHasEvents hasEvents)
        {
            hasEvents.DomainEvents ??= new DomainEventCollection();
            hasEvents.DomainEvents.Observe(DomainEventAdded);
        }
    }

    private void DomainEventAdded(IDomainEvent @event)
    {
        domainEvents ??= new();
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
}
