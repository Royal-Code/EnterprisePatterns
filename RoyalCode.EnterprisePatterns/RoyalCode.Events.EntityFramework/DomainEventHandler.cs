using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RoyalCode.DomainEvents;
using RoyalCode.EventDispatcher;
using RoyalCode.Persistence.EntityFramework.Events.Exceptions;
using RoyalCode.UnitOfWork.EntityFramework;

namespace RoyalCode.Persistence.EntityFramework.Events;

/// <summary>
/// <para>
///     This class is responsible for handling domain events, 
///     getting them from entities, dispatching to listeners via IEventDispatcher, 
///     working with creation events, 
///     and invoking the event processor which may generate entities from the events.
/// </para>
/// </summary>
public class DomainEventHandler
{
    private Queue<IDomainEvent>? domainEvents;
    private Queue<IDomainEvent>? firedEvents;
    private Queue<ICreationEvent>? creationEvents;

    private readonly IEventDispatcher dispatcher;
    private readonly IDomainEventProcessorAggregate domainEventProcessor;
    private readonly ITransactionManager transactionManager;

    /// <summary>
    /// Creates a new instance of the event handler.
    /// </summary>
    /// <param name="dispatcher">The event dispatcher, to send the domain events to observers.</param>
    /// <param name="transactionManager">The unit of work transaction manager, to handle transactions for creation events.</param>
    /// <param name="domainEventProcessor">The domain event processor to process the domain events.</param>
    public DomainEventHandler(
        IEventDispatcher dispatcher,
        ITransactionManager transactionManager, 
        IDomainEventProcessorAggregate domainEventProcessor)
    {
        this.dispatcher = dispatcher;
        this.transactionManager = transactionManager;
        this.domainEventProcessor = domainEventProcessor;
    }

    /// <summary>
    /// Event handler of the <see cref="ChangeTracker"/>.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Entity entry of the change tracker.</param>
    public void EntityTracked(object? sender, EntityTrackedEventArgs e)
    {
        if (e.Entry.Entity is not IHasEvents hasEvents)
            return;

        hasEvents.DomainEvents ??= new DomainEventCollection();
        hasEvents.DomainEvents.Observe(EnqueueDomainEvent);
    }

    /// <summary>
    /// <para>
    ///     Event handling operation during the saving.
    /// </para>
    /// </summary>
    /// <param name="db">
    /// <para>
    ///     The current <see cref="DbContext"/> used by the unit of work.
    /// </para>
    /// </param>
    /// <exception cref="FireEventsAtSameScopeException">
    /// <para>
    ///     When an exception occurs during the sending of events to observers.
    /// </para>
    /// </exception>
    public void Saving(DbContext db)
    {
        try
        {
            FireEventsInCurrentScope(db);
        }
        catch (Exception ex)
        {
            throw new FireEventsAtSameScopeException(ex);
        }

        if (creationEvents is not null)
            transactionManager.RequireSaveChangesInTwoStages();
    }

    /// <summary>
    /// <para>
    ///     Event handling operation during the saving.
    /// </para>
    /// </summary>
    /// <param name="db">
    /// <para>
    ///     The current <see cref="DbContext"/> used by the unit of work.
    /// </para>
    /// </param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Task for async operation.</returns>
    /// <exception cref="FireEventsAtSameScopeException">
    /// <para>
    ///     When an exception occurs during the sending of events to observers.
    /// </para>
    /// </exception>
    public async Task SavingAsync(DbContext db, CancellationToken token)
    {
        try
        {
            await FireEventsInCurrentScopeAsync(db, token);
        }
        catch (Exception ex)
        {
            throw new FireEventsAtSameScopeException(ex);
        }

        if (creationEvents is not null)
            transactionManager.RequireSaveChangesInTwoStages();
    }

    /// <summary>
    /// <para>
    ///     Event handling operation during the staged saving.
    /// </para>
    /// <para>
    ///     The staged occurs when the save changes requires two stages.
    ///     The staged is after the first save changes.
    /// </para>
    /// </summary>
    /// <param name="db">
    /// <para>
    ///     The current <see cref="DbContext"/> used by the unit of work.
    /// </para>
    /// </param>
    public void Staged(DbContext db)
    {
        if (creationEvents is null)
            return;

        FireCreationEvents(db);
    }

    /// <summary>
    /// <para>
    ///     Event handling operation during the staged saving.
    /// </para>
    /// <para>
    ///     The staged occurs when the save changes requires two stages.
    ///     The staged is after the first save changes.
    /// </para>
    /// </summary>
    /// <param name="db">
    /// <para>
    ///     The current <see cref="DbContext"/> used by the unit of work.
    /// </para>
    /// </param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Task for async operation.</returns>
    public async Task StagedAsync(DbContext db, CancellationToken token)
    {
        if (creationEvents is null)
            return;

        await FireCreationEventsAsync(db, token);
    }

    /// <summary>
    /// <para>
    ///     Event handling operation after saved.
    /// </para>
    /// </summary>
    public void Saved()
    {
        if (firedEvents is null)
            return;

        var events = firedEvents.ToArray();
        firedEvents.Clear();

        foreach (var evt in events)
        {
            Dispatch(evt, DispatchStrategy.InSeparetedScope);
        }
    }

    /// <summary>
    /// <para>
    ///     Event handling operation after saved.
    /// </para>
    /// </summary>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Task for async operation.</returns>
    public async Task SavedAsync(CancellationToken token)
    {
        if (firedEvents is null)
            return;

        var events = firedEvents.ToArray();
        firedEvents.Clear();

        foreach (var evt in events)
        {
            await DispatchAsync(evt, DispatchStrategy.InSeparetedScope, token);
        }
    }

    /// <summary>
    /// <para>
    ///     Handler for listener the domain events of the <see cref="IDomainEventCollection"/>.
    /// </para>
    /// <para>
    ///     This handler will capture the events to dispatch at the end of the unit of work.
    /// </para>
    /// </summary>
    /// <param name="evt">The domain event.</param>
    private void EnqueueDomainEvent(IDomainEvent evt)
    {
        domainEvents ??= new Queue<IDomainEvent>();
        domainEvents.Enqueue(evt);
    }

    /// <summary>
    /// Dispatches domain events in the same scope and same transaction as the unit of work.
    /// </summary>
    private void FireEventsInCurrentScope(DbContext db)
    {
        if (domainEvents is null)
            return;

        firedEvents ??= new Queue<IDomainEvent>();

        while (domainEvents.Count > 0)
        {
            var evt = domainEvents.Dequeue();
            firedEvents.Enqueue(evt);
            Dispatch(evt, DispatchStrategy.InCurrentScope);

            if (evt is ICreationEvent creationEvent)
            {
                creationEvents ??= new();
                creationEvents.Enqueue(creationEvent);
            }
            else
            {
                domainEventProcessor.ProcessEvent(db, evt);
            }
        }
    }
    
    /// <summary>
    /// Dispatches domain events in the same scope and same transaction as the unit of work.
    /// </summary>
    private async Task FireEventsInCurrentScopeAsync(DbContext db, CancellationToken token)
    {
        if (domainEvents is null)
            return;

        firedEvents ??= new Queue<IDomainEvent>();

        while (domainEvents.Count > 0)
        {
            var evt = domainEvents.Dequeue();
            firedEvents.Enqueue(evt);
            await DispatchAsync(evt, DispatchStrategy.InCurrentScope, token);

            if (evt is ICreationEvent creationEvent)
            {
                creationEvents ??= new();
                creationEvents.Enqueue(creationEvent);
            }
            else
            {
                await domainEventProcessor.ProcessEventAsync(db, evt, token);
            }
        }
    }

    /// <summary>
    /// Dispatch the event.
    /// </summary>
    /// <param name="evt">Event to be dispatched.</param>
    /// <param name="strategy">Dispatch strategy.</param>
    private void Dispatch(IDomainEvent evt, DispatchStrategy strategy)
    {
        dispatcher.Dispatch(evt.GetType(), evt, strategy);
    }
    
    /// <summary>
    /// Dispatch the event.
    /// </summary>
    /// <param name="evt">Event to be dispatched.</param>
    /// <param name="strategy">Dispatch strategy.</param>
    /// <param name="token">Cancellation token.</param>
    private async Task DispatchAsync(IDomainEvent evt, DispatchStrategy strategy, CancellationToken token)
    {
        await dispatcher.DispatchAsync(evt.GetType(), evt, strategy, token);
    }

    /// <summary>
    /// Notifies creation events that the entity has been saved, and dispatches the events for processing.
    /// </summary>
    private void FireCreationEvents(DbContext db)
    {
        if (creationEvents is null)
            return;
        
        while (creationEvents.Count > 0)
        {
            var evt = creationEvents.Dequeue();
            evt.Saved();

            domainEventProcessor.ProcessEvent(db, (IDomainEvent)evt);
        }
    }
    
    /// <summary>
    /// Notifies creation events that the entity has been saved, and dispatches the events for processing.
    /// </summary>
    private async Task FireCreationEventsAsync(DbContext db, CancellationToken token)
    {
        if (creationEvents is null)
            return;
        
        while (creationEvents.Count > 0)
        {
            var evt = creationEvents.Dequeue();
            evt.Saved();

            await domainEventProcessor.ProcessEventAsync(db, (IDomainEvent)evt, token);
        }
    }
}