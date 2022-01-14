
namespace RoyalCode.OperationResult;

/// <summary>
/// The default implementation of <see cref="IOperationResult"/>.
/// </summary>
public class BaseResult : IOperationResult
{
    /// <summary>
    /// Instance of <see cref="Immutable.ImmutableSuccess"/>. Singleton.
    /// </summary>
    public static readonly IOperationResult ImmutableSuccess = new Immutable.ImmutableSuccess();

    /// <summary>
    /// Instance of <see cref="Immutable.ImmutableFailure"/>. Singleton.
    /// </summary>
    public static readonly IOperationResult ImmutableFailure = new Immutable.ImmutableFailure();

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
    public bool Success { get; internal protected set; }

    #region factory methods

    /// <summary>
    /// Creates a new operation result.
    /// </summary>
    /// <returns>New instance.</returns>
    public static BaseResult CreateSuccess()
    {
        return new BaseResult();
    }

    /// <summary>
    /// Creates a new operation result with a failure message.
    /// </summary>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult CreateFailure(string text, string? property = null, string? code = null, Exception? ex = null)
    {
        return new BaseResult(ResultMessage.Error(text, property, code, ex));
    }

    /// <summary>
    /// Creates a new operation result with a failure message.
    /// </summary>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult CreateFailure(Exception ex, string? property = null, string? code = null)
    {
        return new BaseResult(ResultMessage.Error(ex));
    }

    /// <summary>
    /// Creates a new operation result with a failure message of type not found
    /// and with the message code <see cref="ResultErrorCodes.NotFound"/>.
    /// </summary>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <returns>New instance.</returns>
    public static BaseResult NotFound(string text)
    {
        return new BaseResult(ResultMessage.NotFound(text));
    }

    /// <summary>
    /// Creates a new operation result with a failure message of type forbidden
    /// and with the message code <see cref="ResultErrorCodes.Forbidden"/>.
    /// </summary>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <returns>New instance.</returns>
    public static BaseResult Forbidden(string text)
    {
        return new BaseResult(ResultMessage.Forbidden(text));
    }

    /// <summary>
    /// Creates a new operation result with a failure message of type invalid parameters
    /// and with the message code <see cref="ResultErrorCodes.InvalidParameters"/>.
    /// </summary>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult InvalidParameters(string text, string property)
    {
        return new BaseResult(ResultMessage.InvalidParameters(text, property));
    }

    /// <summary>
    /// Creates a new operation result with a failure message of type validation errors
    /// and with the message code <see cref="ResultErrorCodes.Validation"/>.
    /// </summary>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult ValidationError(string text, string? property = null, Exception? ex = null)
    {
        return new BaseResult(ResultMessage.ValidationError(text, property, ex));
    }

    /// <summary>
    /// Creates a new operation result with a failure message of type application error
    /// and with the message code <see cref="ResultErrorCodes.ApplicationError"/>.
    /// </summary>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="text">The message text, optional, when not informed the exception message will be used.</param>
    /// <returns>New instance.</returns>
    public static BaseResult ApplicationError(Exception ex, string? text = null)
    {
        return new BaseResult(ResultMessage.ApplicationError(ex, text));
    }

    #endregion

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
    internal protected BaseResult(IOperationResult other, bool success)
    {
        Success = success;
        _messages.AddRange(other.Messages);
    }

    /// <summary>
    /// Internal, for <see cref="ValueResult{TValue}"/>.
    /// </summary>
    /// <param name="other">Other result.</param>
    internal protected BaseResult(IOperationResult other)
    {
        Success = other.Success;
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

    /// <summary>
    /// <para>
    ///     Joins the messages from the other result to this result.
    /// </para>
    /// </summary>
    /// <param name="other">The other result.</param>
    public virtual BaseResult Join(IOperationResult other)
    {
        Success = Success && other.Success;
        _messages.AddRange(other.Messages);
        return this;
    }

    private static class Immutable
    {
        /// <summary>
        /// <para>
        ///     Abstract immutable implementation of <see cref="IOperationResult"/>.
        /// </para>
        /// <para>
        ///     See also <see cref="ImmutableSuccess"/> e <see cref="ImmutableFailure"/>.
        /// </para>
        /// </summary>
        public abstract class ImmutableResult : IOperationResult
        {
            /// <summary>
            /// Always the same instace, an empty array.
            /// </summary>
            public IEnumerable<IResultMessage> Messages => Array.Empty<IResultMessage>();

            /// <summary>
            /// abstract.
            /// </summary>
            public abstract bool Success { get; }
        }

        /// <summary>
        /// <para>
        ///     Immutable implementation of <see cref="IOperationResult"/> where the success is true.
        /// </para>
        /// <para>
        ///     Usable via the <see cref="BaseResult.ImmutableSuccess"/>.
        /// </para>
        /// </summary>
        public sealed class ImmutableSuccess : ImmutableResult
        {
            /// <summary>
            /// Always true.
            /// </summary>
            public override bool Success => true;
        }

        /// <summary>
        /// <para>
        ///     Immutable implementation of <see cref="IOperationResult"/> where the success is false.
        /// </para>
        /// <para>
        ///     Usable via the <see cref="BaseResult.ImmutableFailure"/>.
        /// </para>
        /// </summary>
        public sealed class ImmutableFailure : ImmutableResult
        {
            /// <summary>
            /// Always false.
            /// </summary>
            public override bool Success => false;
        }
    }
}
