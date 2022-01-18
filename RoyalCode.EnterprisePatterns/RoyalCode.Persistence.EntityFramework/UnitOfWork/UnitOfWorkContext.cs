using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RoyalCode.UnitOfWork.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork;

/// <summary>
/// <para>
///     Implementação da unidade de trabalho utilizando EntityFrameworkCore.
/// </para> 
/// </summary>
/// <typeparam name="TDbContext">
/// <para>
///     Tipo do <see cref="DbContext"/> que contém as entidades mapeadas referentes ao contexto da unidade de trabalho.
/// </para>
/// </typeparam>
public class UnitOfWorkContext<TDbContext> : IUnitOfWorkContext, ITransaction
    where TDbContext : DbContext
{
    private IDbContextTransaction? dbContextTransaction;

    /// <summary>
    /// Construtor com a sessão para configurar o contexto de persistência e fornecer os serviços.
    /// </summary>
    /// <param name="db">Instância de <see cref="DbContext"/> para realizar as operações.</param>
    public UnitOfWorkContext(TDbContext db)
    {
        Db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Contexto de EF.
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
    public void CleanUp(bool force = true)
    {
        var entries = Db.ChangeTracker.Entries().Where(e => e.Entity != null);
        if (!force)
            entries = entries.Where(e => e.State == EntityState.Unchanged);
        foreach (var entry in entries)
            entry.State = EntityState.Detached;
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
    public ISaveResult Save()
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
    public async Task<ISaveResult> SaveAsync(CancellationToken token = default)
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
}
