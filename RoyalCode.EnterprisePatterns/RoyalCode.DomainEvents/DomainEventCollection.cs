
using System.Collections;

namespace RoyalCode.DomainEvents;

/// <summary>
/// <para>
///     Default implementation of the <see cref="IDomainEventCollection"/>.
/// </para>
/// </summary>
public class DomainEventCollection : IDomainEventCollection
{
    private List<IDomainEvent>? innerList;
    private List<Action<IDomainEvent>>? observers;

    /// <summary>
    /// <para>
    ///     Internal list that stores the domain events.
    /// </para>
    /// </summary>
    protected List<IDomainEvent> InnerList => innerList ??= [];

    /// <inheritdoc />
    public int Count => innerList?.Count ?? 0;

    /// <summary>
    /// <para>
    ///     Always false, the collection will always be able to receive events.
    /// </para>
    /// </summary>
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public virtual void Add(IDomainEvent item)
    {
        InnerList.Add(item);
        Fire(item);
    }

    /// <inheritdoc />
    public void Clear() => innerList?.Clear();

    /// <inheritdoc />
    public bool Contains(IDomainEvent item) => innerList?.Contains(item) ?? false;

    /// <inheritdoc />
    public void CopyTo(IDomainEvent[] array, int arrayIndex) => InnerList.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public IEnumerator<IDomainEvent> GetEnumerator() => InnerList.GetEnumerator();

    /// <inheritdoc />
    public bool Remove(IDomainEvent item) => innerList?.Remove(item) ?? false;

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Observe(Action<IDomainEvent> observerAction)
    {
        if (observerAction is null)
            throw new ArgumentNullException(nameof(observerAction));

        observers ??= [];
        observers.Add(observerAction);

        innerList?.ForEach(observerAction);
    }

    /// <inheritdoc />
    public void RemoveObserver(Action<IDomainEvent> observerAction)
    {
        if (observerAction is null)
            throw new ArgumentNullException(nameof(observerAction));

        observers?.Remove(observerAction);
    }

    /// <summary>
    /// Fires the event to all observers.
    /// </summary>
    /// <param name="evt">The event.</param>
    protected virtual void Fire(IDomainEvent evt) =>observers?.ForEach(a => a(evt));
}
