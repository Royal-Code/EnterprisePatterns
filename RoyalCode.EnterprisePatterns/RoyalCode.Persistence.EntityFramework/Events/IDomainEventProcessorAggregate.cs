
namespace RoyalCode.Persistence.EntityFramework.Events;

/// <summary>
/// <para>
///     Internal service to delegate process to <see cref="IDomainEventProcessor"/>.
/// </para>
/// </summary>
public interface IDomainEventProcessorAggregate : IDomainEventProcessor { }