using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoyalCode.OperationResult.ProblemDetails.Convertion;

namespace RoyalCode.OperationResult.ApiResults;

/// <summary>
/// Minimal API Result for <see cref="IOperationResult"/>.
/// </summary>
public class ApiOperationResult : IResult
{
    private const string OperationResultHeaderKey = "OperationResultHeader";
    private const string OperationResultHeaderDefaultValue = "X-Result";

    private static string? headerName;

    /// <summary>
    /// Creates a new instance of <see cref="ApiOperationResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IOperationResult"/>.</param>
    /// <param name="createdPath">The path created by the operation.</param>
    /// <param name="formatPathWithValue">
    ///     If true, the <paramref name="createdPath"/> will be formatted with the value of the result.
    /// </param>
    public ApiOperationResult(IOperationResult result, string? createdPath, bool formatPathWithValue)
    {
        Result = result;
        CreatedPath = createdPath;
        FormatPathWithValue = formatPathWithValue;
    }

    /// <summary>
    /// The <see cref="IOperationResult"/>.
    /// </summary>
    public IOperationResult Result { get; }

    /// <summary>
    /// The path created by the operation.
    /// </summary>
    public string? CreatedPath { get; }

    /// <summary>
    /// If true, the <see cref="CreatedPath"/> will be formatted with the value of the result.
    /// </summary>
    public bool FormatPathWithValue { get; }

    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        if (httpContext.TryGetResultTypeHeader(out var resultType))
        {
            if (resultType == "ProblemDetails")
                return CreateProblemDetailsResult(httpContext);
            if (resultType == "OperationResult")
                return CreateOperationResult(httpContext);
        }

        return Result.Success
            ? CreateDefaultSuccessResult(httpContext)
            : CreateDefaultFailureResult(httpContext);
    }

    private Task CreateProblemDetailsResult(HttpContext httpContext)
    {
        if (Result.Success)
            return CreateDefaultSuccessResult(httpContext);

        var options = httpContext.RequestServices.GetRequiredService<IOptions<ProblemDetails.ProblemDetailsOptions>>().Value;
        var problemDetails = Result.ToProblemDetails(options);
        return Results.Json(problemDetails,
            contentType: "application/problem+json",
            statusCode: problemDetails.Status).ExecuteAsync(httpContext);
    }

    private Task CreateOperationResult(HttpContext httpContext)
    {
        return Results.Json(Result, statusCode: Result.GetHttpStatus()).ExecuteAsync(httpContext);
    }

    private Task CreateDefaultSuccessResult(HttpContext httpContext)
    {
        IResultHasValue? valueResult = Result as IResultHasValue;
        bool hasValue = valueResult?.Value is not null;

        if (httpContext.Request.Method == "GET")
        {
            return hasValue
                ? Results.Ok(valueResult!.Value).ExecuteAsync(httpContext)
                : Results.Ok(valueResult).ExecuteAsync(httpContext);
        }

        if (CreatedPath is not null)
        {
            var createdPath = CreatedPath;
            if (FormatPathWithValue && hasValue)
                createdPath = string.Format(createdPath, valueResult!.Value);

            return Results.Created(createdPath, valueResult?.Value).ExecuteAsync(httpContext);
        }

        if (!hasValue)
            return Results.NoContent().ExecuteAsync(httpContext);

        return Results.Json(valueResult!.Value, statusCode: Result.GetHttpStatus()).ExecuteAsync(httpContext);
    }

    private Task CreateDefaultFailureResult(HttpContext httpContext)
    {
        return Results.Json(Result, statusCode: Result.GetHttpStatus()).ExecuteAsync(httpContext);
    }
}


