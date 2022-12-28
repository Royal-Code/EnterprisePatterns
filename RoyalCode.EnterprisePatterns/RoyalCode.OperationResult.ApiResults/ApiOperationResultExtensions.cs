
using RoyalCode.OperationResult;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extensions for adapt <see cref="IOperationResult"/> to <see cref="IResult"/>.
/// </summary>
public static partial class ApiOperationResultExtensions
{
    /// <summary>
    /// Cria um sucesso padrão, sem mensagem.
    /// </summary>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult Ok() => Results.Ok();

    /// <summary>
    /// Cria um sucesso padrão, incluindo um objeto de retorno.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto de retorno.</typeparam>
    /// <param name="value">O objeto de retorno.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult Ok<T>(T value) => Results.Ok(ValueResult.CreateSuccess(value));

    /// <summary>
    /// Cria um sucesso padrão, incluindo um objeto de retorno e status code 201 (Created).
    /// </summary>
    /// <typeparam name="T">O tipo do objeto de retorno.</typeparam>
    /// <param name="uri">A URI para o recurso criado.</param>
    /// <param name="value">O objeto de retorno.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult Created<T>(string uri, T value)
        => Results.Created(uri, ValueResult.CreateSuccess(value));

    /// <summary>
    /// Cria um sucesso padrão, incluindo um objeto de retorno e status code 201 (Created).
    /// </summary>
    /// <typeparam name="T">O tipo do objeto de retorno.</typeparam>
    /// <param name="uri">A URI para o recurso criado.</param>
    /// <param name="value">O objeto de retorno.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult Created<T>(Uri uri, T value)
        => Results.Created(uri, ValueResult.CreateSuccess(value));


    /// <summary>
    /// Cria um erro interno a partir da exception.
    /// </summary>
    /// <param name="ex">A exception.</param>
    /// <param name="text">O texto a ser adicionado ao resultado, opcional, quando não informado é utilizada a mensagem da exception.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult ApplicationError(Exception ex, string? text = null)
        => Results.Json(BaseResult.ApplicationError(ex, text), statusCode: StatusCodes.Status500InternalServerError);

    /// <summary>
    /// Cria um resultado de falha com status code 404 (Not Found).
    /// </summary>
    /// <param name="message">A mensagem de erro.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult NotFound(string message)
        => Results.NotFound(BaseResult.NotFound(message));

    /// <summary>
    /// Cria um resultado de falha com status code 403 (Forbidden).
    /// </summary>
    /// <param name="message">A mensagem de erro.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult Forbidden(string message)
        => Results.Json(BaseResult.Forbidden(message), statusCode: StatusCodes.Status403Forbidden);

    /// <summary>
    /// Cria um resultado de falha com status code 400 (Bad Request).
    /// </summary>
    /// <param name="message">A mensagem de erro.</param>
    /// <param name="property">O nome da propriedade que gerou o erro, opcional.</param>
    /// <param name="ex">A exception que gerou o erro, opcional.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult BadRequest(string message, string? property = null, Exception? ex = null)
        => Results.BadRequest(BaseResult.ValidationError(message, property, ex));

    /// <summary>
    /// Cria um <see cref="IResult"/> a partir do <see cref="IOperationResult"/> informando o status code desejado.
    /// </summary>
    /// <param name="statusCode">O status code desejado.</param>
    /// <param name="result">O resultado da operação.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult StatusCode(int statusCode, IOperationResult result)
        => Results.Json(result, statusCode: statusCode);

    /// <summary>
    /// Cria um <see cref="IResult"/> com um resultado de erro e informando o status code desejado.
    /// </summary>
    /// <param name="statusCode">O status code desejado.</param>
    /// <param name="errorMessage">A mensagem de erro.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult StatusCode(int statusCode, string errorMessage)
        => Results.Json(BaseResult.CreateFailure(errorMessage), statusCode: statusCode);

    /// <summary>
    /// Cria um <see cref="IResult"/> com um resultado de erro, um objeto de retorno
    /// e informando o status code desejado.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto de retorno.</typeparam>
    /// <param name="value">O objeto de retorno.</param>
    /// <param name="statusCode">O status code desejado.</param>
    /// <param name="errorMessage">A mensagem de erro.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult StatusCode<T>(T value, int statusCode, string errorMessage)
        => Results.Json(ValueResult.CreateFailure(value, errorMessage), statusCode: statusCode);

    /// <summary>
    /// Cria um <see cref="IResult"/> com um resultado de sucesso, um objeto de retorno
    /// e informando o status code desejado.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto de retorno.</typeparam>
    /// <param name="value">O objeto de retorno.</param>
    /// <param name="statusCode">O status code desejado.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
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
    ///     Cria um <see cref="IResult" /> a partir do resultado da operação com o "Status Code" correspondente
    ///     aos valores do resultado.
    /// </summary>
    /// <param name="result">O resultado da operação.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
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
    ///     Cria um <see cref="IResult" /> a partir do resultado da operação com o "Status Code" correspondente.
    ///     aos valores do resultado.
    /// </para>
    /// <para>
    ///     Em caso de sucesso é returnado o <see cref="IResult" /> com o "Status Code" 201 (Created).
    /// </para>
    /// </summary>
    /// <param name="result">O resultado da operação.</param>
    /// <param name="uri">A URI para o recurso criado.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
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
    ///     Cria um <see cref="IResult" /> a partir do resultado da operação com o "Status Code" correspondente
    ///     aos valores do resultado.
    /// </summary>
    /// <param name="result">O resultado da operação.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
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
    ///     Cria um <see cref="IResult" /> a partir do resultado da operação com o "Status Code" correspondente.
    ///     aos valores do resultado.
    /// </para>
    /// <para>
    ///     Em caso de sucesso é returnado o <see cref="IResult" /> com o "Status Code" 201 (Created).
    /// </para>
    /// </summary>
    /// <param name="result">O resultado da operação.</param>
    /// <param name="uri">A URI para o recurso criado.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult ToHttpResult(this IOperationResult result, string? uri)
    {
        if (result.Success && uri is not null)
            return Results.Created(uri, result);
        return result.ToHttpResult();
    }

    /// <summary>
    ///     Cria um <see cref="IResult" /> a partir do resultado da operação com o "Status Code" correspondente
    ///     aos valores do resultado.
    /// </summary>
    /// <param name="result">O resultado da operação.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
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
    ///     Cria um <see cref="IResult" /> a partir do resultado da operação com o "Status Code" correspondente.
    ///     aos valores do resultado.
    /// </para>
    /// <para>
    ///     Em caso de sucesso é returnado o <see cref="IResult" /> com o "Status Code" 201 (Created).
    /// </para>
    /// </summary>
    /// <param name="result">O resultado da operação.</param>
    /// <param name="uri">A URI para o recurso criado.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult ToHttpResult<T>(this IOperationResult<T> result, string? uri)
    {
        if (result.Success && uri is not null)
            return Results.Created(uri, result);
        return result.ToHttpResult();
    }

    /// <summary>
    /// Cria um <see cref="IResult" /> com o "Status Code" 500 (Internal Server Error) a partir da exceção.
    /// </summary>
    /// <param name="exception">A exceção.</param>
    /// <param name="text">O texto a ser adicionado ao resultado, opcional, quando não informado é utilizada a mensagem da exception.</param>
    /// <returns>O <see cref="IResult" /> criado.</returns>
    public static IResult ToHttpResult(this Exception exception, string? text = null)
        => Results.Json(BaseResult.ApplicationError(exception, text), statusCode: StatusCodes.Status500InternalServerError);
}

#endif