
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
    public static ValueResult<TValue> CreateSuccess<TValue>(TValue value) => new ValueResult<TValue>(value);

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
        => new ValueResult<TValue>(value, ResultMessage.Error(text));
}

/// <summary>
/// The default implementation of <see cref="IOperationResult{TValue}"/>.
/// </summary>
public class ValueResult<TValue> : BaseResult, IOperationResult<TValue>
{
    /// <summary>
    /// The value returned by the operation.
    /// </summary>
    public TValue? Value { get; private set; }

    #region factory methods

    #endregion

    /// <summary>
    /// Default constructor for success, with the value returned by the operation.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    public ValueResult(TValue? value)
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