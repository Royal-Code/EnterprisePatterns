using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RoyalCode.OperationResults;
using System.Net.Http;

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
    public static IResult ToResult<T>(this IResultExtensions extensions,
        OperationResult<T> result,
        string? createdPath = null, bool formatPathWithValue = false)
    {
        return result.Match<IResult>(value =>
        {
            if (createdPath is not null)
            {
                if (formatPathWithValue)
                    createdPath = string.Format(createdPath, value);

                return Results.Created(createdPath, value);
            }

            
            return Results.Ok(value);
        },
        error =>
        {
            return Results.Json(error, statusCode: error.GetHttpStatus());
        });
    }

#else

    public static Results<Ok<T>, MatchErrorResult> ToResult<T>(
        this IResultExtensions _, OperationResult<T> result)
    {
        return result.Match<Results<Ok<T>, MatchErrorResult>>(value =>
        {
            return TypedResults.Ok(value);
        },
        error =>
        {
            return new MatchErrorResult(error);
        });
    }

    public static Results<Created<T>, MatchErrorResult> ToResult<T>(this IResultExtensions _,
        OperationResult<T> result, string createdPath, bool formatPathWithValue = false)
    {
        return result.Match<Results<Created<T>, MatchErrorResult>>(value =>
        {
            if (formatPathWithValue)
                createdPath = string.Format(createdPath, value);

            return TypedResults.Created(createdPath, value);
        },
        error =>
        {
            return new MatchErrorResult(error);
        });
    }

#endif
}