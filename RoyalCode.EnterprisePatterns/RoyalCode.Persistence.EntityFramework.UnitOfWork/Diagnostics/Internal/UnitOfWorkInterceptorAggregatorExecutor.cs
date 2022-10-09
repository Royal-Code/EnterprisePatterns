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
    private static readonly IReadOnlyList<IUnitOfWorkInterceptor> emptyList = new List<IUnitOfWorkInterceptor>();

    private readonly IReadOnlyList<IUnitOfWorkInterceptor> interceptors;
    private readonly bool isEmpty;

    /// <summary>
    /// Create a new interceptor aggregate executor.
    /// </summary>
    /// <param name="interceptors">The interceptors to be executed.</param>
    public UnitOfWorkInterceptorAggregatorExecutor(IReadOnlyList<IInterceptor> interceptors)
    {
        isEmpty = interceptors.Count == 0;
        this.interceptors = isEmpty
            ? emptyList
            : interceptors.OfType<IUnitOfWorkInterceptor>().ToList();
    }

    /// <summary>
    /// Delegate the notification to each of the interceptors.
    /// </summary>
    /// <param name="items"><inheritdoc /></param>
    public void Initializing(UnitOfWorkItems items)
    {
        if (isEmpty)
            return;

        foreach (var i in interceptors)
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
        if (isEmpty)
            return;

        foreach (var i in interceptors)
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
        if (isEmpty)
            return;

        foreach (var i in interceptors)
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
        if (isEmpty)
            return;

        foreach (var i in interceptors)
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
        if (isEmpty)
            return;

        foreach (var i in interceptors)
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
        if (isEmpty)
            return;

        foreach (var i in interceptors)
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
    public async Task SavedAsync(UnitOfWorkItems items, int changes, CancellationToken cancellationToken)
    {
        if (isEmpty)
            return;

        foreach (var i in interceptors)
        {
            await i.SavedAsync(items, changes, cancellationToken);
        }
    }
}