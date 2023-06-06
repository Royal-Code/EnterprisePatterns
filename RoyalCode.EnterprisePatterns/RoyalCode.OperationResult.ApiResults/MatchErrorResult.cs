using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoyalCode.OperationResults.Convertion;
using System.Reflection;
using System.Text.Json;

namespace RoyalCode.OperationResults;

/// <summary>
/// <para>
///     Minimal API Result for <see cref="ResultsCollection"/>.
/// </para>
/// <para>
///     Used for create a result from the <see cref="OperationResult"/> match for the error case.
/// </para>
/// </summary>
public class MatchErrorResult
#if NET7_0_OR_GREATER
    : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<ResultsCollection>
#else
    : IResult
#endif
{
    /// <summary>
    /// Determines if the default result is <see cref="OperationResult"/> or <see cref="ProblemDetails"/>.
    /// </summary>
    public static bool IsProblemDetailsDefault { get; set; } = true;

    private readonly ResultsCollection results;

    /// <summary>
    /// Creates a new instance of <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="results"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public MatchErrorResult(ResultsCollection results)
    {
        this.results = results ?? throw new ArgumentNullException(nameof(results));
        StatusCode = results.GetHttpStatus();
    }

    /// <inheritdoc />
    public int? StatusCode { get; }

    /// <inheritdoc />
    public object? Value => results;

#if NET7_0_OR_GREATER

    /// <inheritdoc />
    ResultsCollection? IValueHttpResult<ResultsCollection>.Value => results;

#endif

    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.TryGetResultTypeHeader(out var resultType);
        return resultType switch
        {
            "ProblemDetails" => WriteProblemDetails(httpContext),
            "OperationResult" => WriteOperationResult(httpContext),
            _ => WriteDefault(httpContext)
        };
    }

    /// <summary>
    /// <para>
    ///     Check if the default result is <see cref="OperationResult"/> or <see cref="ProblemDetails"/> 
    ///     and write the result.
    /// </para>
    /// <para>
    ///     For determinate the default result, use <see cref="IsProblemDetailsDefault"/> static property.
    /// </para>
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the asynchronous execute operation.</returns>
    public Task WriteDefault(HttpContext httpContext) 
        => IsProblemDetailsDefault ? WriteProblemDetails(httpContext) : WriteOperationResult(httpContext);

    /// <summary>
    /// Write the <see cref="ProblemDetails"/> result.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the asynchronous execute operation.</returns>
    public Task WriteProblemDetails(HttpContext httpContext)
    {
        var options = httpContext.RequestServices.GetRequiredService<IOptions<ProblemDetailsOptions>>().Value;
        var problemDetails = results.ToProblemDetails(options);
        JsonSerializerOptions? serializerOptions = null;

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest;
        return httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            serializerOptions,
            "application/problem+json",
            httpContext.RequestAborted);
    }

    /// <summary>
    /// Write the <see cref="OperationResult"/> result.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the asynchronous execute operation.</returns>
    public Task WriteOperationResult(HttpContext httpContext)
    {
#if NET7_0_OR_GREATER
        return httpContext.Response.WriteAsJsonAsync(
            results,
            results.GetJsonTypeInfo(),
            "application/json",
            httpContext.RequestAborted);
#else
        return httpContext.Response.WriteAsJsonAsync(
            results,
            results.GetJsonSerializerOptions(),
            "application/json",
            httpContext.RequestAborted);
#endif
    }

    /// <inheritdoc />
    public static void PopulateMetadata(MethodInfo _, EndpointBuilder builder)
    {
        var type = typeof(IOperationResult);
        string[] content = { "application/json" };

        builder.Metadata.Add(
            new ResponseTypeMetadata(type, StatusCodes.Status400BadRequest, content));
        builder.Metadata.Add(
            new ResponseTypeMetadata(type, StatusCodes.Status404NotFound, content));
        builder.Metadata.Add(
            new ResponseTypeMetadata(type, StatusCodes.Status409Conflict, content));
        builder.Metadata.Add(
            new ResponseTypeMetadata(type, StatusCodes.Status422UnprocessableEntity, content));

        type = typeof(ProblemDetails);
        content = new[] { "application/problem+json" };

        builder.Metadata.Add(
            new ResponseTypeMetadata(type, StatusCodes.Status400BadRequest, content));
        builder.Metadata.Add(
            new ResponseTypeMetadata(type, StatusCodes.Status404NotFound, content));
        builder.Metadata.Add(
            new ResponseTypeMetadata(type, StatusCodes.Status409Conflict, content));
        builder.Metadata.Add(
            new ResponseTypeMetadata(type, StatusCodes.Status422UnprocessableEntity, content));
    }
}