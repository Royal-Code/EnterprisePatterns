using Microsoft.EntityFrameworkCore;
using RoyalCode.UnitOfWork.Abstractions;

namespace RoyalCode.UnitOfWork.EntityFramework;

#pragma warning disable S2326 // TDbContext

/// <summary>
/// A unit of work that will be implemented by the <see cref='DbContext'/> defined by <typeparamref name='TDbContext'/>.
/// </summary>
/// <typeparam name="TDbContext">The DbContext type.</typeparam>
public interface IUnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext: DbContext
{
    /// <summary>
    /// Gets the database context associated with the current operation.
    /// </summary>
    TDbContext Db { get; }
}
