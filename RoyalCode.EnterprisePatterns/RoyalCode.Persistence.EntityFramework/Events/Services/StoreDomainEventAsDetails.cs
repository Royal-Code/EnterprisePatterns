
using Microsoft.EntityFrameworkCore;
using RoyalCode.DomainEvents;
using RoyalCode.Persistence.EntityFramework.Events.Entity;

namespace RoyalCode.Persistence.EntityFramework.Events.Services;

/// <summary>
/// <para>
///     Internal service for store domain events handled by the unit of work as <see cref="DomainEventDetails"/>.
/// </para>
/// </summary>
internal class StoreDomainEventAsDetails : IDomainEventProcessor
{
    /// <summary>
    /// <para>
    ///     Create a <see cref="DomainEventDetails"/> from the <paramref name="evt"/> and add to the 
    ///     <see cref="DbSet{TEntity}"/>.
    /// </para>
    /// </summary>
    /// <param name="db">The <see cref="DbContext"/> of the unit of work.</param>
    /// <param name="evt">The domain event handled by the unit of work.</param>
    public void ProcessEvent(DbContext db, IDomainEvent evt)
    {
        var entity = new DomainEventDetails(evt);
        db.Set<DomainEventDetails>().Add(entity);
    }

    /// <summary>
    /// <para>
    ///     Create a <see cref="DomainEventDetails"/> from the <paramref name="evt"/> and add to the 
    ///     <see cref="DbSet{TEntity}"/>.
    /// </para>
    /// </summary>
    /// <param name="db">The <see cref="DbContext"/> of the unit of work.</param>
    /// <param name="evt">The domain event handled by the unit of work.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Task for async operation.</returns>
    public async Task ProcessEventAsync(DbContext db, IDomainEvent evt, CancellationToken token)
    {
        var entity = new DomainEventDetails(evt);
        await db.Set<DomainEventDetails>().AddAsync(entity, token);
    }
}

