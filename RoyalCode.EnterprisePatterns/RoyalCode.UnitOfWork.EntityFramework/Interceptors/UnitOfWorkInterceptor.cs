namespace RoyalCode.UnitOfWork.EntityFramework.Interceptors;

/// <summary>
/// <para>
///     Base implementation for <see cref="IUnitOfWorkInterceptor"/>.
/// </para>
/// </summary>
public abstract class UnitOfWorkInterceptor : IUnitOfWorkInterceptor
{
    /// <inheritdoc />
    public virtual void Initializing(UnitOfWorkItems items) { }

    /// <inheritdoc />
    public virtual void Saving(UnitOfWorkItems items) { }

    /// <inheritdoc />
    public virtual Task SavingAsync(UnitOfWorkItems items, CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public virtual void Staged(UnitOfWorkItems items) { }

    /// <inheritdoc />
    public virtual Task StagedAsync(UnitOfWorkItems items, CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public virtual void Saved(UnitOfWorkItems items, int changes) { }

    /// <inheritdoc />
    public virtual Task SavedAsync(UnitOfWorkItems items, int changes, CancellationToken cancellationToken) => Task.CompletedTask;
}