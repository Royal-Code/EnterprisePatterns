using Microsoft.EntityFrameworkCore;

namespace RoyalCode.EntityFramework.StagedSaveChanges;

/// <summary>
/// <para>
///     A class that contains the shared items used during the save changes of the <see cref="DbContext"/>,
///     managed by the <see cref="ITransactionManager"/>.
/// </para>
/// </summary>
public class StagedContext
{
    private ICollection<object>? sharedItems;

    /// <summary>
    /// Creates a new instance of staged context.
    /// </summary>
    /// <param name="db">The DbContext.</param>
    /// <param name="transactionManager">The current transaction manager used with the DbContext.</param>
    public StagedContext(DbContext db, ITransactionManager transactionManager)
    {
        Db = db;
        TransactionManager = transactionManager;
    }

    /// <summary>
    /// The DbContext.
    /// </summary>
    public DbContext Db { get; }

    /// <summary>
    /// The transaction manager for the current save changes of the <see cref="DbContext"/>.
    /// </summary>
    public ITransactionManager TransactionManager { get; }

    /// <summary>
    /// Store some item to share in the current save changes of the <see cref="DbContext"/>.
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
    /// Retreive some item shared in the current save changes of the <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <returns>The item instance or null.</returns>
    public TItem? GetItem<TItem>()
        where TItem : class
    {
        return sharedItems?.OfType<TItem>().FirstOrDefault();
    }
}