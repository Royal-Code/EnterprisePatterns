namespace RoyalCode.UnitOfWork;

/// <summary>
/// <para>
///     Represents a transaction with the database.
/// </para>
/// <para>
///     Normally the unit of work will manage a transaction during finalisation, 
///     however it is possible to manage the transaction at the application service level. 
///     There are cases where this is necessary, or preferable.
/// </para>
/// </summary>
public interface ITransaction
{
    /// <summary>
    /// <para>
    ///     Commits all changes made to the database in the current transaction.
    /// </para>
    /// </summary>
    void Commit();

    /// <summary>
    /// <para>
    ///     Discards all changes made to the database in the current transaction.
    /// </para>
    /// </summary>
    void Rollback();
    
    /// <summary>
    /// <para>
    ///     Commits all changes made to the database in the current transaction.
    /// </para>
    /// </summary>
    Task CommitAsync(CancellationToken ct);

    /// <summary>
    /// <para>
    ///     Discards all changes made to the database in the current transaction.
    /// </para>
    /// </summary>
    Task RollbackAsync(CancellationToken ct);
}
