using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RoyalCode.UnitOfWork.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics;

/// <summary>
/// <para>
///     This is an EntityFramework interceptor that will be invoked on initialization of the <see cref="IUnitOfWorkContext"/>.
/// </para>
/// <para>
///     In order for interceptors to work, they must be registered in the <see cref="DbContextOptionsBuilder"/>,
///     by the <see cref="DbContextOptionsBuilder.AddInterceptors(System.Collections.Generic.IEnumerable{Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor})"/> method,
///     and the <see cref="UnitOfWorkDbContextOptionsBuilderExtensions.UseUnitOfWork"/> call included.
/// </para>
/// </summary>
public interface IUnitOfWorkInitializeInterceptor : IInterceptor
{
    /// <summary>
    /// Invoked during the initialisation of the work unit.
    /// </summary>
    /// <param name="context">The DbContext used by the unit of work.</param>
    void Initializing(DbContext context);
}