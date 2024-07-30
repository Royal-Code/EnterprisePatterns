using RoyalCode.SmartProblems;
using RoyalCode.SmartValidations;
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.Outbox.Abstractions.Contracts;

/// <summary>
/// Request to register a consumer for the outbox.
/// </summary>
public sealed class RegisterConsumer
{
    /// <summary>
    /// Name of consumer.
    /// </summary>
    public required string ConsumerName { get; init; }

    /// <summary>
    /// Whether to consume messages from the last one.
    /// </summary>
    public bool ConsumeFromLastMessage { get; init; }

    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return Rules.Set<RegisterConsumer>()
            .NotEmpty(ConsumerName)
            .Length(ConsumerName, 3, 100)
            .HasProblems(out problems);
    }
}
