using RoyalCode.SmartProblems;
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
    /// Implicit conversion from <see cref="SaveResult"/> to <see cref="Result"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result(SaveResult result)
        => result.HasProblems(out var problems) ? problems : Result.Ok();

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
        Problems = Problems.InternalError(ex);
    }

    /// <summary>
    /// The number of entities created, updated or deleted.
    /// </summary>
    public int Changes { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>
    /// The error that occurred during the save operation.
    /// </summary>
    public Problems? Problems { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>
    /// Returns true if the save operation succeeded.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Problems))]
    public bool IsSuccess => Problems is null;

    /// <summary>
    /// Returns true if the save operation failed.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Problems))]
    public bool IsFailure => Problems is not null;

    /// <summary>
    ///     Try to get the problems that occurred during the save operation.
    /// </summary>
    /// <param name="problems">
    ///     The problems occurred during the save operation.
    /// </param>
    /// <returns>
    ///     True if the save operation failed, false otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        problems = Problems;
        return IsFailure;
    }

    /// <summary>
    /// <para>
    ///     Check if the save operation is a success and return the problems.
    /// </para>
    /// <para>
    ///     When the save operation is a success, the problems will be null, otherwise the problems will be returned.
    /// </para>
    /// </summary>
    /// <param name="problems">The problems.</param>
    /// <returns>
    ///     True if the save operation is a success, false otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSuccessOrGetProblems([NotNullWhen(false)] out Problems? problems)
    {
        problems = Problems;
        return IsSuccess;
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
    public Result<TValue> Map<TValue>(TValue value)
    {
        return IsSuccess
            ? value
            : Problems;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return IsSuccess
            ? $"Success: {Changes} changes"
            : $"Failure: {Problems}";
    }
}

/// <summary>
/// Extension methods for <see cref="SaveResult"/>.
/// </summary>
public static class SaveResultExtensions
{
    /// <summary>
    /// Convert the save result to an operation result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="task">A task to save work unit operations.</param>
    /// <param name="value">The value used in case of success.</param>
    /// <returns>
    ///     An operation result with the error that occurred during the save operation in case of failure,
    ///     or an operation result with the given value in case of success.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<Result<TValue>> MapAsync<TValue>(this Task<SaveResult> task, TValue value)
    {
        var saveResult = await task;
        return saveResult.Map(value);
    }
}
