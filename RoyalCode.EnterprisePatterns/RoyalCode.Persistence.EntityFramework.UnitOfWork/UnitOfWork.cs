using Microsoft.EntityFrameworkCore;
using RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics.Internal;
using RoyalCode.Persistence.EntityFramework.UnitOfWork.Interceptors;
using RoyalCode.UnitOfWork.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork;

/// <summary>
/// <para>
///     Implementation of the work unit using EntityFrameworkCore.
/// </para> 
/// </summary>
/// <typeparam name="TDbContext">
/// <para>
///     Type of the <see cref="DbContext"/> that contains the mapped entities referring to the work unit context.
/// </para>
/// </typeparam>
public class UnitOfWork<TDbContext> : IUnitOfWork, ITransaction
    where TDbContext : DbContext
{
    private readonly TransactionManager transactionManager;
    private readonly UnitOfWorkItems items;
    private readonly IUnitOfWorkInterceptor interceptor;

    /// <summary>
    /// Constructor with the <see cref="DbContext"/> used in the unit of work.
    /// </summary>
    /// <param name="db">The <see cref="DbContext"/> used in the unit of work.</param>
    public UnitOfWork(TDbContext db)
    {
        Db = db ?? throw new ArgumentNullException(nameof(db));

        transactionManager = new TransactionManager(db);
        items = new UnitOfWorkItems(db, transactionManager);

        interceptor = Interceptors<TDbContext>.GetUnitOfWorkInterceptor(db);

        interceptor.Initializing(items);
    }

    /// <summary>
    /// The EF Context.
    /// </summary>
    public TDbContext Db { get; }

    /// <inheritdoc/>
    public ITransaction BeginTransaction()
    {
        if (transactionManager.HasApplicationTransactionOpened)
            return this;

        Db.Database.BeginTransaction();
        transactionManager.HasApplicationTransactionOpened = true;

        return this;
    }

    /// <inheritdoc/>
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken token = default)
    {
        if (transactionManager.HasApplicationTransactionOpened)
            return this;

        await Db.Database.BeginTransactionAsync(token);
        transactionManager.HasApplicationTransactionOpened = true;

        return this;
    }

    /// <inheritdoc/>
    public void Commit()
    {
        if (!transactionManager.HasApplicationTransactionOpened || Db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("The transaction is not created");

        Db.Database.CommitTransaction();

        transactionManager.HasApplicationTransactionOpened = false;
    }

    /// <inheritdoc/>
    public void Rollback()
    {
        if (!transactionManager.HasApplicationTransactionOpened || Db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("The transaction is not created");

        Db.Database.RollbackTransaction();

        transactionManager.HasApplicationTransactionOpened = false;
    }

    /// <inheritdoc/>
    public async Task CommitAsync()
    {
        if (!transactionManager.HasApplicationTransactionOpened || Db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("The transaction is not created");

        await Db.Database.CommitTransactionAsync();

        transactionManager.HasApplicationTransactionOpened = false;
    }

    /// <inheritdoc/>
    public async Task RollbackAsync()
    {
        if (!transactionManager.HasApplicationTransactionOpened || Db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("The transaction is not created");

        await Db.Database.RollbackTransactionAsync();

        transactionManager.HasApplicationTransactionOpened = false;
    }

    /// <inheritdoc/>
    public SaveResult Save()
    {
        try
        {
            interceptor.Saving(items);

            var changes = Db.SaveChanges();

            if (transactionManager.WillSaveChangesInTwoStages)
            {
                interceptor.Staged(items);

                changes += Db.SaveChanges();
                
                transactionManager.StagesCompleted();
            }

            interceptor.Saved(items, changes);

            var result = new SaveResult(changes);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var cex = new ConcurrencyException(ex.Message, ex);

            throw cex;
        }
        catch (DbUpdateException ex)
        {
            return new SaveResult(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<SaveResult> SaveAsync(CancellationToken token = default)
    {
        try
        {
            await interceptor.SavingAsync(items, token);
            
            var changes = await Db.SaveChangesAsync(token);
            
            if (transactionManager.WillSaveChangesInTwoStages)
            {
                await interceptor.StagedAsync(items, token);

                changes += await Db.SaveChangesAsync(token);
                
                await transactionManager.StagesCompletedAsync(token);
            }

            await interceptor.SavedAsync(items, changes, token);
            
            var result = new SaveResult(changes);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var cex = new ConcurrencyException(ex.Message, ex);

            throw cex;
        }
        catch (DbUpdateException ex)
        {
            return new SaveResult(ex);
        }
    }

    /// <inheritdoc/>
    public void CleanUp(bool force = true)
    {
        var entries = Db.ChangeTracker.Entries().Where(e => e.Entity != null);
        if (!force)
            entries = entries.Where(e => e.State == EntityState.Unchanged);
        foreach (var entry in entries)
            entry.State = EntityState.Detached;
    }
}