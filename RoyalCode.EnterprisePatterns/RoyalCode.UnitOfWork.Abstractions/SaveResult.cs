using RoyalCode.OperationResults;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RoyalCode.UnitOfWork.Abstractions;

/// <summary>
/// <para>
///     A result of a save operation of a unit of work.
/// </para>
/// </summary>
public readonly struct SaveResult
{
    /// <summary>
    /// Implicit conversion from <see cref="int"/> to <see cref="SaveResult"/>.
    /// </summary>
    /// <param name="changes"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    public static implicit operator SaveResult(int changes) => new(changes);

    /// <summary>
    /// Implicit conversion from <see cref="Exception"/> to <see cref="SaveResult"/>.
    /// </summary>
    /// <param name="ex"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SaveResult(Exception ex) => new(ex);

    /// <summary>
    /// Implicit conversion from <see cref="SaveResult"/> to <see cref="OperationResult"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OperationResult(SaveResult result) 
        => result.TryGetError(out var errors) ? errors : new OperationResult();

    /// <summary>
    /// Implicit conversion from <see cref="SaveResult"/> to <see cref="OperationResult"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValidableResult(SaveResult result)
        => result.TryGetError(out var errors) ? new ValidableResult(errors) : new ValidableResult();

    /// <summary>
    /// Success constructor
    /// </summary>
    /// <param name="changes">Number of entities modified.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SaveResult(int changes)
    {
        Changes = changes;
    }

    /// <summary>
    /// Failure constructor
    /// </summary>
    /// <param name="ex">
    ///     The exception that occurred during the save operation.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SaveResult(Exception ex)
    {
        Error = ResultMessage.Error(ex);
    }

    /// <summary>
    /// The number of entities created, updated or deleted.
    /// </summary>
    public int Changes { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>
    /// The error that occurred during the save operation.
    /// </summary>
    public ResultMessage? Error { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>
    /// Returns true if the save operation succeeded.
    /// </summary>
    public bool IsSuccess => Error is null;

    /// <summary>
    /// Returns true if the save operation failed.
    /// </summary>
    public bool IsFailure => Error is not null;

    /// <summary>
    ///     Try to get the error that occurred during the save operation.
    /// </summary>
    /// <param name="error">
    ///     The error that occurred during the save operation.
    /// </param>
    /// <returns>
    ///     True if the save operation failed, false otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetError([NotNullWhen(true)] out ResultMessage? error)
    {
        error = Error;
        return IsFailure;
    }

    /// <summary>
    /// Convert the save result to an operation result.
    /// </summary>
    /// <returns>
    ///     An operation result with the error that occurred during the save operation in case of failure,
    ///     or an empty operation result in case of success.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OperationResult Convert()
    {
          return IsSuccess
            ? new OperationResult()
            : Error!;
    }

    /// <summary>
    /// Convert the save result to an operation result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value used in case of success.</param>
    /// <returns>
    ///     An operation result with the error that occurred during the save operation in case of failure,
    ///     or an operation result with the given value in case of success.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OperationResult<TValue> Convert<TValue>(TValue value)
    {
        return IsSuccess
            ? value
            : Error!;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return IsSuccess
            ? $"Success: {Changes} changes"
            : $"Failure: {Error}";
    }
}