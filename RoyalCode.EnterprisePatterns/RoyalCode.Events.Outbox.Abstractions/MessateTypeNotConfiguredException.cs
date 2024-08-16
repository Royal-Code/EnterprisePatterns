namespace RoyalCode.Events.Outbox.Abstractions;

/// <summary>
/// Exception for when an event is not configured to be integrated into the Outbox.
/// </summary>
public sealed class MessateTypeNotConfiguredException(string messageType) 
    : InvalidOperationException($"The Type {messageType} has not been configured, you need to configure the type before writing or reading a message.")
{
    /// <summary>
    /// Creates a new exception.
    /// </summary>
    /// <param name="messageType">The message type.</param>
    public MessateTypeNotConfiguredException(Type messageType)
        : this(messageType.Name)
    { }
}
