using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RoyalCode.OperationResults;

/// <summary>
/// <para>
///     A validable result represents the result of an operation validation that can be either successful or a failure.
/// </para>
/// </summary>
public readonly struct ValidableResult
{
    /// <summary>
    /// Adds a new message to the result collection if the result is a failure.
    /// </summary>
    /// <param name="result">The result to add the message to</param>
    /// <param name="message">The new message to add</param>
    /// <returns>The same instance of the collection</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValidableResult operator +(ValidableResult result, IResultMessage message)
    {
        if (!result.Failure)
            return new ValidableResult(new ResultsCollection().With(message));

        result.error.Add(message);
        return result;
    }

    private readonly ResultsCollection? error;

    /// <summary>
    /// Creates a successful operation result.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValidableResult()
    {
        error = default;
    }

    /// <summary>
    /// Creates a failure operation result.
    /// </summary>
    /// <param name="error">The error of the operation result.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValidableResult(ResultsCollection error)
    {
        this.error = error;
    }

    /// <summary>
    /// <para>
    ///     Whether the operation result is successful.
    /// </para>
    /// <para>
    ///     When the operation result is successful, the value is not <see langword="null"/>,
    ///     and the error is <see langword="null"/>.
    /// </para>
    /// </summary>
#if NET6_0_OR_GREATER
    [MemberNotNullWhen(false, nameof(error))]
#endif
    public readonly bool Success { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => error is null; }

    /// <summary>
    /// <para>
    ///     Whether the operation result is a failure.
    /// </para>
    /// <para>
    ///     When the operation result is a failure, the value is <see langword="null"/>,
    ///     and the error is not <see langword="null"/>.
    /// </para>
    /// </summary>
#if NET6_0_OR_GREATER
    [MemberNotNullWhen(true, nameof(error))]
#endif
    public readonly bool Failure { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => error is not null; }

    /// <summary>
    /// <para>
    ///     Check if the operation result is failure, 
    ///     then return true and set the error of the operation result.
    /// </para>
    /// </summary>
    /// <param name="error">The error of the operation result.</param>
    /// <returns>Whether the operation result is failure.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool TryGetError([NotNullWhen(true)] out ResultsCollection? error)
    {
        error = this.error;
        return Failure;
    }

    /// <summary>
    /// <para>
    ///     Create a new operation result with a new value, converted from the current value.
    /// </para>
    /// <para>
    ///     When the operation result is a failure, the new operation result is also a failure.
    ///     When the operation result is successful, the new value is the result of the conversion.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the result, after the conversion.</typeparam>
    /// <param name="map">The function to convert the value.</param>
    /// <returns>The new operation result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly OperationResult<TValue> Convert<TValue>(Func<TValue> map)
        => Failure ? error : map();

    /// <summary>
    /// <para>
    ///     Create a new operation result with a new value, converted from the current value.
    ///     This method also takes a parameter for the conversion.
    /// </para>
    /// <para>
    ///     When the operation result is a failure, the new operation result is also a failure.
    ///     When the operation result is successful, the new value is the result of the conversion.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The type of the result, after the conversion.</typeparam>
    /// <typeparam name="TParam">The type of the parameter for the conversion.</typeparam>
    /// <param name="map">The function to convert the value.</param>
    /// <param name="param">The parameter for the conversion.</param>
    /// <returns>The new operation result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly OperationResult<TValue> Convert<TValue, TParam>(
        Func<TParam, TValue> map,
        TParam param)
        => Failure ? error : map(param);
}
