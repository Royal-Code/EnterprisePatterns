using System.Net;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    protected internal readonly List<IResultMessage> messages = new();

    /// <summary>
    /// The result messages.
    /// </summary>
    [JsonIgnore]
    IEnumerable<IResultMessage> IOperationResult.Messages => messages;

    /// <summary>
    /// The result messages.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<IResultMessage>? Messages => messages.Count == 0 ? null : messages;

    /// <summary>
    /// Determines whether the result of the operation was success or failure.
    /// </summary>
    [JsonIgnore]
    public bool Success { get; internal protected set; }

    /// <summary>
    /// Determines whether the result of the operation was success or failure.
    /// </summary>
    [JsonIgnore] 
    public bool Failure => !Success;

    /// <summary>
    /// Count of the error messages of the result.
    /// </summary>
    [JsonIgnore]
    public int ErrorsCount => messages.Count;

    /// <summary>
    /// Default constructor, with success result, until some error message is added.
    /// </summary>
    public BaseResult()
    {
        Success = true;
    }

    /// <summary>
    /// Default constructor for deserialize.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="messages"></param>
    internal protected BaseResult(bool success, IEnumerable<IResultMessage> messages)
    {
        Success = success;
        this.messages.AddRange(messages);
    }

    /// <summary>
    /// Internal constructor for static methods factory.
    /// </summary>
    /// <param name="message">The message.</param>
    internal protected BaseResult(IResultMessage message)
    {
        Success = false;
        messages.Add(message);
    }

    /// <summary>
    /// Internal, for <see cref="ValueResult{TValue}"/>.
    /// </summary>
    /// <param name="other">Other result.</param>
    /// <param name="success">Value of property <see cref="Success"/>.</param>
    internal protected BaseResult(IOperationResult other, bool success)
    {
        Success = success;
        messages.AddRange(other.Messages);
    }

    /// <summary>
    /// Internal, for <see cref="ValueResult{TValue}"/>.
    /// </summary>
    /// <param name="other">Other result.</param>
    internal protected BaseResult(IOperationResult other)
    {
        Success = other.Success;
        messages.AddRange(other.Messages);
    }
    
    #region factory methods

    /// <summary>
    /// Creates a new operation result.
    /// </summary>
    /// <returns>New instance.</returns>
    public static BaseResult Create()
    {
        return new BaseResult();
    }

    /// <summary>
    /// Creates a new operation result with a failure message.
    /// </summary>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult Error(string? code, string text,
        string? property = null, HttpStatusCode? status = null, Exception? ex = null)
    {
        return new BaseResult(ResultMessage.Error(code, text, property, status, ex));
    }

    /// <summary>
    /// Creates a new operation result with a failure message.
    /// </summary>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult Error(string text, HttpStatusCode? status = null)
    {
        return new BaseResult(ResultMessage.Error(text, status));
    }

    /// <summary>
    /// Creates a new operation result with a failure message.
    /// </summary>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult Error(Exception ex, string? property = null, string? code = null, HttpStatusCode? status = null)
    {
        return new BaseResult(ResultMessage.Error(ex, property, code, status));
    }

    /// <summary>
    /// Creates a new operation result with a error message 
    /// with the code from <see cref="GenericErrorCodes.NotFound"/> 
    /// and HTTP status NotFound 404.
    /// </summary>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult NotFound(string text, string? property)
    {
        return new BaseResult(ResultMessage.NotFound(text, property));
    }

    /// <summary>
    /// Creates a new operation result with a error message with a specified code
    /// and HTTP status NotFound 404.
    /// </summary>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult NotFound(string code, string text, string? property)
    {
        return new BaseResult(ResultMessage.NotFound(code, text, property));
    }

    /// <summary>
    /// Creates a new operation result with a error message with a specified code and HTTP status Forbidden 403.
    /// </summary>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult Forbidden(string code, string text, string? property = null)
    {
        return new BaseResult(ResultMessage.Forbidden(code, text, property));
    }

    /// <summary>
    /// Creates a new operation result with a error message with a specified code and HTTP status Conflict 409.
    /// </summary>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult Conflict(string code, string text, string? property = null)
    {
        return new BaseResult(ResultMessage.Conflict(code, text, property));
    }

    /// <summary>
    /// Creates a new operation result with a error message 
    /// with the code from <see cref="GenericErrorCodes.InvalidParameters"/> 
    /// and HTTP status BadRequest 400.
    /// </summary>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The related property.</param>
    /// <returns>New instance.</returns>
    public static BaseResult InvalidParameters(string text, string property)
    {
        return new BaseResult(ResultMessage.InvalidParameters(text, property));
    }

    /// <summary>
    /// Creates a new operation result with a error message 
    /// with the code from <see cref="GenericErrorCodes.Validation"/> 
    /// and HTTP status UnprocessableEntity 422.
    /// </summary>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The related property.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult ValidationError(string text, string property, Exception? ex = null)
    {
        return new BaseResult(ResultMessage.ValidationError(text, property, ex));
    }

    /// <summary>
    /// Creates a new operation result with a error message
    /// with the specified code and HTTP status UnprocessableEntity 422.
    /// </summary>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The related property.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>New instance.</returns>
    public static BaseResult ValidationError(string code, string text, string property, Exception? ex = null)
    {
        return new BaseResult(ResultMessage.ValidationError(code, text, property, ex));
    }

    /// <summary>
    /// Creates a new operation result with a error message 
    /// with the code from <see cref="GenericErrorCodes.Validation"/> 
    /// and HTTP status UnprocessableEntity 422.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <returns>New instance.</returns>
    public static BaseResult ValidationError(Exception ex)
    {
        return new BaseResult(ResultMessage.ValidationError(ex));
    }

    /// <summary>
    /// Creates a new operation result with a error message 
    /// with the code from <see cref="GenericErrorCodes.ApplicationError"/>
    /// and HTTP status InternalServerError 500.
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
    /// Adds a message, and changes the result to failure if the message type is error.
    /// </summary>
    /// <param name="message">The message to be added.</param>
    public void Add(IResultMessage message)
    {
        Success = false;
        messages.Add(message);
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
        messages.AddRange(other.Messages);
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
            [JsonIgnore]
            public IEnumerable<IResultMessage> Messages => Array.Empty<IResultMessage>();

            /// <summary>
            /// abstract.
            /// </summary>
            [JsonIgnore]
            public abstract bool Success { get; }

            /// <inheritdoc />
            [JsonIgnore]
            public int ErrorsCount => 0;
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
            [JsonIgnore]
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
            [JsonIgnore]
            public override bool Success => false;
        }
    }
}
