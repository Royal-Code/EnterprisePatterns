using System.Net;

namespace RoyalCode.OperationResults;

/// <summary>
/// Extension methods for results and messages.
/// </summary>
[Obsolete]
public static class ResultsExtensions
{
    /// <summary>
    /// <para>
    ///     Creates a new result of type <typeparamref name="TValue"/> from an existing result.
    /// </para>
    /// <para>
    ///     This result will fail because it does not contain the model.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>
    ///     A new instance of <see cref="IOperationResult{TValue}"/>.
    /// </returns>
    [Obsolete]
    public static IOperationResult<TValue> ToValue<TValue>(this IOperationResult result)
    {
        return new ValueResult<TValue>(result);
    }

    /// <summary>
    /// <para>
    ///     Creates a new result of type <typeparamref name="TValue"/> from an existing result.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="value">The operation result value.</param>
    /// <returns>
    ///     A new instance of <see cref="IOperationResult{TValue}"/>.
    /// </returns>
    [Obsolete]
    public static IOperationResult<TValue> ToValue<TValue>(this IOperationResult result, TValue value)
    {
        return new ValueResult<TValue>(value, result);
    }

    /// <summary>
    /// Creates a new result from this one, with the same messages, adapting the data model.
    /// </summary>
    /// <typeparam name="TValue">The type of operation result value.</typeparam>
    /// <typeparam name="TAdaptedValue">The adapted type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="adapter">The adapter.</param>
    /// <returns>
    ///     A new instance of <see cref="IOperationResult{TValue}"/>.
    /// </returns>
    [Obsolete]
    public static IOperationResult<TAdaptedValue> Adapt<TValue, TAdaptedValue>(
        this IOperationResult<TValue> result, Func<TValue, TAdaptedValue> adapter)
    {
        if (adapter is null)
            throw new ArgumentNullException(nameof(adapter));

        TAdaptedValue? newModel = result.Value is null ? default : adapter(result.Value);
        var newResult = new ValueResult<TAdaptedValue>(newModel);
        return newResult.Join(result);
    }

    #region Add Messages

    [Obsolete]
    private static ResultMessage AddMessage(this BaseResult result, ResultMessage message)
    {
        result.Add(message);
        return message;
    }

    /// <summary>
    /// Adds a new message with its properties to the operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage Add(this BaseResult result,
        string text, string? property, string? code, HttpStatusCode? status, Exception? ex)
    {
        return result.AddMessage(new ResultMessage(text, property, code, status, ex));
    }

    /// <summary>
    /// Adds a new error message with its properties to the operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddError(this BaseResult result, string? code, string text,
        string? property = null, HttpStatusCode? status = null, Exception? ex = null)
    {
        return result.AddMessage(ResultMessage.Error(code, text, property, status, ex));
    }

    /// <summary>
    /// Adds a new error message to the operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddError(this BaseResult result, string text, HttpStatusCode? status = null)
    {
        return result.AddMessage(ResultMessage.Error(text, status));
    }

    /// <summary>
    /// Adds a new error message with its properties to the operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddError(this BaseResult result, Exception ex, 
        string? property = null, string? code = null, HttpStatusCode? status = null)
    {
        return result.AddMessage(ResultMessage.Error(ex, property, code, status));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the code from <see cref="GenericErrorCodes.NotFound"/>
    ///     and HTTP status NotFound 404.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddNotFound(this BaseResult result, string text, string? property)
    {
        return result.AddMessage(ResultMessage.NotFound(text, property));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with a specified code
    ///     and HTTP status NotFound 404.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddNotFound(this BaseResult result, string code, string text, string? property)
    {
        return result.AddMessage(ResultMessage.NotFound(code, text, property));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with a specified code and HTTP status Forbidden 403.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddForbidden(this BaseResult result, string code, string text, string? property = null)
    {
        return result.AddMessage(ResultMessage.Forbidden(code, text, property));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with a specified code and HTTP status Conflict 409.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddConflict(this BaseResult result, string code, string text, string? property = null)
    {
        return result.AddMessage(ResultMessage.Conflict(code, text, property));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the code from <see cref="GenericErrorCodes.InvalidParameters"/> 
    ///     and HTTP status BadRequest 400.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddInvalidParameters(this BaseResult result, string text, string property)
    {
        return result.AddMessage(ResultMessage.InvalidParameter(text, property));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the code from <see cref="GenericErrorCodes.Validation"/> 
    ///     and HTTP status UnprocessableEntity 422.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddValidationError(this BaseResult result,
        string text, string property, Exception? ex = null)
    {
        return result.AddMessage(ResultMessage.ValidationError(text, property, ex));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the specified code and HTTP status UnprocessableEntity 422.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddValidationError(this BaseResult result,
        string code, string text, string property, Exception? ex = null)
    {
        return result.AddMessage(ResultMessage.ValidationError(code, text, property, ex));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the code from <see cref="GenericErrorCodes.Validation"/> 
    ///     and HTTP status UnprocessableEntity 422.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="ex">The exception that generate the message.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddValidationError(this BaseResult result, Exception ex)
    {
        return result.AddMessage(ResultMessage.ValidationError(ex));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the code from <see cref="GenericErrorCodes.ApplicationError"/>
    ///     and HTTP status InternalServerError 500.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="text">The message text, optional, when not informed the exception message will be used.</param>
    /// <returns>
    ///     The created message.
    /// </returns>
    [Obsolete]
    public static ResultMessage AddApplicationError(this BaseResult result, Exception ex, string? text = null)
    {
        return result.AddMessage(ResultMessage.ApplicationError(ex, text));
    }

    #endregion

    #region With Messages

    /// <summary>
    /// <para>
    ///     Adds a message and return the same operation result instance.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="message">The message.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithMessage<TResult>(this TResult result, IResultMessage message)
        where TResult : BaseResult
    {
        result.Add(message);
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message and return the same operation result instance.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithError<TResult>(this TResult result, string? code, string text,
        string? property = null, HttpStatusCode? status = null, Exception? ex = null)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.Error(code, text, property, status, ex));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message and return the same operation result instance.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithError<TResult>(this TResult result, string text, HttpStatusCode? status = null)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.Error(text, status));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message and return the same operation result instance.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="status">The HTTP status code, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithError<TResult>(this TResult result, 
        Exception ex, string? property = null, string? code = null, HttpStatusCode? status = null)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.Error(ex, property, code, status));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the code from <see cref="GenericErrorCodes.NotFound"/>
    ///     and HTTP status NotFound 404.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithNotFound<TResult>(this TResult result, string text, string? property)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.NotFound(text, property));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with a specified code
    ///     and HTTP status NotFound 404.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithNotFound<TResult>(this TResult result, string code, string text, string? property)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.NotFound(code, text, property));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with a specified code and HTTP status Forbidden 403.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithForbidden<TResult>(this TResult result, string code, string text, string? property = null)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.Forbidden(code, text, property));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with a specified code and HTTP status Conflict 409.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The property related, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithConflict<TResult>(this TResult result, string code, string text, string? property = null)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.Conflict(code, text, property));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the code from <see cref="GenericErrorCodes.InvalidParameters"/> 
    ///     and HTTP status BadRequest 400.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithInvalidParameters<TResult>(this TResult result, string text, string property)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.InvalidParameter(text, property));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the code from <see cref="GenericErrorCodes.Validation"/> 
    ///     and HTTP status UnprocessableEntity 422.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithValidationError<TResult>(this TResult result,
        string text, string property, Exception? ex = null)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.ValidationError(text, property, ex));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the specified code and HTTP status UnprocessableEntity 422.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="code">Some kind of code that can identify the type of message or error.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithValidationError<TResult>(this TResult result,
        string code, string text, string property, Exception? ex = null)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.ValidationError(code, text, property, ex));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the code from <see cref="GenericErrorCodes.Validation"/> 
    ///     and HTTP status UnprocessableEntity 422.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    [Obsolete]
    public static TResult WithValidationError<TResult>(this TResult result, Exception ex)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.ValidationError(ex));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message with the code from <see cref="GenericErrorCodes.ApplicationError"/>
    ///     and HTTP status InternalServerError 500.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="text">The message text, optional, when not informed the exception message will be used.</param>
    /// <returns>A mesma instância de <paramref name="result"/> para chamadas encadeadas.</returns>
    [Obsolete]
    public static TResult WithApplicationError<TResult>(this TResult result, Exception ex, string? text = null)
        where TResult : BaseResult
    {
        result.Add(ResultMessage.ApplicationError(ex, text));
        return result;
    }

    #endregion

    #region Has Error Code

    /// <summary>
    /// <para>
    ///     Checks if the result has a message with the InvalidParameter error code.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <returns><see langword="true"/> if the result has a message with the InvalidParameter error code, otherwise <see langword="false"/>.</returns>
    [Obsolete]
    public static bool HasInvalidParameters(this IOperationResult result)
    {
        return result.HasErrorCode(GenericErrorCodes.InvalidParameters);
    }

    /// <summary>
    /// <para>
    ///     Checks if the result has a message with the Validation error code.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <returns><see langword="true"/> if the result has a message with the Validation error code, otherwise <see langword="false"/>.</returns>
    [Obsolete]
    public static bool HasValidation(this IOperationResult result)
    {
        return result.HasErrorCode(GenericErrorCodes.Validation);
    }

    /// <summary>
    /// <para>
    ///     Checks if the result has a message with the NotFound error code.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <returns><see langword="true"/> if the result has a message with the NotFound error code, otherwise <see langword="false"/>.</returns>
    [Obsolete]
    public static bool HasNotFound(this IOperationResult result)
    {
        return result.HasErrorCode(GenericErrorCodes.NotFound);
    }

    /// <summary>
    /// <para>
    ///     Checks if the result has a message with the ApplicationError error code.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <returns><see langword="true"/> if the result has a message with the ApplicationError error code, otherwise <see langword="false"/>.</returns>
    [Obsolete]
    public static bool HasApplicationError(this IOperationResult result)
    {
        return result.HasErrorCode(GenericErrorCodes.ApplicationError);
    }

    #endregion

    
}