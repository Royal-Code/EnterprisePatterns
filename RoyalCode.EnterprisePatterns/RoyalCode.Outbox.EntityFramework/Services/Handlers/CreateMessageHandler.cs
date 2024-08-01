using Microsoft.EntityFrameworkCore;
using RoyalCode.Outbox.Abstractions.Contracts;
using RoyalCode.Outbox.Abstractions.Contracts.Handlers;
using RoyalCode.Outbox.Abstractions.Models;
using RoyalCode.SmartProblems;

namespace RoyalCode.Outbox.EntityFramework.Services.Handlers;

/// <inheritdoc />
/// <typeparam name="TDbContext">The <see cref="DbContext"/> type.</typeparam>
public sealed class CreateMessageHandler<TDbContext> : ICreateMessageHandler
    where TDbContext : DbContext
{
    private readonly TDbContext dbContext;

    /// <summary>
    /// Cria novo handler com o <see cref="DbContext"/> com a configuração da entidade <see cref="OutboxMessage"/>;
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    public CreateMessageHandler(TDbContext dbContext)
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
