
using Microsoft.AspNetCore.Http;
using RoyalCode.OperationResult;
using RoyalCode.OperationResult.MvcResults;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// Extensions for adapt <see cref="IOperationResult"/> to <see cref="ObjectResult"/>.
/// </summary>
public static class MvcResults
{
    /// <summary>
    /// Convert <see cref="IOperationResult"/> to <see cref="IResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="IOperationResult"/>.</param>
    /// <param name="createdPath">The path for created responses, when applyable.</param>
    /// <param name="formatPathWithValue">Indicates if the <paramref name="createdPath"/> should be formatted with the value of the result.</param>
    /// <returns>The <see cref="IResult"/> for the response.</returns>
    public static ObjectResult ToResult(this IOperationResult result,
        string? createdPath = null, bool formatPathWithValue = false)
    {
        return new MvcOperationResult(result, createdPath, formatPathWithValue);
    }
}

    ///// <summary>
    ///// Default, immutable success return.
    ///// </summary>
    //private static OkObjectResult? okObjectResult;

    ///// <summary>
    /////     Creates a <see cref="ObjectResult" /> from the operation result
    /////     with the "Status Code" corresponding to the values in the result.
    ///// </summary>
    ///// <param name="result">The operation result.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static ObjectResult ToMvcResult(this IOperationResult result)
    //{
    //    if (result.HasHttpStatus(out var statusCode))
    //        return new ObjectResult(result) { StatusCode = (int)statusCode.Value };

    //    if (result.Success)
    //        return new OkObjectResult(result);
        
    //    if (result.HasApplicationError())
    //        return new ObjectResult(result) { StatusCode = StatusCodes.Status500InternalServerError };
    //    else if (result.HasNotFound())
    //        return new NotFoundObjectResult(result);
    //    else if (result.HasForbidden())
    //        return new ObjectResult(result) { StatusCode = StatusCodes.Status403Forbidden };

    //    return new BadRequestObjectResult(result);
    //}

    ///// <summary>
    ///// <para>
    /////     Creates a <see cref="ObjectResult" /> from the operation result
    /////     with the "Status Code" corresponding to the values in the result.
    ///// </para>
    ///// <para>
    /////     On success a <see cref="ObjectResult" /> with "Status Code" 201 (Created) is returned.
    ///// </para>
    ///// </summary>
    ///// <param name="result">The operation result.</param>
    ///// <param name="uri">The URI for the created resource.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static ObjectResult ToMvcResult(this IOperationResult result, string? uri)
    //{
    //    if (result.Success && uri is not null)
    //        return new CreatedResult(uri, result);
    //    return result.ToMvcResult();
    //}

    ///// <summary>
    ///// <para>
    /////     Creates a <see cref="ObjectResult" /> from the operation result
    /////     with the "Status Code" corresponding to the values in the result.
    ///// </para>
    ///// <para>
    /////     On success a <see cref="ObjectResult" /> with "Status Code" 201 (Created) is returned.
    ///// </para>
    ///// </summary>
    ///// <param name="result">The operation result.</param>
    ///// <param name="uri">The URI for the created resource.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static ObjectResult ToMvcResult(this IOperationResult result, Uri uri)
    //{
    //    if (result.Success)
    //        return new CreatedResult(uri, result);
    //    return result.ToMvcResult();
    //}

    ///// <summary>
    ///// Creates a <see cref="ObjectResult" /> with "Status Code" 500 (Internal Server Error) from the exception.
    ///// </summary>
    ///// <param name="exception">A exceção.</param>
    ///// <param name="text">O texto a ser adicionado ao resultado, opcional, quando não informado é utilizada a mensagem da exception.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static ObjectResult ToMvcResult(this Exception exception, string? text = null)
    //{
    //    return new ObjectResult(BaseResult.ApplicationError(exception, text))
    //    {
    //        StatusCode = StatusCodes.Status500InternalServerError
    //    };
    //}

    //#region funções de suporte retornando ObjectResult

    ///// <summary>
    ///// Creates a default success, with no message.
    ///// </summary>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static OkObjectResult Ok() => okObjectResult ??= new(BaseResult.ImmutableSuccess);

    ///// <summary>
    ///// Creates a default success, including a return object.
    ///// </summary>
    ///// <typeparam name="T">The type of the return object.</typeparam>
    ///// <param name="value">The return object.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static OkObjectResult Ok<T>(T value) => new(ValueResult.Create(value));

    ///// <summary>
    ///// Creates a default success, including a return object and status code 201 (Created).
    ///// </summary>
    ///// <typeparam name="T">The type of the return object.</typeparam>
    ///// <param name="uri">The URI for the created resource.</param>
    ///// <param name="value">The return object.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static CreatedResult Created<T>(string uri, T value)
    //    => new(uri, ValueResult.Create(value));

    ///// <summary>
    ///// Creates a default success, including a return object and status code 201 (Created).
    ///// </summary>
    ///// <typeparam name="T">The type of the return object.</typeparam>
    ///// <param name="uri">The URI for the created resource.</param>
    ///// <param name="value">The return object.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static CreatedResult Created<T>(Uri uri, T value)
    //    => new(uri, ValueResult.Create(value));

    ///// <summary>
    ///// Creates an internal server error from the exception.
    ///// </summary>
    ///// <param name="ex">The exception.</param>
    ///// <param name="text">The text to be added to the result, optional, when not informed the exception message is used.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static ObjectResult ApplicationError(Exception ex, string? text = null)
    //    => new(BaseResult.ApplicationError(ex, text))
    //    {
    //        StatusCode = StatusCodes.Status500InternalServerError
    //    };

    ///// <summary>
    ///// Creates a failed result with status code 404 (Not Found).
    ///// </summary>
    ///// <param name="message">The error message.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static NotFoundObjectResult NotFound(string message)
    //    => new(BaseResult.NotFound(message));

    ///// <summary>
    ///// Creates a failed result with status code 403 (Forbidden).
    ///// </summary>
    ///// <param name="message">The error message.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static ObjectResult Forbidden(string message) => new(BaseResult.Forbidden(message))
    //{
    //    StatusCode = StatusCodes.Status403Forbidden
    //};

    ///// <summary>
    ///// Creates a failed result with status code 400 (Bad Request).
    ///// </summary>
    ///// <param name="message">The error message.</param>
    ///// <param name="property">The name of the property that generated the error, optional.</param>
    ///// <param name="ex">The exception that generated the error, optional.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static BadRequestObjectResult BadRequest(string message, string? property = null, Exception? ex = null)
    //    => new(BaseResult.ValidationError(message, property, ex));

    ///// <summary>
    ///// Creates a <see cref="ObjectResult"/> from the <see cref="IOperationResult"/> giving the required status code. 
    ///// </summary>
    ///// <param name="statusCode">The status code.</param>
    ///// <param name="result">The operation result.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static ObjectResult StatusCode(int statusCode, IOperationResult result) 
    //    => new(result)
    //    {
    //        StatusCode = statusCode
    //    };

    ///// <summary>
    ///// Creates a <see cref="ObjectResult"/> with an error result and giving the required status code.
    ///// </summary>
    ///// <param name="statusCode">The status code.</param>
    ///// <param name="errorMessage">The error message.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static ObjectResult StatusCode(int statusCode, string errorMessage)
    //    => new(BaseResult.Failure(errorMessage))
    //    {
    //        StatusCode = statusCode
    //    };

    ///// <summary>
    ///// Creates a <see cref="ObjectResult"/> with a value and an error result and giving the required status code.
    ///// </summary>
    ///// <typeparam name="T">The type of the return object.</typeparam>
    ///// <param name="value">The return object.</param>
    ///// <param name="statusCode">The status code.</param>
    ///// <param name="errorMessage">The error message.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static ObjectResult StatusCode<T>(T value, int statusCode, string errorMessage)
    //    => new(ValueResult.Error(value, errorMessage))
    //    {
    //        StatusCode = statusCode
    //    };

    ///// <summary>
    ///// Creates a <see cref="ObjectResult"/> with a value and giving the required status code.
    ///// </summary>
    ///// <typeparam name="T">The type of the return object.</typeparam>
    ///// <param name="value">The return object.</param>
    ///// <param name="statusCode">The status code.</param>
    ///// <returns>The <see cref="ObjectResult" /> created.</returns>
    //public static ObjectResult StatusCode<T>(T value, int statusCode)
    //    => new(ValueResult.Create(value))
    //    {
    //        StatusCode = statusCode
    //    };

    //#endregion
//}
