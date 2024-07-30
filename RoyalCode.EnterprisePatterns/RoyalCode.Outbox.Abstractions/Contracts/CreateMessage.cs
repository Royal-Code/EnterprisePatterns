
using RoyalCode.SmartProblems;
using RoyalCode.SmartValidations;
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.Outbox.Abstractions.Contracts;

/// <summary>
/// Request to create a message.
/// </summary>
public sealed class CreateMessage
{
    /// <summary>
    /// Type of message.
    /// </summary>
    public required string MessageType { get; init; }

    /// <summary>
    /// Message type version.
    /// </summary>
    public required int VersionType { get; init; }

    /// <summary>
    /// Optional, the key that identifies the message, depending on the type of message.
    /// </summary>
    public string? Key { get; init; }

    /// <summary>
    /// Message payload.
    /// </summary>
    public required string Payload { get; init; }

    /// <summary>
    /// Validates the properties of this object.
    /// </summary>
    /// <param name="problems">The problems encountered, if any.</param>
    /// <returns>
    ///     True if there are problems, i.e. the object has invalid properties, false if it is valid.
    /// </returns>
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return Rules.Set<CreateMessage>()
            .NotEmpty(MessageType)
            .Min(VersionType, 1)
            .NullOrNotEmpty(Key)
            .NotEmpty(Payload)
            .HasProblems(out problems);
    }
}
