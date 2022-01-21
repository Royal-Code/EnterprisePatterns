using Microsoft.EntityFrameworkCore;
using RoyalCode.EventDispatcher;
using RoyalCode.Persistence.EntityFramework.UnitOfWork;

namespace RoyalCode.Persistence.EntityFramework.Events;

/// <summary>
/// <para>
///     Internal service for create <see cref="DomainEventHandler"/> for each unit of work.
/// </para>
/// </summary>
public class DomainEventHandlerFactory
{
    private readonly IEventDispatcher dispatcher;

    /// <summary>
    /// Creates a new factory.
    /// </summary>
    /// <param name="dispatcher">The event dispathcer.</param>
    public DomainEventHandlerFactory(IEventDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    /// <summary>
    /// Creates a new <see cref="DomainEventHandler"/> for one unit of work.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/> used by the unit of work.</param>
    /// <param name="transactionManager">The unit of work transaction manager.</param>
    /// <returns>A new instance of <see cref="DomainEventHandler"/>.</returns>
    public DomainEventHandler Create(DbContext dbContext, ITransactionManager transactionManager)
    {
        var listener = new DomainEventHandler(dispatcher, transactionManager);
        dbContext.ChangeTracker.Tracked += listener.EntityTracked;

        return listener;
    }
}