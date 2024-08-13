using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RoyalCode.UnitOfWork.Abstractions;
using RoyalCode.UnitOfWork.EntityFramework.Diagnostics.Internal;
using RoyalCode.UnitOfWork.EntityFramework.Interceptors;

namespace RoyalCode.UnitOfWork.EntityFramework;

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
    private IDbContextTransaction? dbContextTransaction;

    /// <summary>
    /// Constructor with the <see cref="DbContext"/> used in the unit of work.
    /// </summary>
    /// <param name="db">The <see cref="DbContext"/> used in the unit of work.</param>
    public UnitOfWork(TDbContext db)
    {
        Db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// The EF Context.
    /// </summary>
    public TDbContext Db { get; }

    /// <inheritdoc/>
    public ITransaction BeginTransaction()
    {
        dbContextTransaction ??= Db.Database.BeginTransaction();
        return this;
    }

    /// <inheritdoc/>
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken token = default)
    {
        dbContextTransaction ??= await Db.Database.BeginTransactionAsync(token);
        return this;
    }

    /// <inheritdoc/>
    public void Commit()
    {
        if (dbContextTransaction is null)
            throw new InvalidOperationException("The transaction is not created");

        dbContextTransaction.Commit();
        dbContextTransaction = null;
    }

    /// <inheritdoc/>
    public void Rollback()
    {
        if (dbContextTransaction is null)
            throw new InvalidOperationException("The transaction is not created");

        dbContextTransaction.Rollback();
        dbContextTransaction = null;
    }

    /// <inheritdoc/>
    public async Task CommitAsync(CancellationToken ct)
    {
        if (dbContextTransaction is null)
            throw new InvalidOperationException("The transaction is not created");

        await dbContextTransaction.CommitAsync(ct);
        dbContextTransaction = null;
    }

    /// <inheritdoc/>
    public async Task RollbackAsync(CancellationToken ct)
    {
        if (dbContextTransaction is null)
            throw new InvalidOperationException("The transaction is not created");

        await dbContextTransaction.RollbackAsync(ct);
        dbContextTransaction = null;
    }

    /// <inheritdoc/>
    public SaveResult Save()
    {
        try
        {
            var changes = Db.SaveChanges();
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
            var changes = await Db.SaveChangesAsync(token);
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