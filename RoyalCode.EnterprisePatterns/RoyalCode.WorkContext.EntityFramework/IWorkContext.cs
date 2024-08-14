using Microsoft.EntityFrameworkCore;
using RoyalCode.WorkContext.Abstractions;

namespace RoyalCode.WorkContext.EntityFramework;

#pragma warning disable S2326 // TDbContext

/// <summary>
/// A <see cref='IWorkContext'/> that is implemented with <see cref='DbContext'/> of type <typeparamref name='TDbContext'/>.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public interface IWorkContext<TDbContext> : IWorkContext
    where TDbContext: DbContext
{ }
