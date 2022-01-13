
namespace RoyalCode.OperationResult;

/// <summary>
/// The default implementation of <see cref="IOperationResult{TValue}"/>.
/// </summary>
public class ValueResult<TValue> : BaseResult, IOperationResult<TValue>
{
    /// <summary>
    /// The value returned by the operation.
    /// </summary>
    public TValue? Value { get; private init; }

    /// <summary>
    /// Used by private static methods.
    /// </summary>
    private ValueResult() { }

    /// <summary>
    /// Default constructor for success, with the value returned by the operation.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    public ValueResult(TValue? value)
    {
        Value = value;
    }

    /// <summary>
    /// <para>
    ///     Create a new <see cref="ValueResult{TValue}"/> form other result (<see cref="IOperationResult"/>).
    /// </para>
    /// <para>
    ///     This result will fail because it does not contain the model.
    /// </para>
    /// </summary>
    /// <param name="other">Other result.</param>
    public ValueResult(IResult other) : base(other, false) { }

    /// <summary>
    /// <para>
    ///     Create a new <see cref="ValueResult{TValue}"/> form other result (<see cref="IOperationResult"/>).
    /// </para>
    /// <para>
    ///     Success will depend on the other result.
    /// </para>
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    /// <param name="other">Other result.</param>
    public ValueResult(TValue value, IResult other) : base(other)
    {
        Value = value;
    }
}