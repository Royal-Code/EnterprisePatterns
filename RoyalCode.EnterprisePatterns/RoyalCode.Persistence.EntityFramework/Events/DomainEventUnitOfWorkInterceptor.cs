using Microsoft.EntityFrameworkCore.Infrastructure;
using RoyalCode.Persistence.EntityFramework.UnitOfWork;
using RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics;

namespace RoyalCode.Persistence.EntityFramework.Events;

/// <summary>
/// <para>
///     Interceptor for handle domain events for a unit of work.
/// </para>
/// </summary>
public class DomainEventUnitOfWorkInterceptor : IUnitOfWorkInterceptor
{
    /// <summary>
    /// Initialize a domain event handler for the unit of work.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    /// <exception cref="InvalidOperationException">
    ///     If the service <see cref="DomainEventHandlerFactory"/> is not found.
    /// </exception>
    public void Initializing(UnitOfWorkItems items)
    {
        var factory = items.Db.GetService<DomainEventHandlerFactory>();
        if (factory is null)
            throw new InvalidOperationException(DomainEventResources.DomainEventServiceNotFound);

        var handler = factory.Create(items.Db, items.TransactionManager);

        items.AddItem(handler);
    }

    /// <summary>
    /// Execute the required operations for handle the domain events when saving the unit of work.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    public void Saving(UnitOfWorkItems items)
    {
        var handler = items.GetItem<DomainEventHandler>();
        handler!.Saving(items.Db);
    }

    /// <summary>
    /// Execute the required operations for handle the domain events when saving the unit of work.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    /// <param name="cancellationToken"><inheritdoc /></param>
    public async Task SavingAsync(UnitOfWorkItems items, CancellationToken cancellationToken)
    {
        var handler = items.GetItem<DomainEventHandler>();
        await handler!.SavingAsync(items.Db, cancellationToken);
    }

    /// <summary>
    /// Execute the required operations for handle the domain events when the unit of work is staged.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    public void Staged(UnitOfWorkItems items)
    {
        var handler = items.GetItem<DomainEventHandler>();
        handler!.Staged(items.Db);
    }

    /// <summary>
    /// Execute the required operations for handle the domain events when the unit of work is staged.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    /// <param name="cancellationToken"><inheritdoc /></param>
    public async Task StagedAsync(UnitOfWorkItems items, CancellationToken cancellationToken)
    {
        var handler = items.GetItem<DomainEventHandler>();
        await handler!.StagedAsync(items.Db, cancellationToken);
    }

    /// <summary>
    /// Execute the required operations for handle the domain events after the end of the unit of work.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    /// <param name="changes"><inheritdoc /></param>
    public void Saved(UnitOfWorkItems items, int changes)
    {
        var handler = items.GetItem<DomainEventHandler>();
        handler!.Saved();
    }

    /// <summary>
    /// Execute the required operations for handle the domain events after the end of the unit of work.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    /// <param name="changes"><inheritdoc /></param>
    /// <param name="cancellationToken"><inheritdoc /></param>
    public async Task SavedAsync(UnitOfWorkItems items, int changes, CancellationToken cancellationToken)
    {
        var handler = items.GetItem<DomainEventHandler>();
        await handler!.SavedAsync(cancellationToken);
    }
}