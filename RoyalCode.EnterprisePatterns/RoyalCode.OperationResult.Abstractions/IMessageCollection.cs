
namespace RoyalCode.OperationResult;

/// <summary>
/// A list of <see cref="IResultMessage"/>s
/// </summary>
public class MessageCollection : List<IResultMessage>
{
    /// <summary>
    /// Creates a new instance of <see cref="MessageCollection"/>
    /// </summary>
    public MessageCollection() : base (1) { }

    /// <summary>
    /// Adds a new message to the collection and returns the collection.
    /// </summary>
    /// <param name="message">The new message to add</param>
    /// <returns>The same instance of the collection</returns>
    public MessageCollection With(IResultMessage message)
    {
        Add(message);
        return this;
    }

    /// <summary>
    /// Adds a range of messages to the collection and returns the collection.
    /// </summary>
    /// <param name="messages">The messages to add</param>
    /// <returns>The same instance of the collection</returns>
    public MessageCollection With(IEnumerable<IResultMessage> messages)
    {
        AddRange(messages);
        return this;
    }
}