using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using RoyalCode.DomainEvents;
using RoyalCode.EventDispatcher;
using RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics;

namespace RoyalCode.Persistence.EntityFramework.Events;

public class DomainEventListernerInitializer : IUnitOfWorkInitializeInterceptor
{
    public void Initializing(DbContext context)
    {

        var listener = context.GetService<DomainEventListerner>();
        if (listener is null)
            throw new InvalidOperationException(DomainEventResources.DomainEventServiceNotFound);

        context.ChangeTracker.Tracked += listener.EntityTracked;
        context.SavingChanges += listener.Saving;
        context.SavedChanges += listener.Saved;
    }
}

public class DomainEventListerner
{
    private Queue<IDomainEvent>? domainEvents;
    private Queue<IDomainEvent>? firedEvents;
    private Queue<ICreationEvent>? creationEvents;

    private TransactionManager? transactionManager;

    private IEventDispatcher dispatcher;

    public void EntityTracked(object? sender, EntityTrackedEventArgs e)
    {
        var entry = e.Entry;
        var entity = entry.Entity;
        if (entity is IHasEvents hasEvents)
        {
            if (hasEvents.DomainEvents is null)
                hasEvents.DomainEvents = new DomainEventCollection();

            hasEvents.DomainEvents.Observe(EnqueueDomainEvent);
        }

        // TODO: notificar outro serviço para converter o evento em entidade.
    }

    public void Saving(object? sender, SavingChangesEventArgs e)
    {
        if (sender is null)
            throw new ArgumentNullException(nameof(sender));

        var db = (DbContext)sender;

        try
        {
            FireEventsInCurrentScope();
        }
        catch (Exception ex)
        {
            //throw new FireEventsAtSameScopeException(
            //    "An error occurred while firing events at same scope, see the inner exception for more details.",
            //    ex);
        }

        // se há eventos de criação, é necessário gerenciar uma transaction
        if (creationEvents is not null && transactionManager is null)
        {
            try
            {
                // checar options para ver se deve ser criada transaction.

                transactionManager = db.Database.CurrentTransaction is null
                    ? db.Database.ProviderName is "Microsoft.EntityFrameworkCore.InMemory"
                        ? new TransactionManager()
                        : new TransactionManager(db.Database.BeginTransaction(), true)
                    : new TransactionManager(db.Database.CurrentTransaction, false);
            }
            catch (Exception)
            {
                transactionManager = new();
            }
        }

    }

    public void Saved(object? sender, SavedChangesEventArgs e)
    {
        if (sender is null)
            throw new ArgumentNullException(nameof(sender));

        var db = (DbContext)sender;

        if (transactionManager is not null)
        {
            var requireSave = FireCreationEvents();

            var changes = requireSave ? db.SaveChanges() : 0;

            if (transactionManager.IsInternal && transactionManager.IsSupported)
                transactionManager.Transaction.Commit();

            transactionManager = null;
        }

        // Dispara os eventos de domínio assíncronos.
        FireEventsInCurrentScope();
    }

    /// <summary>
    /// Adiciona um evento de domínio a fila de eventos a serem despachados.
    /// </summary>
    /// <param name="evt">Evento de domínio.</param>
    private void EnqueueDomainEvent(IDomainEvent evt)
    {
        if (domainEvents is null)
            domainEvents = new Queue<IDomainEvent>();

        domainEvents.Enqueue(evt);

        if (evt is ICreationEvent creationEvent)
        {
            if (creationEvents is null)
                creationEvents = new();
            creationEvents.Enqueue(creationEvent);
        }
    }

    /// <summary>
    /// Despacha os eventos de domínio no mesmo escopo da unidade de trabalho.
    /// </summary>
    private void FireEventsInCurrentScope()
    {
        if (domainEvents is null)
            return;

        if (firedEvents is null)
            firedEvents = new Queue<IDomainEvent>();

        while (domainEvents.Count > 0)
        {
            var evt = domainEvents.Dequeue();
            firedEvents.Enqueue(evt);
            Dispatch(evt, DispatchStrategy.InCurrentScope);
        }
    }

    /// <summary>
    /// Despacha os eventos de domínio em escopos diferentes da unidade de trabalho.
    /// </summary>
    private void FireEventsInSeparetedScope()
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
    /// Despacha um evento de domínio, segundo uma estratégia.
    /// </summary>
    /// <param name="evt">Evento a ser despachado.</param>
    /// <param name="strategy">Estratégia de despacho.</param>
    private void Dispatch(IDomainEvent evt, DispatchStrategy strategy)
    {
        dispatcher.Dispatch(evt.GetType(), evt, strategy);
    }

    /// <summary>
    /// Notifica os eventos de criação que a entidade foi salva, e despacha os eventos ao monitoramento.
    /// </summary>
    /// <returns>Verdadeiro se algum evento foi despachado ao monitoramento, falso caso contrário.</returns>
    private bool FireCreationEvents()
    {
        var monitored = false;

        if (creationEvents is not null)
        {
            while (creationEvents.Count > 0)
            {
                var evt = creationEvents.Dequeue();
                evt.Saved();

                // TODO: notificar outro serviço para converter o evento em entidade.
            }
        }

        return monitored;
    }
}

/// <summary>
/// Componente interno para controlar transações do <see cref="DbContext"/>.
/// </summary>
internal class TransactionManager
{
    /// <summary>
    /// Cria novo gerenciador de transações.
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="isInternal"></param>
    public TransactionManager(IDbContextTransaction transaction, bool isInternal)
    {
        Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        IsInternal = isInternal;
        IsSupported = true;
    }

    /// <summary>
    /// Cria novo gerenciador de transações para quando não é suportado a transação.
    /// </summary>
    public TransactionManager()
    {
        IsInternal = true;
        IsSupported = false;
    }

    /// <summary>
    /// A transaction utilizada.
    /// </summary>
    public IDbContextTransaction Transaction { get; private set; }

    /// <summary>
    /// Se a transaction é interna, criado e gerenciada pelo <see cref="PersistenceContext"/>.
    /// </summary>
    public bool IsInternal { get; private set; }

    /// <summary>
    /// Se o provider do EFCore é suportado.
    /// </summary>
    public bool IsSupported { get; private set; }
}