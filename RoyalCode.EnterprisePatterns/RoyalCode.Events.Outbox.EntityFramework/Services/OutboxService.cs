using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoyalCode.Events.Outbox.Abstractions.Contracts.Handlers;
using RoyalCode.Events.Outbox.Abstractions.Options;
using RoyalCode.Events.Outbox.Abstractions.Services;
using RoyalCode.Events.Outbox.Abstractions.Services.Defaults;
using RoyalCode.Events.Outbox.EntityFramework.Services.Handlers;

namespace RoyalCode.Events.Outbox.EntityFramework.Services;

/// <summary>
/// Default implementation for <see cref="IOutboxService"/>.
/// </summary>
public sealed class OutboxService : OutboxServiceBase
{
    /// <summary>
    /// Creates a new instance of <see cref="OutboxService"/>.
    /// </summary>
    /// <param name="db">The DbContext used to write the messages to the outbox entity.</param>
    /// <param name="options">The outbox options.</param>
    /// <param name="dispatcher">The dispatcher of messagens.</param>
    public OutboxService(DbContext db, IOptions<OutboxOptions> options, IMessageDispatcher dispatcher) 
        : base(options, dispatcher)
    {
        CreateMessageHandler = new CreateMessageHandler(db);
    }

    /// <inheritdoc />
    protected override ICreateMessageHandler CreateMessageHandler { get; }
}
