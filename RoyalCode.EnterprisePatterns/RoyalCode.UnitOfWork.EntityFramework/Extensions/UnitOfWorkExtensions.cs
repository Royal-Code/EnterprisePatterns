using Microsoft.EntityFrameworkCore;
using RoyalCode.UnitOfWork.EntityFramework;

namespace RoyalCode.UnitOfWork.Abstractions;

/// <summary>
/// Provides extension methods for working with <see cref="IUnitOfWork"/> instances.
/// </summary>
public static class UnitOfWorkExtensions
{

    /// <summary>
    /// <para>
    ///     Unwraps the underlying <see cref="DbContext"/> from an <see cref="IUnitOfWork"/> instance.
    /// </para>
    /// <para>
    ///     Must be used when the <see cref="IUnitOfWork"/> is implemented by a specific <see cref="DbContext"/> type,
    ///     such as <see cref="IUnitOfWork{TDbContext}"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> to unwrap.</typeparam>
    /// <param name="unitOfWork">The unit of work instance.</param>
    /// <returns>The unwrapped <see cref="DbContext"/> instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// <para>
    ///     Thrown when the <paramref name="unitOfWork"/> does not implement <see cref="IUnitOfWork{TDbContext}"/>.
    /// </para>
    /// </exception>
    public static DbContext Unwrap<TDbContext>(this IUnitOfWork unitOfWork)
        where TDbContext : DbContext
    {
        if (unitOfWork is IUnitOfWork<TDbContext> uow)
        {
            return uow.Db;
        }

        throw new InvalidOperationException(
            $"The unit of work does not implement {nameof(IUnitOfWork<TDbContext>)}. " +
            $"Expected type: {typeof(IUnitOfWork<TDbContext>).FullName}, " +
            $"actual type: {unitOfWork.GetType().FullName}");
    }
}
