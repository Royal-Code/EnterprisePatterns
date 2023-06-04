
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Options;
using RoyalCode.OperationResults;
using System.Net.Http;
using System.Reflection;

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
        this IResultExtensions extensions, OperationResult<T> result)
    {
        return result.Match<Results<Ok<T>, MatchErrorResult>>(value =>
        {
            return TypedResults.Ok(value);
        },
        error =>
        {
            return Results.Json(error, statusCode: error.GetHttpStatus());
        });
    }

    public static Results<Results<Ok<T>, Created<T>>, MatchErrorResult> ToResult<T>(this IResultExtensions extensions,
        OperationResult<T> result,
        string? createdPath = null, bool formatPathWithValue = false)
    {
        return result.Match<Results<Results<Ok<T>, Created<T>>, MatchErrorResult>>(value =>
        {
            if (createdPath is not null)
            {
                if (formatPathWithValue)
                    createdPath = string.Format(createdPath, value);

                return Results.Created(createdPath, value);
            }

            return TypedResults.Ok(value);
        },
        error =>
        {
            return Results.Json(error, statusCode: error.GetHttpStatus());
        });
    }

#endif
}

#if NET7_0_OR_GREATER

public class MatchErrorResult : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<ResultsCollection>
{
    private readonly ResultsCollection results;

    public MatchErrorResult(ResultsCollection results)
    {
        this.results = results ?? throw new ArgumentNullException(nameof(results));
        StatusCode = results.GetHttpStatus();
    }

    public int? StatusCode { get; }

    public object? Value => results;

    ResultsCollection? IValueHttpResult<ResultsCollection>.Value => results;

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        throw new NotImplementedException();
    }
}

#endif