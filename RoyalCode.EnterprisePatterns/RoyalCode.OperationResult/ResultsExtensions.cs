
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

        var exceptions = result.Messages.Where(m => m.Type == MessageType.Error)
            .Select(m => m.ToInvalidOperationException())
            .ToList();

        Exception exception = exceptions.Count == 1
            ? exceptions.First()
            : new AggregateException("Multiple exceptions have occurred, check the internal exceptions to see the details.", exceptions);

        throw exception;
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

        throw new InvalidOperationException(result.Messages.JoinMessages(" - "));
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
    /// <typeparam name="TModel">The result value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="value">The operation result value.</param>
    /// <returns>
    ///     A new instance of <see cref="IOperationResult{TValue}"/>.
    /// </returns>
    public static IOperationResult<TModel> Adapt<TModel>(this IOperationResult result, TModel value)
    {
        return new ValueResult<TModel>(value, result);
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
}