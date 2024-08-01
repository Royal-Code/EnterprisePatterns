using Microsoft.EntityFrameworkCore;
using RoyalCode.Outbox.Abstractions.Contracts;
using RoyalCode.Outbox.Abstractions.Contracts.Handlers;
using RoyalCode.Outbox.Abstractions.Models;
using RoyalCode.SmartProblems;

namespace RoyalCode.Outbox.EntityFramework.Services.Handlers;

/// <inheritdoc />
/// <typeparam name="TDbContext">The <see cref="DbContext"/> type.</typeparam>
public sealed class RegisterConsumerHandler<TDbContext> : IRegisterConsumerHandler
    where TDbContext : DbContext
{
    private readonly TDbContext db;

    /// <summary>
    /// Creates a new instance of <see cref="RegisterConsumerHandler{TDbContext}"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    public RegisterConsumerHandler(TDbContext dbContext)
    {
        db = dbContext;
    }

    /// <inheritdoc />
    public async Task<Result> HandleAsync(RegisterConsumer request, CancellationToken ct)
    {
        if (request.HasProblems(out var problems))
            return problems;

        var existingConsumer = await db.Set<OutboxConsumer>()
            .FirstOrDefaultAsync(c => c.Name == request.ConsumerName, ct);

        if (existingConsumer != null)
            return Problems.InvalidParameter(string.Format(R.ConsumerAlreadyExists, request.ConsumerName))
                .With("consumer_name", request.ConsumerName);

        var consumer = new OutboxConsumer(request.ConsumerName);

        if (request.ConsumeFromLastMessage)
            consumer.LastConsumedMessageId = await db.Set<OutboxMessage>().MaxAsync(m => m.Id, ct);

        await db.Set<OutboxConsumer>().AddAsync(consumer, ct);

        await db.SaveChangesAsync(ct);

        return Result.Ok();
    }

    /// <inheritdoc />
    public async Task<bool> CanRegisterAsync(RegisterConsumer request, CancellationToken ct)
    {
        if (request.HasProblems(out _))
            return false;

        var existingConsumer = await db.Set<OutboxConsumer>()
            .FirstOrDefaultAsync(c => c.Name == request.ConsumerName, ct);

        return existingConsumer is null;
    }
}
