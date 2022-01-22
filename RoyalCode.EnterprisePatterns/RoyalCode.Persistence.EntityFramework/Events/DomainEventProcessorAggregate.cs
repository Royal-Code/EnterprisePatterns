using Microsoft.EntityFrameworkCore;
using RoyalCode.DomainEvents;

namespace RoyalCode.Persistence.EntityFramework.Events;

/// <summary>
/// <para>
///     Internal service, default implementation of <see cref="IDomainEventProcessorAggregate"/>.
/// </para>
/// </summary>
internal class DomainEventProcessorAggregate : IDomainEventProcessorAggregate
{
    private readonly IEnumerable<IDomainEventProcessor> eventProcessors;

    public DomainEventProcessorAggregate(IEnumerable<IDomainEventProcessor> eventProcessors)
    {
        this.eventProcessors = eventProcessors;
    }

    public void ProcessEvent(DbContext db, IDomainEvent evt)
    {
        foreach (var processor in eventProcessors)
        {
            processor.ProcessEvent(db, evt);
        }
    }

    public async Task ProcessEventAsync(DbContext db, IDomainEvent evt, CancellationToken token)
    {
        foreach (var processo in eventProcessors)
        {
            await processo.ProcessEventAsync(db, evt, token);
        }
    }
}
