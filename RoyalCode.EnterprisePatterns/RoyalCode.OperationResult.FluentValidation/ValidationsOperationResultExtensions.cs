using FluentValidation.Results;
using FluentValidation;
using System.Net;

namespace RoyalCode.OperationResult;

/// <summary>
/// Extensions methods for <see cref="IOperationResult"/> and <see cref="ValidationResult"/>.
/// </summary>
public static class ValidationsOperationResultExtensions
{
    /// <summary>
    /// Converts to a <see cref="IOperationResult"/>.
    /// </summary>
    /// <param name="result">FluentValidation result.</param>
    /// <returns>Operation Result.</returns>
    public static IOperationResult ToOperationResult(this ValidationResult result)
    {
        if (result.IsValid)
            return BaseResult.ImmutableSuccess;

        var br = BaseResult.CreateSuccess();

        result.Errors.AddErrorsTo(br);
        return br;
    }

    /// <summary>
    /// Converts to a <see cref="IOperationResult"/>.
    /// </summary>
    /// <param name="result">FluentValidation result</param>
    /// <param name="value">A value to be included into the result</param>
    /// <returns>Operation Result.</returns>
    public static IOperationResult ToOperationResult<TValue>(this ValidationResult result, TValue value)
    {
        var operationResult = new ValueResult<TValue>(value);

        if (result.IsValid)
            return operationResult;

        result.Errors.AddErrorsTo(operationResult);
        return operationResult;
    }

    /// <summary>
    /// Converts to a <see cref="BaseResult"/>.
    /// </summary>
    /// <param name="result">FluentValidation result</param>
    /// <returns>Operation Result.</returns>
    public static BaseResult ToBaseResult(this ValidationResult result)
    {
        if (result.IsValid)
            return new BaseResult();

        var br = BaseResult.CreateSuccess();

        result.Errors.AddErrorsTo(br);
        return br;
    }

    /// <summary>
    /// Converts to a <see cref="ValueResult{TValue}"/>.
    /// </summary>
    /// <param name="result">FluentValidation result</param>
    /// <param name="value">A value to be included into the result</param>
    /// <returns>Operation Result.</returns>
    public static ValueResult<TValue> ToValueResult<TValue>(this ValidationResult result, TValue value)
    {
        var operationResult = new ValueResult<TValue>(value);

        if (result.IsValid)
            return operationResult;

        result.Errors.AddErrorsTo(operationResult);
        return operationResult;
    }

    /// <summary>
    /// Merges the result of the FluentValidation with an Operation Result.
    /// </summary>
    /// <param name="operationResult">Operation Result.</param>
    /// <param name="validationResult">FluentValidation result</param>
    /// <returns>The same instance of <paramref name="operationResult"/>.</returns>
    public static BaseResult Join(this BaseResult operationResult, ValidationResult validationResult)
    {
        validationResult.Errors.AddErrorsTo(operationResult);
        return operationResult;
    }

    /// <summary>
    /// Merges the result of the FluentValidation with an Operation Result.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    /// <param name="operationResult">Operation Result.</param>
    /// <param name="validationResult">FluentValidation result</param>
    /// <returns>The same instance of <paramref name="operationResult"/>.</returns>
    public static ValueResult<T> Join<T>(this ValueResult<T> operationResult, ValidationResult validationResult)
    {
        validationResult.Errors.AddErrorsTo(operationResult);
        return operationResult;
    }

    /// <summary>
    /// Adds validation errors to the Operation Result.
    /// </summary>
    /// <param name="errors">FluentValidation errors.</param>
    /// <param name="result">Operation Result.</param>
    public static void AddErrorsTo(this IList<ValidationFailure> errors, BaseResult result)
    {
        for (var i = 0; i < errors.Count; i++)
        {
            var error = errors[i];
            switch (error.Severity)
            {
                case Severity.Error:
                    result.AddError(error.ErrorMessage, error.PropertyName, error.ErrorCode, HttpStatusCode.BadRequest);
                    break;
                case Severity.Warning:
                    result.AddWarning(error.ErrorMessage, error.PropertyName, error.ErrorCode);
                    break;
                case Severity.Info:
                    result.AddInfo(error.ErrorMessage, error.PropertyName, error.ErrorCode);
                    break;
                default:
                    result.AddError(error.ErrorMessage, error.PropertyName, error.ErrorCode, HttpStatusCode.BadRequest);
                    break;
            }
        }
    }
}
