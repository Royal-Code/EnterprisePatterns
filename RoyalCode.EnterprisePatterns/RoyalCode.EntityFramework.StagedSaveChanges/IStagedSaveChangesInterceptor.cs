using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace RoyalCode.EntityFramework.StagedSaveChanges;

/// <summary>
/// <para>
///     This is an EntityFramework interceptor that will be invoked by the <see cref="ITransactionManager"/>.
/// </para>
/// <para>
///     In order for interceptors to work, they must be registered in the <see cref="DbContextOptionsBuilder"/>,
///     by the <see cref="DbContextOptionsBuilder.AddInterceptors(IEnumerable{IInterceptor})"/> method.
/// </para>
/// </summary>
public interface IStagedSaveChangesInterceptor : IInterceptor
{
    /// <summary>
    /// Invoked during the save operation of the DbContext.
    /// Its is before call the SaveChanges of the DbContext. 
    /// </summary>
    /// <param name="context">The shared context used by the transaction manager.</param>
    void Saving(StagedContext context);

    /// <summary>
    /// Invoked during the save operation of the DbContext.
    /// Its is before call the SaveChanges of the DbContext. 
    /// </summary>
    /// <param name="context">The shared context used by the transaction manager.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SavingAsync(StagedContext context, CancellationToken cancellationToken);

    /// <summary>
    /// <para>
    ///     Invoked during the save operation of the DbContext.
    /// </para>
    /// <para>
    ///     Is is called only when the save changes requires two stages,
    ///     and its is after the first call to the SaveChanges of the DbContext.
    /// </para> 
    /// </summary>
    /// <param name="context">The shared context used by the transaction manager.</param>
    void Staged(StagedContext context);

    /// <summary>
    /// <para>
    ///     Invoked during the save operation of the DbContext.
    /// </para>
    /// <para>
    ///     Is is called only when the save changes requires two stages,
    ///     and its is after the first call to the SaveChanges of the DbContext.
    /// </para> 
    /// </summary>
    /// <param name="context">The shared context used by the transaction manager.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StagedAsync(StagedContext context, CancellationToken cancellationToken);

    /// <summary>
    /// <para>
    ///     Invoked during the save operation of the DbContext.
    ///     Its is after call the SaveChanges of the DbContext.
    /// </para>
    /// <para>
    ///     When the save changes requires two stages, its called after the second SaveChanges.
    /// </para>
    /// </summary>
    /// <param name="context">The shared context used by the transaction manager.</param>
    /// <param name="changes">The number of state entries written to the database.</param>
    void Saved(StagedContext context, int changes);

    /// <summary>
    /// <para>
    ///     Invoked during the save operation of the DbContext.
    ///     Its is after call the SaveChanges of the DbContext.
    /// </para>
    /// <para>
    ///     When the save changes requires two stages, its called after the second SaveChanges.
    /// </para>
    /// </summary>
    /// <param name="context">The shared context used by the transaction manager.</param>
    /// <param name="changes">The number of state entries written to the database.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SavedAsync(StagedContext context, int changes, CancellationToken cancellationToken);

    /// <summary>
    /// <para>
    ///     Invoke during the save operation of the DbContext,
    ///     when the operation fail.
    /// </para>
    /// </summary>
    /// <param name="context">The shared context used by the transaction manager.</param>
    void Failed(StagedContext context);

    /// <summary>
    /// <para>
    ///     Invoke during the save operation of the DbContext,
    ///     when the operation fail.
    /// </para>
    /// </summary>
    /// <param name="context">The shared context used by the transaction manager.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task FailedAsync(StagedContext context, CancellationToken cancellationToken = default);
}