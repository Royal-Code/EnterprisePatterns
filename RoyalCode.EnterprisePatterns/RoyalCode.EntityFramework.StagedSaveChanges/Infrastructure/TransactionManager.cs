using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.EntityFramework.StagedSaveChanges.Infrastructure;

/// <summary>
/// <para>
///     Internal component to manage two stage transaction of <see cref="DbContext"/>.
/// </para>
/// <para>
///     This class is designed to be a service, with the same lifetime of the <see cref="DbContext"/>.
/// </para>
/// </summary>
internal class TransactionManager : ITransactionManager, IResettableService
{
    private readonly DbContext db;
    private readonly IStagedSaveChangesInterceptor interceptor;

    private StagedContext stagedContext;

    /// <summary>
    /// Creates new unit of work transaction manager.
    /// </summary>
    /// <param name="db">The DbContext used by the unit of work.</param>
    public TransactionManager(DbContext db)
    {
        this.db = db;
        Init();
        
        interceptor = Interceptors.GetStagedSaveChangesInterceptor(db);
    }

    [MemberNotNull(nameof(stagedContext))]
    private void Init()
    {
        stagedContext = new StagedContext(db, this);
        Stage = TransactionStage.None;
    }

    /// <inheritdoc />
    public IDbContextTransaction? Transaction => db.Database.CurrentTransaction;

    /// <inheritdoc />
    public bool IsTransactionSupported => db.Database.ProviderName is not "Microsoft.EntityFrameworkCore.InMemory";

    /// <inheritdoc />
    public bool WillSaveChangesInTwoStages { get; private set; }

    /// <inheritdoc />
    public bool HasApplicationTransactionOpened { get; private set; }

    /// <inheritdoc />
    public TransactionStage Stage { get; private set; }

    /// <inheritdoc />
    public void RequireSaveChangesInTwoStages()
    {
        if (WillSaveChangesInTwoStages)
            return;

        WillSaveChangesInTwoStages = true;

        if (!IsTransactionSupported)
            return;

        if (Transaction is not null)
        {
            HasApplicationTransactionOpened = true;
            return;
        }

        WillSaveChangesInTwoStages = false;
        db.Database.BeginTransaction();
    }

    /// <inheritdoc />
    public async Task RequireSaveChangesInTwoStagesAsync(CancellationToken cancellationToken = default)
    {
        if (WillSaveChangesInTwoStages)
            return;

        WillSaveChangesInTwoStages = true;

        if (!IsTransactionSupported)
            return;

        if (Transaction is not null)
        {
            HasApplicationTransactionOpened = true;
            return;
        }

        WillSaveChangesInTwoStages = false;
        await db.Database.BeginTransactionAsync(cancellationToken);
    }

    public void Saving()
    {
        Stage = TransactionStage.FirstStage;
        interceptor.Saving(stagedContext);
    }

    public Task SavingAsync(CancellationToken cancellationToken = default)
    {
        Stage = TransactionStage.FirstStage;
        return interceptor.SavingAsync(stagedContext, cancellationToken);
    }

    public void Staged()
    {
        Stage = TransactionStage.SecondStage;
        interceptor.Staged(stagedContext);
    }

    public Task StagedAsync(CancellationToken cancellationToken = default)
    {
        Stage = TransactionStage.SecondStage;
        return interceptor.StagedAsync(stagedContext, cancellationToken);
    }
    
    public void Saved(int changes)
    {
        interceptor.Saved(stagedContext, changes);

        if (HasApplicationTransactionOpened || !WillSaveChangesInTwoStages)
            return;

        db.Database.CommitTransaction();
    }

    public async Task SavedAsync(int changes, CancellationToken cancellationToken = default)
    {
        await interceptor.SavedAsync(stagedContext, changes, cancellationToken);

        if (HasApplicationTransactionOpened || !WillSaveChangesInTwoStages)
            return;

        await db.Database.CommitTransactionAsync(cancellationToken);
    }

    public void Failed()
    {
        interceptor.Failed(stagedContext);

        if (HasApplicationTransactionOpened || Stage != TransactionStage.SecondStage)
            return;

        db.Database.RollbackTransaction();
    }
    
    public async Task FailedAsync(CancellationToken cancellationToken = default)
    {
        await interceptor.FailedAsync(stagedContext, cancellationToken);

        if (HasApplicationTransactionOpened || Stage != TransactionStage.SecondStage)
            return;

        await db.Database.RollbackTransactionAsync(cancellationToken);
    }

    public void ResetState()
    {
        Init();
    }

    public Task ResetStateAsync(CancellationToken cancellationToken = default)
    {
        Init();
        return Task.CompletedTask;
    }
}