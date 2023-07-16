#if NET7_0_OR_GREATER

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using RoyalCode.OperationResults.Metadata;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoyalCode.OperationResults.HttpResults;

/// <summary>
/// <para>
///     A <see cref="IResult"/> for <see cref="OperationResult"/> match for the success or error case.
/// </para>
/// <para>
///     When success, returns a <see cref="Ok{TValue}"/>.
///     When error, returns a <see cref="MatchErrorResult"/>.
/// </para>
/// </summary>
/// <typeparam name="T">The returned value type when success.</typeparam>
public sealed class OkMatch<T> : IResult, INestedHttpResult, IEndpointMetadataProvider
{
    /// <summary>
    /// Creates a new <see cref="OkMatch{T}"/> for the <see cref="OperationResult{TValue}"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OkMatch<T>(OperationResult<T> result) => new(result);

    /// <summary>
    /// Creates a new <see cref="IResult"/> for the <see cref="OperationResult"/> match.
    /// </summary>
    /// <param name="result">The <see cref="OperationResult"/> to be converted.</param>
    public OkMatch(OperationResult<T> result)
    {
        Result = result.Match<IResult>(
            TypedResults.Ok,
            static error => new MatchErrorResult(error));
    }

    /// <inheritdoc/>
    public IResult Result { get; }

    /// <inheritdoc/>
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ResponseTypeMetadata(typeof(T), StatusCodes.Status200OK, MediaTypeNames.Application.Json));
        MatchErrorResult.PopulateMetadata(method, builder);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);
}

#endif