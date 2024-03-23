using Microsoft.EntityFrameworkCore;

namespace RoyalCode.UnitOfWork.EntityFramework.Interceptors;

/// <summary>
/// Items used and shared during a unit of work.
/// </summary>
public class UnitOfWorkItems
{
    private ICollection<object>? sharedItems;

    /// <summary>
    /// Creates a new instance of items.
    /// </summary>
    /// <param name="db">The unit of work DbContext.</param>
    /// <param name="transactionManager">The unit of work transaction manager.</param>
    public UnitOfWorkItems(DbContext db, ITransactionManager transactionManager)
    {
        Db = db;
        TransactionManager = transactionManager;
    }

    /// <summary>
    /// The unit of work DbContext.
    /// </summary>
    public DbContext Db { get; }

    /// <summary>
    /// The unit of work transaction manager.
    /// </summary>
    public ITransactionManager TransactionManager { get; }

    /// <summary>
    /// Store some item to share in the current unit of work.
    /// </summary>
    /// <param name="item">Some item to share.</param>
    /// <typeparam name="TItem">The item type.</typeparam>
    public void AddItem<TItem>(TItem item)
        where TItem : class
    {
        sharedItems ??= new List<object>();

        sharedItems.Add(item);
    }

    /// <summary>
    /// Retreive some item shared in the current unit of work.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <returns>The item instance or null.</returns>
    public TItem? GetItem<TItem>()
        where TItem : class
    {
        return sharedItems?.OfType<TItem>().FirstOrDefault();
    }
}