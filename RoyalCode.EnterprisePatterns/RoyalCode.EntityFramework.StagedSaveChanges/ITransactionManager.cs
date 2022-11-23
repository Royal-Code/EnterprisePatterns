using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace RoyalCode.EntityFramework.StagedSaveChanges;

/// <summary>
/// <para>
///     Component to manage the unit of work transaction.
/// </para>
/// <para>
///     This component is designed to manage a two stage save changes of the <see cref="DbContext"/>,
///     notifying the <see cref="IStagedSaveChangesInterceptor"/> for each stage.
/// </para>
/// </summary>
public interface ITransactionManager
{
    /// <summary>
    /// The transaction stage.
    /// </summary>
    TransactionStage Stage { get; }

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

    /// <summary>
    /// <para>
    ///     It informs that the first stage of save changes has been started.
    /// </para>
    /// </summary>
    /// <param name="db">The current DbContext.</param>
    void Saving(DbContext db);

    /// <summary>
    /// <para>
    ///     It informs that the second stage of save changes has been started.
    /// </para>
    /// </summary>
    /// <param name="db">The current DbContext.</param>
    void Staged(DbContext db);

    /// <summary>
    /// <para>
    ///     It informs that the save changes has been completed.
    /// </para>
    /// </summary>
    /// <param name="context">The current DbContext.</param>
    /// <param name="changes">The number of changes.</param>
    void Saved(DbContext context, int changes);

    /// <summary>
    /// <para>
    ///     It informs that the operation has been failed.
    /// </para>
    /// </summary>
    /// <param name="context">The current DbContext.</param>
    void Failed(DbContext context);

    /// <summary>
    /// <para>
    ///     It informs that the first stage of save changes has been started.
    /// </para>
    /// </summary>
    /// <param name="context">The current DbContext.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SavingAsync(DbContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// <para>
    ///     It informs that the second stage of save changes has been started.
    /// </para>
    /// </summary>
    /// <param name="context">The current DbContext.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StagedAsync(DbContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// <para>
    ///     It informs that the save changes has been completed.
    /// </para>
    /// </summary>
    /// <param name="context">The current DbContext.</param>
    /// <param name="changes">The number of changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SavedAsync(DbContext context, int changes, CancellationToken cancellationToken = default);

    /// <summary>
    /// <para>
    ///     It informs that the operation has been failed.
    /// </para>
    /// </summary>
    /// <param name="context">The current DbContext.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task FailedAsync(DbContext context, CancellationToken cancellationToken = default);
}
