using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RoyalCode.UnitOfWork.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork.Interceptors;

/// <summary>
/// <para>
///     This is an EntityFramework interceptor that will be invoked by the <see cref="IUnitOfWorkContext"/>.
/// </para>
/// <para>
///     In order for interceptors to work, they must be registered in the <see cref="DbContextOptionsBuilder"/>,
///     by the <see cref="DbContextOptionsBuilder.AddInterceptors(IEnumerable{IInterceptor})"/> method,
///     and the <see cref="UnitOfWorkDbContextOptionsBuilderExtensions.UseUnitOfWork"/> call included.
/// </para>
/// </summary>
public interface IUnitOfWorkInterceptor : IInterceptor
{
    /// <summary>
    /// Invoked during the initialisation of the work unit.
    /// </summary>
    /// <param name="items">The shared items used by the unit of work.</param>
    void Initializing(UnitOfWorkItems items);

    /// <summary>
    /// Invoked during the save operation of the work unit.
    /// Its is before call the SaveChanges of the DbContext. 
    /// </summary>
    /// <param name="items">The shared items used by the unit of work.</param>
    void Saving(UnitOfWorkItems items);

    /// <summary>
    /// Invoked during the save operation of the work unit.
    /// Its is before call the SaveChanges of the DbContext. 
    /// </summary>
    /// <param name="items">The shared items used by the unit of work.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SavingAsync(UnitOfWorkItems items, CancellationToken cancellationToken);

    /// <summary>
    /// <para>
    ///     Invoked during the save operation of the work unit.
    /// </para>
    /// <para>
    ///     Is is called only when the save changes requires two stages,
    ///     and its is after the first call to the SaveChanges of the DbContext.
    /// </para> 
    /// </summary>
    /// <param name="items">The shared items used by the unit of work.</param>
    void Staged(UnitOfWorkItems items);

    /// <summary>
    /// <para>
    ///     Invoked during the save operation of the work unit.
    /// </para>
    /// <para>
    ///     Is is called only when the save changes requires two stages,
    ///     and its is after the first call to the SaveChanges of the DbContext.
    /// </para> 
    /// </summary>
    /// <param name="items">The shared items used by the unit of work.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StagedAsync(UnitOfWorkItems items, CancellationToken cancellationToken);

    /// <summary>
    /// <para>
    ///     Invoked during the save operation of the work unit.
    ///     Its is after call the SaveChanges of the DbContext.
    /// </para>
    /// <para>
    ///     When the save changes requires two stages, its called after the second SaveChanges.
    /// </para>
    /// </summary>
    /// <param name="items">The shared items used by the unit of work.</param>
    /// <param name="changes">The number of state entries written to the database.</param>
    void Saved(UnitOfWorkItems items, int changes);

    /// <summary>
    /// <para>
    ///     Invoked during the save operation of the work unit.
    ///     Its is after call the SaveChanges of the DbContext.
    /// </para>
    /// <para>
    ///     When the save changes requires two stages, its called after the second SaveChanges.
    /// </para>
    /// </summary>
    /// <param name="items">The shared items used by the unit of work.</param>
    /// <param name="changes">The number of state entries written to the database.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SavedAsync(UnitOfWorkItems items, int changes, CancellationToken cancellationToken);
}