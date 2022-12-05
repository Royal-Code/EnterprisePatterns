
namespace RoyalCode.OperationResult;

/// <summary>
/// Extension methods for results and messages.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// It ensures that the result is success, otherwise it fires a <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>The same instance of <paramref name="result"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Case the result is not success.
    /// </exception>
    public static IOperationResult EnsureSuccess(this IOperationResult result)
    {
        if (result.Success)
            return result;

        throw result.Messages.CreateException();
    }

    /// <summary>
    /// It ensures that the result is success, otherwise it fires a <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <typeparam name="TValue">The result value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>The same instance of <paramref name="result"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Case the result is not success.
    /// </exception>
    public static IOperationResult<TValue> EnsureSuccess<TValue>(this IOperationResult<TValue> result)
    {
        if (result.Success)
            return result;

        throw result.Messages.CreateException();
    }

    private static Exception CreateException(this IEnumerable<IResultMessage> messages)
    {
        var exceptions = messages.Where(m => m.Type == ResultMessageType.Error)
            .Select(m => m.ToException())
            .ToList();

        Exception exception = exceptions.Count == 1
            ? exceptions.First()
            : new AggregateException("Multiple exceptions have occurred, check the internal exceptions to see the details.", exceptions);

        return exception;
    }

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
    public static IOperationResult<TValue> Adapt<TValue>(this IOperationResult result)
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
    public static IOperationResult<TValue> Adapt<TValue>(this IOperationResult result, TValue value)
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

    /// <summary>
    /// Adds a new message with its properties to the operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="type">The message type.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    public static void AddMessage(this BaseResult result,
        ResultMessageType type, string text, string? property, string? code, Exception? ex)
    {
        result.AddMessage(new ResultMessage(type, text, property, code, ex));
    }

    /// <summary>
    /// Adds a new success message with its properties to the operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    public static void AddSuccess(this BaseResult result, string text, string? property = null, string? code = null)
    {
        result.AddMessage(ResultMessage.Success(text, property, code));
    }

    /// <summary>
    /// Adds a new informative message with its properties to the operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    public static void AddInfo(this BaseResult result, string text, string? property = null, string? code = null)
    {
        result.AddMessage(ResultMessage.Info(text, property, code));
    }

    /// <summary>
    /// Adds a new warning message with its properties to the operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    public static void AddWarning(this BaseResult result, string text, string? property = null, string? code = null)
    {
        result.AddMessage(ResultMessage.Warning(text, property, code));
    }

    /// <summary>
    /// Adds a new error message with its properties to the operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    public static void AddError(this BaseResult result, string text, 
        string? property = null, string? code = null, Exception? ex = null)
    {
        result.AddMessage(ResultMessage.Error(text, property, code, ex));
    }

    /// <summary>
    /// Adds a new error message with its properties to the operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    public static void AddError(this BaseResult result, Exception ex, string? property = null, string? code = null)
    {
        result.AddMessage(ResultMessage.Error(ex, property, code));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message of type not found,
    ///     and with the message code <see cref="ResultErrorCodes.NotFound"/>.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    public static void AddNotFound(this BaseResult result, string text)
    {
        result.AddMessage(ResultMessage.NotFound(text));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message of type forbidden,
    ///     and with the message code <see cref="ResultErrorCodes.Forbidden"/>.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    public static void AddForbidden(this BaseResult result, string text)
    {
        result.AddMessage(ResultMessage.Forbidden(text));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message of type invalid parameters,
    ///     and with the message code <see cref="ResultErrorCodes.InvalidParameters"/>.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    public static void AddInvalidParameters(this BaseResult result, string text, string? property = null)
    {
        result.AddMessage(ResultMessage.InvalidParameters(text, property));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message of type validation error,
    ///     and with the message code <see cref="ResultErrorCodes.Validation"/>.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    public static void AddValidationError(this BaseResult result, 
        string text, string? property = null, Exception? ex = null)
    {
        result.AddMessage(ResultMessage.ValidationError(text, property, ex));
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message of type application error,
    ///     and with the message code <see cref="ResultErrorCodes.ApplicationError"/>.
    /// </para>
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="text">The message text, optional, when not informed the exception message will be used.</param>
    public static void AddApplicationError(this BaseResult result, Exception ex, string? text = null)
    {
        if (ex is null)
            throw new ArgumentNullException(nameof(ex));

        result.AddMessage(ResultMessage.ApplicationError(ex, text));
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
    public static TResult WithMessage<TResult>(this TResult result, IResultMessage message)
        where TResult : BaseResult
    {
        result.AddMessage(message);
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new success message and return the same operation result instance.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    public static TResult WithSuccess<TResult>(this TResult result, string text, 
        string? property = null, string? code = null)
        where TResult : BaseResult
    {
        result.AddMessage(ResultMessage.Success(text, property, code));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new informative message and return the same operation result instance.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    public static TResult WithInfo<TResult>(this TResult result,
        string text, string? property = null, string? code = null)
        where TResult : BaseResult
    {
        result.AddMessage(ResultMessage.Info( text, property, code));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new warning message and return the same operation result instance.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="code">The message code, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    public static TResult WithWarning<TResult>(this TResult result, string text, 
        string? property = null, string? code = null)
        where TResult : BaseResult
    {
        result.AddMessage(ResultMessage.Warning(text, property, code));
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
    /// <param name="ex">The exception that generate the message, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    public static TResult WithError<TResult>(this TResult result, string text, 
        string? property = null, string? code = null, Exception? ex = null)
        where TResult : BaseResult
    {
        result.AddMessage(ResultMessage.Error( text, property, code, ex));
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
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    public static TResult WithError<TResult>(this TResult result, Exception ex, 
        string? property = null, string? code = null)
        where TResult : BaseResult
    {
        result.AddMessage(ResultMessage.Error(ex, property, code));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message of type not found,
    ///     and with the message code <see cref="ResultErrorCodes.NotFound"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    public static TResult WithNotFound<TResult>(this TResult result, string text)
        where TResult : BaseResult
    {
        result.AddMessage(ResultMessage.NotFound(text));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message of type forbidden,
    ///     and with the message code <see cref="ResultErrorCodes.Forbidden"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    public static TResult WithForbidden<TResult>(this TResult result, string text)
        where TResult : BaseResult
    {
        result.AddMessage(ResultMessage.Forbidden(text));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message of type invalid parameters,
    ///     and with the message code <see cref="ResultErrorCodes.InvalidParameters"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    public static TResult WithInvalidParameters<TResult>(this TResult result, string text, string? property = null)
        where TResult : BaseResult
    {
        result.AddMessage(ResultMessage.InvalidParameters(text, property));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message of type validation error,
    ///     and with the message code <see cref="ResultErrorCodes.Validation"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="text">The message text.</param>
    /// <param name="property">The related property, optional.</param>
    /// <param name="ex">The exception that generate the message, optional.</param>
    /// <returns>The same instace of <paramref name="result"/>.</returns>
    public static TResult WithValidationError<TResult>(this TResult result, 
        string text, string? property = null, Exception? ex = null)
        where TResult : BaseResult
    {
        result.AddMessage(ResultMessage.ValidationError(text, property, ex));
        return result;
    }

    /// <summary>
    /// <para>
    ///     Adds a new error message of type application error,
    ///     and with the message code <see cref="ResultErrorCodes.ApplicationError"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The operation result type.</typeparam>
    /// <param name="result">The operation result.</param>
    /// <param name="ex">The exception that generate the message.</param>
    /// <param name="text">The message text, optional, when not informed the exception message will be used.</param>
    /// <returns>A mesma instância de <paramref name="result"/> para chamadas encadeadas.</returns>
    public static TResult WithApplicationError<TResult>(this TResult result, Exception ex, string? text = null)
        where TResult : BaseResult
    {
        if (ex is null)
            throw new ArgumentNullException(nameof(ex));

        result.AddMessage(ResultMessage.ApplicationError(ex, text));
        return result;
    }

    #endregion
}