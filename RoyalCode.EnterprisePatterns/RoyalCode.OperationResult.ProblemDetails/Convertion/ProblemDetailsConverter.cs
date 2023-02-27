﻿namespace RoyalCode.OperationResult.ProblemDetails.Convertion;

/// <summary>
/// Converts the <see cref="IOperationResult"/> to <see cref="ProblemDetails"/>.
/// </summary>
public static class ProblemDetailsConverter
{
    /// <summary>
    /// Convert the <paramref name="result"/> to <see cref="ProblemDetails"/>.
    /// </summary>
    /// <param name="result">The result to be converted.</param>
    /// <param name="options">The options to be used in the conversion.</param>
    /// <returns>A new instance of <see cref="ProblemDetails"/>.</returns>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails ToProblemDetails(
       this IOperationResult result, ProblemDetailsOptions options)
    {
        if (result.Success || result.ErrorsCount == 0)
            return null!;

        if (result.ErrorsCount == 1)
        {
            var message = result.Messages.First();
            return message.ToProblemDetails(options);
        }

        var builder = new ProblemDetailsBuilder();
        foreach (var message in result.Messages)
        {
            AddMessage(message, builder);
        }

        if (builder.Code is null)
            return null!;

        // get the description
        options.Descriptor.TryGetDescription(builder.Code, out var description);

        var status = description?.Status.HasValue ?? false
            ? (int)description.Status.Value
            : builder.GetStatusCode();

        // crete the problem details
        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Type = description?.Type ?? builder.Code.ToProblemDetailsType(options),
            Title = description?.Title ?? builder.Code,
            Detail = builder.GetDetail(),
            Status = status,
        };

        // add the extensions
        builder.WriteExtensions(problemDetails, options);

        return problemDetails;
    }

    private static void AddMessage(IResultMessage message, ProblemDetailsBuilder builder)
    {
        bool isGenericError = message.Code is null || GenericErrorCodes.Contains(message.Code);
        var code = message.Code ?? GenericErrorCodes.InvalidParameters;

        builder.SetCode(code, isGenericError);

        if (isGenericError)
        {
            // if the code is of invalid parameters, or validation, then add the details
            if (code == GenericErrorCodes.InvalidParameters
                || code == GenericErrorCodes.Validation)
            {
                builder.AddInvalidParameter(new InvalidParameterDetails(message.Text)
                {
                    Name = message.Property,
                    Extensions = message.AdditionalInformation
                }, code == GenericErrorCodes.Validation);
            }
            // if the code is not found
            else if (code == GenericErrorCodes.NotFound)
            {
                builder.AddNotFound(new NotFoundDetails(message.Text)
                {
                    Property = message.Property,
                    Extensions = message.AdditionalInformation
                });
            }
            // if not, add internal error
            else
            {
                builder.AddInternalErrorMessage(message.Text);
                if (message.AdditionalInformation is not null)
                    builder.AddExtension(message.AdditionalInformation);
            }
        }
        else
        {
            builder.AddCustomProblem(message);
        }
    }

    /// <summary>
    /// Convert one message to a problem details.
    /// </summary>
    /// <param name="message">The result message</param>
    /// <param name="options">The options for the conversion.</param>
    /// <returns>A new instance of <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/>.</returns>
    public static Microsoft.AspNetCore.Mvc.ProblemDetails ToProblemDetails(
       this IResultMessage message, ProblemDetailsOptions options)
    {
        var code = message.Code ?? GenericErrorCodes.InvalidParameters;

        // get the description
        options.Descriptor.TryGetDescription(code, out var description);

        int? status = description?.Status.HasValue ?? false
            ? (int)description.Status.Value
            : null;

        var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails()
        {
            Type = description?.Type ?? code.ToProblemDetailsType(options),
            Title = description?.Title ?? code,
            Detail = message.Text,
            Status = status
        };

        if (message.AdditionalInformation is not null)
            foreach (var (key, value) in message.AdditionalInformation)
                problem.Extensions.Add(key, value);

        return problem;
    }

    /// <summary>
    /// Convert a <see cref="IResultMessage.Code"/> 
    /// to the value for <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails.Type"/>.
    /// </summary>
    /// <param name="code">The code of the message.</param>
    /// <param name="options">The options for the conversion.</param>
    /// <returns>A string with the value for <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails.Type"/>.</returns>
    public static string ToProblemDetailsType(this string code, ProblemDetailsOptions options)
    {
        if (string.IsNullOrEmpty(code))
            return options.BaseAddress;

        return $"{options.BaseAddress}{options.TypeComplement}{code}";
    }
}