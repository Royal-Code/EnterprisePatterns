
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoyalCode.OperationResults.Convertion;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;

namespace RoyalCode.OperationResults;

#if NET7_0_OR_GREATER

/// <summary>
/// <para>
///     Minimal API Result for <see cref="ResultsCollection"/>.
/// </para>
/// <para>
///     Used for create a result from the <see cref="OperationResult"/> match for the error case.
/// </para>
/// </summary>
public class MatchErrorResult : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<ResultsCollection>
{
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

    /// <inheritdoc />
    ResultsCollection? IValueHttpResult<ResultsCollection>.Value => results;

    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        if (httpContext.TryGetResultTypeHeader(out var resultType) && resultType == "ProblemDetails")
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

        return httpContext.Response.WriteAsJsonAsync(
            results,
            results.GetJsonTypeInfo(),
            "application/problem+json",
            httpContext.RequestAborted);
    }

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        throw new NotImplementedException();
    }
}

#endif