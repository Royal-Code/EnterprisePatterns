using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoyalCode.OperationResults.HttpResults;

#if NET7_0_OR_GREATER

/// <summary>
/// <para>
///     A <see cref="IResult"/> for <see cref="OperationResult"/> match for the success or error case.
/// </para>
/// <para>
///     When success, returns a <see cref="Created"/>.
///     When error, returns a <see cref="MatchErrorResult"/>.
/// </para>
/// </summary>
public class CreatedMatch : IResult, INestedHttpResult, IEndpointMetadataProvider
{
    /// <summary>
    /// Creates a new <see cref="IResult"/> for the <see cref="OperationResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="OperationResult"/> to be converted.</param>
    /// <param name="createdPath">The location of the created resource.</param>
    public CreatedMatch(OperationResult result, string createdPath)
    {
        Result = result.Match<IResult, string>(
            TypedResults.Created,
            static (error, uri) => new MatchErrorResult(error),
            createdPath);
    }

    /// <inheritdoc/>
    public IResult Result { get; }

    /// <inheritdoc/>
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ResponseTypeMetadata(StatusCodes.Status201Created));
        MatchErrorResult.PopulateMetadata(method, builder);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);
}

#endif