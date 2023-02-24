
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoyalCode.OperationResult;

/// <summary>
/// Method factories for <see cref="ValueResult{TValue}"/>.
/// </summary>
public static class ValueResult
{
    /// <summary>
    /// Creates a new success operation result with the value of the operation.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value of the operation.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> Create<TValue>(TValue value) => new(value);

    /// <summary>
    /// Creates a new failure operation result with the value of the operation and the error message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value of the operation.</param>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>Nova instância.</returns>
    public static ValueResult<TValue> Error<TValue>(TValue value, string? code, string text,
        string? property = null, HttpStatusCode? status = null, Exception? ex = null)
        => new(value, ResultMessage.Error(code, text, property, status, ex));

    /// <summary>
    /// Creates a new failure operation result with the value of the operation and the error message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>Nova instância.</returns>
    public static ValueResult<TValue> Error<TValue>(string? code, string text,
        string? property = null, HttpStatusCode? status = null, Exception? ex = null)
        => new(default, ResultMessage.Error(code, text, property, status, ex));

    /// <summary>
    /// Creates a new failure operation result with the value of the operation and the error message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value of the operation.</param>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <returns>Nova instância.</returns>
    public static ValueResult<TValue> Error<TValue>(TValue value, string text)
        => new(value, ResultMessage.Error(text));

    /// <summary>
    /// Creates a new failure operation result with the value of the operation and the error message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <returns>Nova instância.</returns>
    public static ValueResult<TValue> Error<TValue>(string text)
        => new(default, ResultMessage.Error(text));

    /// <summary>
    /// Creates a new failure operation result with the value of the operation and the error message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value of the operation.</param>
    /// <param name="ex">The exception.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <returns>Nova instância.</returns>
    public static ValueResult<TValue> Error<TValue>(TValue value,
        Exception ex, string? property = null, string? code = null, HttpStatusCode? status = null)
        => new(value, ResultMessage.Error(ex, property, code, status));

    /// <summary>
    /// Creates a new failure operation result with the value of the operation and the error message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="ex">The exception.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <returns>Nova instância.</returns>
    public static ValueResult<TValue> Error<TValue>(
        Exception ex, string? property = null, string? code = null, HttpStatusCode? status = null)
        => new(default, ResultMessage.Error(ex, property, code, status));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a error message 
    /// with the code from <see cref="GenericErrorCodes.NotFound"/>
    /// and HTTP status NotFound 404.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> NotFound<TValue>(string text, string? property)
        => new(default, ResultMessage.NotFound(text, property));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a error message with a specified code
    /// and HTTP status NotFound 404.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> NotFound<TValue>(string code, string text, string? property)
        => new(default, ResultMessage.NotFound(code, text, property));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a error message 
    /// with a specified code and HTTP status Forbidden 403.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> Forbidden<TValue>(string code, string text, string? property = null)
        => new(default, ResultMessage.Forbidden(code, text, property));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a error message 
    /// with a specified code and HTTP status Conflict 409.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> Conflict<TValue>(string code, string text, string? property = null)
        => new(default, ResultMessage.Conflict(code, text, property));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a error message 
    /// with the code from <see cref="GenericErrorCodes.InvalidParameters"/> 
    /// and HTTP status BadRequest 400.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The related property.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> InvalidParameters<TValue>(string text, string property)
        => new(default, ResultMessage.InvalidParameters(text, property));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a error message 
    /// with the code from <see cref="GenericErrorCodes.Validation"/> 
    /// and HTTP status UnprocessableEntity 422.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> ValidationError<TValue>(string text, string property, Exception? ex = null)
        => new(default, ResultMessage.ValidationError(text, property, ex));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a error message
    /// with the specified code and HTTP status UnprocessableEntity 422.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> ValidationError<TValue>(string code, string text, string property, Exception? ex = null)
        => new(default, ResultMessage.ValidationError(code, text, property, ex));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a error message 
    /// with the code from <see cref="GenericErrorCodes.Validation"/> 
    /// and HTTP status UnprocessableEntity 422.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> ValidationError<TValue>(Exception ex)
        => new(default, ResultMessage.ValidationError(ex));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a error message 
    /// with the code from <see cref="GenericErrorCodes.ApplicationError"/>
    /// and HTTP status InternalServerError 500.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="text">The message text, optional, when not informed the exception message will be used.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> ApplicationError<TValue>(Exception ex, string? text = null)
        => new(default, ResultMessage.ApplicationError(ex, text));

    /// <summary>
    /// Deserialize a json string to a <see cref="ValueResult{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="json">The json string.</param>
    /// <returns>The deserialized object.</returns>
    public static ValueResult<TValue> Deserialize<TValue>(string json)
    {
        var result = ResultsSerializeContext.Deserialize<TValue>(json);
        return result is null
            ? new ValueResult<TValue>(default, true, Enumerable.Empty<IResultMessage>())
            : new ValueResult<TValue>(result.Value, result.Success, result.Messages);
    }
}

/// <summary>
/// The default implementation of <see cref="IOperationResult{TValue}"/>.
/// </summary>
public class ValueResult<TValue> : BaseResult, IOperationResult<TValue>
{
    /// <summary>
    /// The value returned by the operation.
    /// </summary>
    [JsonIgnore]
    object? IResultHasValue.Value => Value;

    /// <summary>
    /// The value returned by the operation.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public TValue? Value { get; }

    /// <summary>
    /// Default constructor for success, with the value returned by the operation.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    public ValueResult(TValue? value)
    {
        Value = value;
        Success = value is not null;
    }

    /// <summary>
    /// Default constructor for deserealization.
    /// </summary>
    internal ValueResult(TValue? value, bool success, IEnumerable<IResultMessage> messages)
        : base(success, messages)
    {
        Value = value;
    }

    /// <summary>
    /// Internal constructor for static methods factory.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="value">The value returned by the operation.</param>
    internal ValueResult(TValue? value, IResultMessage message) : base(message)
    {
        Value = value;
        if (value is null)
            Success = false;
    }

    /// <summary>
    /// <para>
    ///     Create a new <see cref="ValueResult{TValue}"/> from other result (<see cref="IOperationResult"/>).
    /// </para>
    /// <para>
    ///     This result will fail because it does not contain the model (value).
    /// </para>
    /// </summary>
    /// <param name="other">Other result.</param>
    public ValueResult(IOperationResult other) : base(other, false) { }

    /// <summary>
    /// <para>
    ///     Create a new <see cref="ValueResult{TValue}"/> from other result (<see cref="IOperationResult"/>).
    /// </para>
    /// <para>
    ///     Success will depend on the other result.
    /// </para>
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    /// <param name="other">Other result.</param>
    public ValueResult(TValue value, IOperationResult other) : base(other)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        Value = value;
    }

    /// <summary>
    /// <para>
    ///     Joins the messages from the other result to this result.
    /// </para>
    /// </summary>
    /// <param name="other">The other result.</param>
    public new ValueResult<TValue> Join(IOperationResult other)
    {
        base.Join(other);
        return this;
    }

    /// <summary>
    /// Create a new operation result from this, with the same messages, and adapting the value.
    /// </summary>
    /// <typeparam name="TAdapted">The type of value to be adapted.</typeparam>
    /// <param name="adapter">The adaptor.</param>
    /// <returns>A new instance of <see cref="ValueResult{TValue}"/>.</returns>
    public ValueResult<TAdapted> AdaptTo<TAdapted>(Func<TValue, TAdapted> adapter)
    {
        if (adapter is null)
            throw new ArgumentNullException(nameof(adapter));

        TAdapted? newValue = Value == null ? default : adapter(Value);
        var newResult = new ValueResult<TAdapted>(newValue);
        return newResult.Join(this);
    }

    /// <summary>
    /// Create a new operation result from this, with the same messages, but without the value.
    /// </summary>
    /// <returns>A new instance of <see cref="BaseResult"/>.</returns>
    public BaseResult ToBase() => Create().Join(this);

    /// <summary>
    /// <para>
    ///     Serialize this instance to a JSON string.
    /// </para>
    /// </summary>
    /// <returns>The JSON string.</returns>
    public override string Serialize()
    {
        return JsonSerializer.Serialize(this, ResultsSerializeContext.JsonSerializerOptions);
    }
}