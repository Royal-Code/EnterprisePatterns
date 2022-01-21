using Microsoft.EntityFrameworkCore.Storage;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork;

/// <summary>
/// <para>
///     Component to manage the unit of work transaction.
/// </para>
/// </summary>
public interface ITransactionManager
{
    /// <summary>
    /// <para>
    ///     Gets the current <see cref="IDbContextTransaction" /> being used by the context,
    ///     or null if no transaction is in use.
    /// </para>
    /// </summary>
    IDbContextTransaction? Transaction { get; }
    
    /// <summary>
    /// If the DbContext provider supports transactions.
    /// </summary>
    bool IsTransactionSupported { get; }
    
    /// <summary>
    /// <para>
    ///     Determines if the save changes must be applied in two stages.
    /// </para>
    /// <para>
    ///     This causes a transaction to be used when possible.
    /// </para>
    /// </summary>
    bool WillSaveChangesInTwoStages { get; }

    /// <summary>
    /// Determines if the application begin the transaction. 
    /// </summary>
    bool HasApplicationTransactionOpened { get; }
    
    /// <summary>
    /// <para>
    ///     It informs that save changes will need to be carried out in two stages.
    /// </para>
    /// <para>
    ///     If transactions are supported, a transaction will be initiated
    ///     if there is not already a transaction initiated by the application.
    /// </para>
    /// </summary>
    void RequireSaveChangesInTwoStages();
}