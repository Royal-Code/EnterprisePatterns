using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoyalCode.Events.Outbox.Abstractions;
using RoyalCode.Events.Outbox.Abstractions.Models;
using RoyalCode.Events.Outbox.Abstractions.Options;
using System.Text.Json;

namespace RoyalCode.Events.Outbox.EntityFramework.Services;

/// <summary>
/// Utility for working with Outbox events.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public sealed class EventsUtils<TDbContext>
    where TDbContext: DbContext
{
    private readonly OutboxOptions options;
    private readonly TDbContext dbContext;

    /// <summary>
    /// Creates a new instance of <see cref="EventsUtils{TDbContext}"/>.
    /// </summary>
    /// <param name="dbContext">The DbContext.</param>
    /// <param name="options">The outbox options.</param>
    public EventsUtils(
        TDbContext dbContext,
        IOptions<OutboxOptions> options)
    {
        this.options = options.Value;
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Gets all the outbox events of the type specified by <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <returns>All events from the outbox.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If <typeparamref name="TEvent"/> not configured.
    /// </exception>
    public IEnumerable<TEvent> GetAll<TEvent>()
    {
        if (!options.TryGetMetadata<TEvent>(out var metadata))
            throw new MessateTypeNotConfiguredException(typeof(TEvent));

        return dbContext.Set<OutboxMessage>()
            .Where(m => m.MessageType == metadata.TypeName)
            .Select(m => m.Payload)
            .AsEnumerable()
            .Select(payload =>
            {
                return (TEvent) (metadata.JsonTypeInfo is not null
                    ? JsonSerializer.Deserialize(payload, metadata.JsonTypeInfo)!
                    : JsonSerializer.Deserialize(payload, metadata.PayloadType, metadata.SerializerOptions ?? options.SerializerOptions)!);
            })
            .ToList();
    }

    /// <summary>
    /// Gets the last outbox event of a specific type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <returns>The last event of the type <typeparamref name="TEvent"/> from the outbox.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If <typeparamref name="TEvent"/> not configured.
    /// </exception>
    public TEvent? GetLast<TEvent>()
    {
        var all = GetAll<TEvent>();
        return all.LastOrDefault();
    }
}
