
using RoyalCode.OperationResult;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extensions for adapt <see cref="IOperationResult"/> to <see cref="IResult"/>.
/// </summary>
public static partial class ApiResults
{
    /// <summary>
    /// Creates a default success, with no message.
    /// </summary>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult Ok() => Results.Ok(BaseResult.ImmutableSuccess);

    /// <summary>
    /// Creates a default success, including a return object.
    /// </summary>
    /// <typeparam name="T">The type of the return object.</typeparam>
    /// <param name="value">The return object.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult Ok<T>(T value) => Results.Ok(ValueResult.CreateSuccess(value));

    /// <summary>
    /// Creates a default success, including a return object and status code 201 (Created).
    /// </summary>
    /// <typeparam name="T">The type of the return object.</typeparam>
    /// <param name="uri">The URI of the created resource.</param>
    /// <param name="value">The return object.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult Created<T>(string uri, T value)
        => Results.Created(uri, ValueResult.CreateSuccess(value));

    /// <summary>
    /// Creates a default success, including a return object and status code 201 (Created).
    /// </summary>
    /// <typeparam name="T">The type of the return object.</typeparam>
    /// <param name="uri">The URI of the created resource.</param>
    /// <param name="value">The return object.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult Created<T>(Uri uri, T value)
        => Results.Created(uri, ValueResult.CreateSuccess(value));


    /// <summary>
    /// Creates an internal error from the exception.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <param name="text">The text to be added to the result, optional, when not informed the exception message is used.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult ApplicationError(Exception ex, string? text = null)
        => Results.Json(BaseResult.ApplicationError(ex, text), statusCode: StatusCodes.Status500InternalServerError);

    /// <summary>
    /// Creates a failure result with status code 404 (Not Found).
    /// </summary>
    /// <param name="message">The message to be added to the result.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult NotFound(string message)
        => Results.NotFound(BaseResult.NotFound(message));

    /// <summary>
    /// Creates a failure result with status code 403 (Forbidden).
    /// </summary>
    /// <param name="message">The message to be added to the result.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult Forbidden(string message)
        => Results.Json(BaseResult.Forbidden(message), statusCode: StatusCodes.Status403Forbidden);

    /// <summary>
    /// Creates a failure result with status code 400 (Bad Request).
    /// </summary>
    /// <param name="message">The message to be added to the result.</param>
    /// <param name="property">The property name that caused the error, optional.</param>
    /// <param name="ex">A exception que gerou o erro, opcional.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult BadRequest(string message, string? property = null, Exception? ex = null)
        => Results.BadRequest(BaseResult.ValidationError(message, property, ex));

    /// <summary>
    /// Creates a <see cref="IResult"/> from the <see cref="IOperationResult"/> informing the necessary status code.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="result">The <see cref="IOperationResult"/>.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult StatusCode(int statusCode, IOperationResult result)
        => Results.Json(result, statusCode: statusCode);

    /// <summary>
    /// Creates a <see cref="IResult"/> with an error message and informing the necessary status code.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult StatusCode(int statusCode, string errorMessage)
        => Results.Json(BaseResult.CreateFailure(errorMessage), statusCode: statusCode);

    /// <summary>
    /// Creates a <see cref="IResult"/> with a value and an error message and informing the necessary status code.
    /// </summary>
    /// <typeparam name="T">The type of the return object.</typeparam>
    /// <param name="value">The return object.</param>
    /// <param name="statusCode">The status code.</param>
    ///<param name="errorMessage">The error message.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult StatusCode<T>(T value, int statusCode, string errorMessage)
        => Results.Json(ValueResult.CreateFailure(value, errorMessage), statusCode: statusCode);

    /// <summary>
    /// Creates a <see cref="IResult"/> with a value and informing the necessary status code.
    /// </summary>
    /// <typeparam name="T">The type of the return object.</typeparam>
    /// <param name="value">The return object.</param>
    /// <param name="statusCode">The status code.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult StatusCode<T>(T value, int statusCode)
        => Results.Json(ValueResult.CreateSuccess(value), statusCode: statusCode);
}

#if NET6_0

/// <summary>
///    Provides a set of static methods for creating <see cref="IResult" /> objects.
/// </summary>
public static partial class ApiResults
{
    /// <summary>
    ///     Creates a <see cref="IResult" /> from the operation result 
    ///     with the "Status Code" corresponding to the result values.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult ToHttpResult(this IOperationResult result)
    {
        if (result.HasHttpStatus(out var statusCode))
            return Results.Json(result, statusCode: (int)statusCode.Value);

        if (result.Success)
            return Results.Ok(result);

        if (result.HasApplicationError())
            return Results.Json(result, statusCode: StatusCodes.Status500InternalServerError);
        else if (result.HasNotFound())
            return Results.NotFound(result);
        else if (result.HasForbidden())
            return Results.Json(result, statusCode: StatusCodes.Status403Forbidden);

        return Results.BadRequest(result);
    }

    /// <summary>
    /// <para>
    ///     Creates a <see cref="IResult" /> from the operation result 
    ///     with the "Status Code" corresponding to the result values.
    /// </para>
    /// <para>
    ///     Em caso de sucesso é returnado o <see cref="IResult" /> com o "Status Code" 201 (Created).
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="uri">The URI of the created resource.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult ToHttpResult(this IOperationResult result, string? uri)
    {
        if (result.Success && uri is not null)
            return Results.Created(uri, result);
        return result.ToHttpResult();
    }
}
#endif

#if NET7_0_OR_GREATER

/// <summary>
///    Provides a set of static methods for creating <see cref="IResult" /> objects.
/// </summary>
public static partial class ApiResults
{
    /// <summary>
    ///     Creates a <see cref="IResult" /> from the operation result 
    ///     with the "Status Code" corresponding to the result values.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult ToHttpResult(this IOperationResult result)
    {
        if (result.HasHttpStatus(out var statusCode))
            return Results.Json(result, statusCode: (int)statusCode.Value);

        if (result.Success)
            return Results.Ok(result);

        if (result.HasApplicationError())
            return Results.Json(result, statusCode: StatusCodes.Status500InternalServerError);
        else if (result.HasNotFound())
            return TypedResults.NotFound(result);
        else if (result.HasForbidden())
            return TypedResults.Json(result, statusCode: StatusCodes.Status403Forbidden);

        return Results.BadRequest(result);
    }

    /// <summary>
    /// <para>
    ///     Creates a <see cref="IResult" /> from the operation result 
    ///     with the "Status Code" corresponding to the result values.
    /// </para>
    /// <para>
    ///     Em caso de sucesso é returnado o <see cref="IResult" /> com o "Status Code" 201 (Created).
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="uri">The URI of the created resource.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult ToHttpResult(this IOperationResult result, string? uri)
    {
        if (result.Success && uri is not null)
            return Results.Created(uri, result);
        return result.ToHttpResult();
    }

    /// <summary>
    ///     Creates a <see cref="IResult" /> from the operation result 
    ///     with the "Status Code" corresponding to the result values.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult ToHttpResult<T>(this IOperationResult<T> result)
    {
        if (result.HasHttpStatus(out var statusCode))
            return Results.Json(result, statusCode: (int)statusCode.Value);

        if (result.Success)
            return Results.Ok(result);
        
        if (result.HasApplicationError())
            return Results.Json(result, statusCode: StatusCodes.Status500InternalServerError);
        else if (result.HasNotFound())
            return TypedResults.NotFound(result);
        else if (result.HasForbidden())
            return TypedResults.Json(result, statusCode: StatusCodes.Status403Forbidden);

        return Results.BadRequest(result);
    }

    /// <summary>
    /// <para>
    ///     Creates a <see cref="IResult" /> from the operation result 
    ///     with the "Status Code" corresponding to the result values.
    /// </para>
    /// <para>
    ///     On success a <see cref="IResult" /> with "Status Code" 201 (Created) is returned.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="uri">The URI of the created resource.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult ToHttpResult<T>(this IOperationResult<T> result, string? uri)
    {
        if (result.Success && uri is not null)
            return Results.Created(uri, result);
        return result.ToHttpResult();
    }

    /// <summary>
    /// Creates a <see cref="IResult" /> with "Status Code" 500 (Internal Server Error) from the exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="text">The text to be included as a message in the result.</param>
    /// <returns>The created <see cref="IResult"/> for the response.</returns>
    public static IResult ToHttpResult(this Exception exception, string? text = null)
        => Results.Json(BaseResult.ApplicationError(exception, text), statusCode: StatusCodes.Status500InternalServerError);
}

#endif