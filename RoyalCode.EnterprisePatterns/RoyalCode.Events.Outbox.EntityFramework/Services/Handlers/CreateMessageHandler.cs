using Microsoft.EntityFrameworkCore;
using RoyalCode.Events.Outbox.Abstractions.Contracts;
using RoyalCode.Events.Outbox.Abstractions.Contracts.Handlers;
using RoyalCode.Events.Outbox.Abstractions.Models;
using RoyalCode.SmartProblems;

namespace RoyalCode.Events.Outbox.EntityFramework.Services.Handlers;

/// <inheritdoc />
public sealed class CreateMessageHandler : ICreateMessageHandler
{
    private readonly DbContext dbContext;

    /// <summary>
    /// Create a new handler with the <see cref="DbContext"/> with the configuration of the <see cref="OutboxMessage"/> entity;
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    public CreateMessageHandler(DbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public Result Handle(CreateMessage request)
    {
        if (request.HasProblems(out var problems))
            return problems;

        var entity = new OutboxMessage(request.MessageType, request.VersionType, request.Payload);
        dbContext.Set<OutboxMessage>().Add(entity);

        return Result.Ok();
    }
}
