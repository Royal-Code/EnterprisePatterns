
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
    public static ValueResult<TValue> CreateSuccess<TValue>(TValue value) => new(value);

    /// <summary>
    /// Creates a new failure operation result with the value of the operation and the error message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value of the operation.</param>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>Nova instância.</returns>
    public static ValueResult<TValue> CreateFailure<TValue>(TValue value, string text,
        string? property = null, string? code = null, Exception? ex = null)
        => new(value, ResultMessage.Error(text, property, code, ex));

    /// <summary>
    /// Creates a new failure operation result with the value of the operation and the error message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>Nova instância.</returns>
    public static ValueResult<TValue> CreateFailure<TValue>(string text,
        string? property = null, string? code = null, Exception? ex = null)
        => new(default, ResultMessage.Error(text, property, code, ex));

    /// <summary>
    /// Creates a new failure operation result with the value of the operation and the error message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value of the operation.</param>
    /// <param name="ex">The exception.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <returns>Nova instância.</returns>
    public static ValueResult<TValue> CreateFailure<TValue>(TValue value, 
        Exception ex, string? property = null, string? code = null)
        => new(value, ResultMessage.Error(ex, property, code));

    /// <summary>
    /// Creates a new failure operation result with the value of the operation and the error message.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="ex">The exception.</param>
    /// <param name="property">The property related, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <returns>Nova instância.</returns>
    public static ValueResult<TValue> CreateFailure<TValue>(
        Exception ex, string? property = null, string? code = null)
        => new(default, ResultMessage.Error(ex, property, code));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a failure message of type not found
    /// and with the message code <see cref="ResultErrorCodes.NotFound"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> NotFound<TValue>(string text) 
        => new(default, ResultMessage.NotFound(text));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a failure message of type forbidden
    /// and with the message code <see cref="ResultErrorCodes.Forbidden"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> Forbidden<TValue>(string text)
        => new(default, ResultMessage.Forbidden(text));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a failure message of type invalid parameters
    /// and with the message code <see cref="ResultErrorCodes.InvalidParameters"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> InvalidParameters<TValue>(string text, string property)
        => new(default, ResultMessage.InvalidParameters(text, property));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a failure message of type validation errors
    /// and with the message code <see cref="ResultErrorCodes.Validation"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="text">The error text that will be used in the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="ex">The exception, optional.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> ValidationError<TValue>(string text, string? property = null, Exception? ex = null)
        => new(default, ResultMessage.ValidationError(text, property, ex));

    /// <summary>
    /// Creates a new operation result with the value of the operation and with a failure message of type application error
    /// and with the message code <see cref="ResultErrorCodes.ApplicationError"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="text">The message text, optional, when not informed the exception message will be used.</param>
    /// <returns>New instance.</returns>
    public static ValueResult<TValue> ApplicationError<TValue>(Exception ex, string? text = null)
        => new(default, ResultMessage.ApplicationError(ex, text));
}

/// <summary>
/// The default implementation of <see cref="IOperationResult{TValue}"/>.
/// </summary>
public class ValueResult<TValue> : BaseResult, IOperationResult<TValue>
{
    /// <summary>
    /// The value returned by the operation.
    /// </summary>
    public TValue? Value { get; }

    #region factory methods

    #endregion

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
    public BaseResult ToBase() => CreateSuccess().Join(this);
}