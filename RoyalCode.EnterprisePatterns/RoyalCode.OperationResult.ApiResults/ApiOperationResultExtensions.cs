#if NET7_0_OR_GREATER
using Microsoft.AspNetCore.Http.HttpResults;
#endif
using RoyalCode.OperationResults;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extensions for adapt <see cref="IOperationResult"/> to <see cref="IResult"/>.
/// </summary>
public static partial class ApiResults
{
    /// <summary>
    /// Convert <see cref="IOperationResult"/> to <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IOperationResult"/>.</param>
    /// <param name="createdPath">The path for created responses, when applyable.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static IResult ToResult(this IOperationResult result,
        string? createdPath = null, bool formatPathWithValue = false)
    {
        return new ApiOperationResult(result, createdPath, formatPathWithValue);
    }

#if NET6_0

    /// <summary>
    /// Convert the <see cref="OperationResult{T}"/> to <see cref="IResult"/>.
    /// </summary>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static IResult ToResult(this IResultExtensions _, OperationResult result)
    {
        return result.Match(
            Results.NoContent,
            error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="OperationResult{T}"/> to <see cref="IResult"/>.
    /// </summary>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static IResult ToResult(this IResultExtensions _, OperationResult result, string createdPath)
    {
        return result.Match(
            () => Results.Created(createdPath, null),
            error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="OperationResult{T}"/> to <see cref="IResult"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static IResult ToResult<T>(this IResultExtensions _, OperationResult<T> result)
    {
        return result.Match(
            value => Results.Ok(value),
            error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="OperationResult{T}"/> to <see cref="IResult"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static IResult ToResult<T>(this IResultExtensions _,
        OperationResult<T> result, string createdPath, bool formatPathWithValue = false)
    {
        return result.Match(
            value => Results.Created(formatPathWithValue ? string.Format(createdPath, value) : createdPath, value),
            error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="OperationResult{T}"/> to <see cref="IResult"/>.
    /// </summary>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static IResult ToResult(this IResultExtensions _, ValidableResult result)
    {
        return result.Match(
            Results.NoContent,
            error => new MatchErrorResult(error));
    }

#else

    /// <summary>
    /// Convert the <see cref="OperationResult{T}"/> to <see cref="NoContent"/> or <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static Results<NoContent, MatchErrorResult> ToResult(this IResultExtensions _, OperationResult result)
    {
        return result.Match<Results<NoContent, MatchErrorResult>>(
            () => TypedResults.NoContent(),
            error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="OperationResult{T}"/> to <see cref="Created"/> or <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static Results<Created, MatchErrorResult> ToResult(this IResultExtensions _, OperationResult result, string createdPath)
    {
        return result.Match<Results<Created, MatchErrorResult>>(
            () => TypedResults.Created(createdPath),
            error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="OperationResult{T}"/> to <see cref="Ok{TValue}"/> or <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <returns>The <see cref="IResult"/> implementation for the response.</returns>
    public static Results<Ok<T>, MatchErrorResult> ToResult<T>(
        this IResultExtensions _, OperationResult<T> result)
    {
        return result.Match<Results<Ok<T>, MatchErrorResult>>(
            value => TypedResults.Ok(value),
            error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="OperationResult{T}"/> to <see cref="Created{T}"/> or <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <param name="createdPath">The path for created responses.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="IResult"/> implementation for the response.</returns>
    public static Results<Created<T>, MatchErrorResult> ToResult<T>(this IResultExtensions _,
        OperationResult<T> result, string createdPath, bool formatPathWithValue = false)
    {
        return result.Match<Results<Created<T>, MatchErrorResult>>(
            value => TypedResults.Created(formatPathWithValue ? string.Format(createdPath, value) : createdPath, value),
            error => new MatchErrorResult(error));
    }

    /// <summary>
    /// Convert the <see cref="ValidableResult{T}"/> to <see cref="NoContent"/> or <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="_">Used for extension.</param>
    /// <param name="result">The operation result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static Results<NoContent, MatchErrorResult> ToResult(this IResultExtensions _, ValidableResult result)
    {
        return result.Match<Results<NoContent, MatchErrorResult>>(
            () => TypedResults.NoContent(),
            error => new MatchErrorResult(error));
    }

#endif
}