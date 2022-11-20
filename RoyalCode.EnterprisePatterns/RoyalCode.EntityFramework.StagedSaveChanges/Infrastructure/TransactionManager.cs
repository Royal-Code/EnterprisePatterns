using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace RoyalCode.EntityFramework.StagedSaveChanges.Infrastructure;

/// <summary>
/// Internal component to handle transaction of <see cref="DbContext"/> for the unit of work.
/// </summary>
internal class TransactionManager : ITransactionManager
{
    private readonly DbContext db;

    /// <summary>
    /// Creates new unit of work transaction manager.
    /// </summary>
    /// <param name="db">The DbContext used by the unit of work.</param>
    public TransactionManager(DbContext db)
    {
        this.db = db;
    }

    /// <inheritdoc />
    public IDbContextTransaction? Transaction => db.Database.CurrentTransaction;

    /// <inheritdoc />
    public bool IsTransactionSupported => db.Database.ProviderName is not "Microsoft.EntityFrameworkCore.InMemory";

    /// <inheritdoc />
    public bool WillSaveChangesInTwoStages { get; private set; }

    /// <inheritdoc />
    public bool HasApplicationTransactionOpened { get; internal set; }

    /// <inheritdoc />
    public void RequireSaveChangesInTwoStages()
    {
        if (WillSaveChangesInTwoStages)
            return;

        WillSaveChangesInTwoStages = true;

        if (!IsTransactionSupported)
            return;

        if (Transaction is not null)
            return;

        db.Database.BeginTransaction();
    }

    internal void StagesCompleted()
    {
        if (HasApplicationTransactionOpened || !WillSaveChangesInTwoStages)
            return;

        db.Database.CommitTransaction();
    }

    /// <summary>
    /// <para>
    ///     Called by the unit of work after the save changes
    /// </para>
    /// </summary>
    internal async Task StagesCompletedAsync(CancellationToken token)
    {
        if (HasApplicationTransactionOpened || !WillSaveChangesInTwoStages)
            return;

        await db.Database.CommitTransactionAsync(token);
    }
}