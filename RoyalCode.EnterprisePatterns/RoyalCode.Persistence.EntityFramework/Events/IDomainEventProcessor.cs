using Microsoft.EntityFrameworkCore;
using RoyalCode.DomainEvents;

namespace RoyalCode.Persistence.EntityFramework.Events;

public interface IDomainEventProcessor
{
    void ProcessEvent(DbContext db, IDomainEvent evt);
    
    Task ProcessEventAsync(DbContext db, IDomainEvent evt, CancellationToken token);
}