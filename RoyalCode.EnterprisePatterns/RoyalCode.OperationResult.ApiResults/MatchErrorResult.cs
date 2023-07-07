﻿using Microsoft.AspNetCore.Builder;
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
///     Minimal API Result for <see cref="ResultErrors"/>.
/// </para>
/// <para>
///     Used for create a result from the <see cref="OperationResult"/> match for the error case.
/// </para>
/// </summary>
public class MatchErrorResult
#if NET7_0_OR_GREATER
    : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<ResultErrors>
#else
    : IResult
#endif
{
    private readonly ResultErrors errors;

    /// <summary>
    /// Creates a new instance of <see cref="MatchErrorResult"/>.
    /// </summary>
    /// <param name="errors"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public MatchErrorResult(ResultErrors errors)
    {
        this.errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }

    /// <inheritdoc />
    public int? StatusCode => errors.GetHttpStatus();

    /// <inheritdoc />
    public object? Value => errors;

#if NET7_0_OR_GREATER

    /// <inheritdoc />
    ResultErrors? IValueHttpResult<ResultErrors>.Value => errors;

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
    ///     For determinate the default result, 
    ///     use <see cref="ApiOperationResultOptions.IsProblemDetailsDefault"/> static property.
    /// </para>
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the asynchronous execute operation.</returns>
    public Task WriteDefault(HttpContext httpContext) 
        => ApiOperationResultOptions.IsProblemDetailsDefault 
            ? WriteProblemDetails(httpContext) 
            : WriteOperationResult(httpContext);

    /// <summary>
    /// Write the <see cref="ProblemDetails"/> result.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the asynchronous execute operation.</returns>
    public Task WriteProblemDetails(HttpContext httpContext)
    {
        var options = httpContext.RequestServices.GetRequiredService<IOptions<ProblemDetailsOptions>>().Value;
        var problemDetails = errors.ToProblemDetails(options);
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
        httpContext.Response.StatusCode = errors.GetHttpStatus();

#if NET7_0_OR_GREATER
        return httpContext.Response.WriteAsJsonAsync(
            errors,
            errors.GetJsonTypeInfo(),
            "application/json",
            httpContext.RequestAborted);
#else
        return httpContext.Response.WriteAsJsonAsync(
            errors,
            errors.GetJsonSerializerOptions(),
            "application/json",
            httpContext.RequestAborted);
#endif
    }

    /// <inheritdoc />
    public static void PopulateMetadata(MethodInfo _, EndpointBuilder builder)
    {
        var type = typeof(ResultErrors);
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