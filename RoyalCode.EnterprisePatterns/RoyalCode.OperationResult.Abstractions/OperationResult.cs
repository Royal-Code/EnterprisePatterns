
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RoyalCode.OperationResult;

/// <summary>
/// <para>
///     Represents the result of an operation.
/// </para>
/// <para>
///     The operation result can be either successful or a failure.
///     When the operation result is successful, the value of <see cref="Value"/> is not <see langword="null"/>.
///     When the operation result is a failure, the value of <see cref="Value"/> is <see langword="null"/>.
/// </para>
/// <para>
///     The operation result can be implicitly converted from a value or an error.
///     The operation result can be matched using the <see cref="Match{TResult}(Func{TValue, TResult}, Func{TError, TResult})"/> method.
///     The operation result can be matched using the <see cref="Match{TResult, TParam}(Func{TValue, TParam, TResult}, Func{TError, TParam, TResult}, TParam)"/> method.
///     The operation result can be converted to a string using the <see cref="ToString"/> method.
/// </para>
/// </summary>
/// <typeparam name="TValue">The type of the value of the operation result.</typeparam>
/// <typeparam name="TError">The type of the error of the operation result.</typeparam>
public readonly struct OperationResult<TValue, TError>
{
    /// <summary>
    /// Implicitly convert a value to a successful operation result.
    /// </summary>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OperationResult<TValue, TError>(TValue value) => new(value);

    /// <summary>
    /// Implicitly convert an error to a failure operation result.
    /// </summary>
    /// <param name="error"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OperationResult<TValue, TError>(TError error) => new(error);

    private readonly TValue? value;
    private readonly TError? error;

    /// <summary>
    /// Creates a successful operation result.
    /// </summary>
    /// <param name="value">The value of the operation result.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OperationResult(TValue? value)
    {
        this.value = value;
        error = default;
        Failure = false;
    }

    /// <summary>
    /// Creates a failure operation result.
    /// </summary>
    /// <param name="error">The error of the operation result.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OperationResult(TError? error)
    {
        value = default;
        this.error = error;
        Failure = true;
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
    [MemberNotNullWhen(true, nameof(value))]
    [MemberNotNullWhen(false, nameof(error))]
#endif
    public readonly bool Success { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => !Failure; }

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
    [MemberNotNullWhen(false, nameof(value))]
#endif
    public readonly bool Failure { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>
    /// <para>
    ///     Check if the operation result is failure, the return true,
    ///     otherwise set the value of the operation result, because it is successful.
    /// </para>
    /// </summary>
    /// <param name="value">The value of the operation result.</param>
    /// <returns>Whether the operation result is failure.</returns>
    public readonly bool IsFailureOrGetValue([NotNullWhen(false)] out TValue? value)
    {
        value = this.value;
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
    /// <typeparam name="TOther">The other type of the result, after the conversion.</typeparam>
    /// <param name="map">The function to convert the value.</param>
    /// <returns>The new operation result.</returns>
    public readonly OperationResult<TOther, TError> Convert<TOther>(Func<TValue, TOther> map)
        => Failure ? error : map(value!);

    /// <summary>
    /// <para>
    ///     Check if the operation result is failure, the return true,
    ///     otherwise set the coverted value of the operation result, because it is successful.
    /// </para>
    /// </summary>
    /// <typeparam name="TOther">The other type of the result, after the conversion.</typeparam>
    /// <param name="map">The function to convert the value.</param>
    /// <param name="value">The converted value of the operation result.</param>
    /// <returns>Whether the operation result is failure.</returns>
    public readonly bool IsFailureOrConvert<TOther>(Func<TValue, TOther> map, [NotNullWhen(false)] out TOther? value)
        where TOther : notnull
    {
        if (Failure)
        {
            value = default;
            return true;
        }

        value = map(this.value);
        return false;
    }

    /// <summary>
    /// <para>
    ///     Match a function depending on the operation result.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The type returned by the match function.</typeparam>
    /// <param name="match">The function to execute if the operation result is successful.</param>
    /// <param name="error">The function to execute if the operation result is a failure.</param>
    /// <returns>The result of the executed function.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(Func<TValue, TResult> match, Func<TError, TResult> error)
        => Failure ? error(this.error!) : match(value!);

    /// <summary>
    /// <para>
    ///     Match a function depending on the operation result.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The type returned by the match function.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the match function.</typeparam>
    /// <param name="match">The function to execute if the operation result is successful.</param>
    /// <param name="error">The function to execute if the operation result is a failure.</param>
    /// <param name="param">The parameter passed to the match function.</param>
    /// <returns>The result of the executed function.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult, TParam>(
        Func<TValue, TParam, TResult> match, 
        Func<TError, TParam, TResult> error, 
        TParam param)
        => Failure ? error(this.error!, param) : match(value!, param);

    /// <summary>
    /// Convert the operation result to a string.
    /// When the operation result is successful, the string will be "Success: {value}".
    /// When the operation result is a failure, the string will be "Failure: {error}".
    /// </summary>
    /// <returns>The string representation of the operation result.</returns>
    public override readonly string ToString()
        => Failure ? $"Failure: {error}" : $"Success: {value}";
}
