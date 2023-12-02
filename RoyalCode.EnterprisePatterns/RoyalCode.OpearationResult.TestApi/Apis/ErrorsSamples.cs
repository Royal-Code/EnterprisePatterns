
using RoyalCode.OperationResults.HttpResults;

namespace RoyalCode.OperationResults.TestApi.Apis;

public static class ErrorsSamples
{
    public static void MapErrors(this WebApplication app)
    {
        var group = app.MapGroup("/api/erros")
            .WithOpenApi()
            .WithDescription("Exemplos de erros.");

        group.MapGet("SingleError", SingleError)
            .WithName("SingleError")
            .WithDescription("Obtém um erro de BadRequest.")
            .WithOpenApi()
            .WithErrorResponseTypeHeader();

        group.MapGet("SingleInvalidParameters", SingleInvalidParameters)
            .WithName("SingleInvalidParameters")
            .WithDescription("Obtém um erro de BadRequest.")
            .WithOpenApi()
            .WithErrorResponseTypeHeader();

        group.MapGet("SingleNotFound", SingleNotFound)
            .WithName("SingleNotFound")
            .WithDescription("Obtém um erro de NotFound.")
            .WithOpenApi()
            .WithErrorResponseTypeHeader();

        group.MapGet("SingleValidationError", SingleValidationError)
            .WithName("SingleValidationError")
            .WithDescription("Obtém um erro de ValidationError.")
            .WithOpenApi()
            .WithErrorResponseTypeHeader();


    }

    private static OkMatch SingleError()
    {
        return ResultMessage.Error("invalid input");
    }

    private static OkMatch SingleInvalidParameters()
    {
        return ResultMessage.InvalidParameter("invalid input", "prop");
    }

    private static OkMatch SingleNotFound()
    {
        return ResultMessage.NotFound("not found", null);
    }

    private static OkMatch SingleValidationError()
    {
        return ResultMessage.ValidationError("invalid input", "prop");
    }
}