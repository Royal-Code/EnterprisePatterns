using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoyalCode.Events.Outbox.Abstractions.Options;
using RoyalCode.Events.Outbox.Abstractions.Services;

namespace RoyalCode.Events.Outbox.EntityFramework.Services;

/// <summary>
/// A factory to create instances of <see cref="IOutboxService"/> with the <see cref="OutboxService"/> type
/// and a <see cref="DbContext"/>.
/// </summary>
/// <param name="options">The outbox options.</param>
/// <param name="dispatcher">The dispatcher of messagens.</param>
public sealed class OutboxServiceFactory(IOptions<OutboxOptions> options, IMessageDispatcher dispatcher)
{

    /// <summary>
    /// Creates a new <see cref="OutboxService"/> using the <paramref name="db"/>.
    /// </summary>
    /// <param name="db">The DbContext used to write the messages to the outbox entity.</param>
    /// <returns>A new <see cref="OutboxService"/></returns>
    public OutboxService CreateOutboxService(DbContext db) => new OutboxService(db, options, dispatcher);
}