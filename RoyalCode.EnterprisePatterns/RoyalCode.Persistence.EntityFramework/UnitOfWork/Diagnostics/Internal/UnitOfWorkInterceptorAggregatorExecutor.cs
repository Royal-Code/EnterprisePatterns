using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork.Diagnostics.Internal;

/// <summary>
/// <para>
///     Internal class for extends the <see cref="DbContext"/>.
/// </para>
/// </summary>
public class UnitOfWorkInterceptorAggregatorExecutor : IUnitOfWorkInterceptor
{
    private readonly IReadOnlyList<IInterceptor> interceptors;

    /// <summary>
    /// Create a new interceptor aggregate executor.
    /// </summary>
    /// <param name="interceptors">The interceptors to be executed.</param>
    public UnitOfWorkInterceptorAggregatorExecutor(IReadOnlyList<IInterceptor> interceptors)
    {
        this.interceptors = interceptors;
    }

    /// <summary>
    /// Delegate the notification to each of the interceptors.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    public void Initializing(UnitOfWorkItems items)
    {
        foreach (var i in interceptors.OfType<IUnitOfWorkInterceptor>())
        {
            i.Initializing(items);
        }
    }

    /// <summary>
    /// Delegate the notification to each of the interceptors.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    public void Saving(UnitOfWorkItems items)
    {
        foreach (var i in interceptors.OfType<IUnitOfWorkInterceptor>())
        {
            i.Saving(items);
        }
    }

    /// <summary>
    /// Delegate the notification to each of the interceptors.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    /// <param name="cancellationToken"><inheritdoc /></param>
    public async Task SavingAsync(UnitOfWorkItems items, CancellationToken cancellationToken)
    {
        foreach (var i in interceptors.OfType<IUnitOfWorkInterceptor>())
        {
            await i.SavingAsync(items, cancellationToken);
        }
    }

    /// <summary>
    /// Delegate the notification to each of the interceptors.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    public void Staged(UnitOfWorkItems items)
    {
        foreach (var i in interceptors.OfType<IUnitOfWorkInterceptor>())
        {
            i.Staged(items);
        }
    }

    /// <summary>
    /// Delegate the notification to each of the interceptors.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    /// <param name="cancellationToken"><inheritdoc /></param>
    public async Task StagedAsync(UnitOfWorkItems items, CancellationToken cancellationToken)
    {
        foreach (var i in interceptors.OfType<IUnitOfWorkInterceptor>())
        {
            await i.StagedAsync(items, cancellationToken);
        }
    }

    /// <summary>
    /// Delegate the notification to each of the interceptors.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    /// <param name="changes"><inheritdoc /></param>
    public void Saved(UnitOfWorkItems items, int changes)
    {
        foreach (var i in interceptors.OfType<IUnitOfWorkInterceptor>())
        {
            i.Saved(items, changes);
        }
    }

    /// <summary>
    /// Delegate the notification to each of the interceptors.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    /// <param name="changes"><inheritdoc /></param>
    /// <param name="cancellationToken"><inheritdoc /></param>
    public async Task Savedasync(UnitOfWorkItems items, int changes, CancellationToken cancellationToken)
    {
        foreach (var i in interceptors.OfType<IUnitOfWorkInterceptor>())
        {
            await i.Savedasync(items, changes, cancellationToken);
        }
    }
}