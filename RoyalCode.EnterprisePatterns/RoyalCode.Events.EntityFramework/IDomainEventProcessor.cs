using Microsoft.EntityFrameworkCore;
using RoyalCode.DomainEvents;

namespace RoyalCode.Persistence.EntityFramework.Events;

/// <summary>
/// <para>
///     Service interface to process domain events handled by the unit of work.
/// </para>
/// </summary>
public interface IDomainEventProcessor
{
    /// <summary>
    /// <para>
    ///     After dispatched the domain event by the unit of work event handler, this processor
    ///     are called to manage the domain event.
    /// </para>
    /// </summary>
    /// <param name="db">The <see cref="DbContext"/> used by the unit of work.</param>
    /// <param name="evt">The handled domain event.</param>
    void ProcessEvent(DbContext db, IDomainEvent evt);

    /// <summary>
    /// <para>
    ///     After dispatched the domain event by the unit of work event handler, this processo
    ///     are called to manage the domain event.
    /// </para>
    /// </summary>
    /// <param name="db">The <see cref="DbContext"/> used by the unit of work.</param>
    /// <param name="evt">The handled domain event.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>
    ///     Task for async operation.
    /// </returns>
    Task ProcessEventAsync(DbContext db, IDomainEvent evt, CancellationToken token);
}
