
using System.Collections;

namespace RoyalCode.DomainEvents;

/// <summary>
/// <para>
///     Default implementation of the <see cref="IDomainEventCollection"/>.
/// </para>
/// </summary>
public class DomainEventCollection : IDomainEventCollection
{
    private List<IDomainEvent>? _innerList;
    private List<Action<IDomainEvent>>? _observers;

    /// <summary>
    /// <para>
    ///     Internal list that stores the domain events.
    /// </para>
    /// </summary>
    protected List<IDomainEvent> InnerList => _innerList ??= new List<IDomainEvent>();

    /// <inheritdoc />
    public int Count => _innerList?.Count ?? 0;

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
    public void Clear() => _innerList?.Clear();

    /// <inheritdoc />
    public bool Contains(IDomainEvent item) => _innerList?.Contains(item) ?? false;

    /// <inheritdoc />
    public void CopyTo(IDomainEvent[] array, int arrayIndex) => InnerList.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public IEnumerator<IDomainEvent> GetEnumerator() => InnerList.GetEnumerator();

    /// <inheritdoc />
    public bool Remove(IDomainEvent item) => _innerList?.Remove(item) ?? false;

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Observe(Action<IDomainEvent> observerAction)
    {
        if (observerAction is null)
            throw new ArgumentNullException(nameof(observerAction));

        _observers ??= new List<Action<IDomainEvent>>();
        _observers.Add(observerAction);

        _innerList?.ForEach(observerAction);
    }

    private void Fire(IDomainEvent evt) =>_observers?.ForEach(a => a(evt));
}
