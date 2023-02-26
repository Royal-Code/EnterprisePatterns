using RoyalCode.OperationResult.ProblemDetails.Convertion;

namespace RoyalCode.OperationResult.ProblemDetails;

/// <summary>
/// Converts the <see cref="IOperationResult"/> to <see cref="ProblemDetails"/>.
/// </summary>
public static class ProblemDetailsConverter
{

    public static Microsoft.AspNetCore.Mvc.ProblemDetails ToProblemDetails(
       this IOperationResult result, ProblemDetailsOptions options)
    {
        if (result.Success)
            return null!;

        var builder = new ProblemDetailsBuilder();
        foreach (var message in result.Messages)
        {
            ConvertMessage(message, builder);
        }

        if (builder.Code is null)
            return null!;

        // get the description
        options.Descriptor.TryGetDescription(builder.Code, out var description);
            
        // defini status code
        // como fazer?

    }

    private static void ConvertMessage(IResultMessage message, ProblemDetailsBuilder builder)
    {
        bool isGenericError = message.Code is null
            ? true
            : GenericErrorCodes.Contains(message.Code);
        var code = message.Code ?? GenericErrorCodes.InvalidParameters;

        builder.SetCode(code, isGenericError);

        if (isGenericError)
        {
            // se o codigo for de parâmetros inválidos, ou validação, então adicionar os detalhes
            if (code == GenericErrorCodes.InvalidParameters
                || code == GenericErrorCodes.Validation)
            {
                builder.AddInvalidParameter(new InvalidParameterDetails(message.Text)
                {
                    Name = message.Property,
                    Extensions = message.AdditionalInformation
                });
            }
            // se for not found
            else if (code == GenericErrorCodes.NotFound)
            {
                builder.AddNotFound(new NotFoundDetails(message.Text)
                {
                    Property = message.Property,
                    Extensions = message.AdditionalInformation
                });
            }
            // se não, adiciona erro interno
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
}
