
namespace RoyalCode.OperationResult;

/// <summary>
/// The default implementation of <see cref="IOperationResult"/>.
/// </summary>
public class BaseResult : IOperationResult
{
    /// <summary>
    /// Private list to store the messages.
    /// </summary>
    private readonly List<IResultMessage> _messages = new();

    /// <summary>
    /// The result messages.
    /// </summary>
    public IEnumerable<IResultMessage> Messages => _messages.AsReadOnly();

    /// <summary>
    /// Determines whether the result of the operation was success or failure.
    /// </summary>
    public bool Success { get; private set; }

    /// <summary>
    /// Default constructor, with success result, until some error message is added.
    /// </summary>
    public BaseResult()
    {
        Success = true;
    }

    /// <summary>
    /// Internal constructor for static methods factory.
    /// </summary>
    /// <param name="message">The message.</param>
    internal protected BaseResult(IResultMessage message)
    {
        Success = message.Type != ResultMessageType.Error;
        _messages.Add(message);
    }

    /// <summary>
    /// Internal, for <see cref="ValueResult{TValue}"/>.
    /// </summary>
    /// <param name="other">Other result.</param>
    /// <param name="success">Value of property <see cref="Success"/>.</param>
    internal protected BaseResult(IResult other, bool success)
    {
        Success = success;
        _messages.AddRange(other.Messages);
    }

    /// <summary>
    /// Adds a message, and changes the result to failure if the message type is error.
    /// </summary>
    /// <param name="message">The message to be added.</param>
    public void AddMessage(IResultMessage message)
    {
        Success = Success && message.Type != ResultMessageType.Error;
        _messages.Add(message);
    }
}
