using RoyalCode.OperationResults.Convertion;
using System.Net;

namespace RoyalCode.OperationResults.Tests;

public class ProblemDetailsConverterTests
{
    [Fact]
    public void ToProblemDetails_Should_ReturnProblemDetails_With_Status400_When_Error_WithoutStatus_And_WithoutCode()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error message");
        result.TryGetError(out var error);

        // Act
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.InvalidParametersType, problemDetails.Type);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.InvalidParametersTitle, problemDetails.Title);
        Assert.Equal("Error message", problemDetails.Detail);
        Assert.Equal(400, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_Should_ReturnProblemDetails_With_Status400_When_Error_WithoutStatus_And_WithCode()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error Code", "Error message");
        result.TryGetError(out var error);

        // Act
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal("Error message", problemDetails.Detail);
        Assert.Equal(400, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_Should_ReturnProblemDetails_With_DefinedStatus_And_DefaultDescriptions_When_Error_WithStatus400()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error message", HttpStatusCode.BadRequest);
        result.TryGetError(out var error);

        // Act
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.InvalidParametersType, problemDetails.Type);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.InvalidParametersTitle, problemDetails.Title);
        Assert.Equal("Error message", problemDetails.Detail);
        Assert.Equal(400, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_Should_ReturnProblemDetails_With_DefinedStatus_And_DefaultDescriptions_When_Error_WithStatus404()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error message", HttpStatusCode.NotFound);
        result.TryGetError(out var error);

        // Act
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.NotFoundType, problemDetails.Type);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.NotFoundTitle, problemDetails.Title);
        Assert.Equal("Error message", problemDetails.Detail);
        Assert.Equal(404, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_Should_ReturnProblemDetails_With_DefinedStatus_And_DefaultDescriptions_When_Error_WithStatus422()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error message", HttpStatusCode.UnprocessableEntity);
        result.TryGetError(out var error);

        // Act
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.ValidationType, problemDetails.Type);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.ValidationTitle, problemDetails.Title);
        Assert.Equal("Error message", problemDetails.Detail);
        Assert.Equal(422, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_Should_ReturnProblemDetails_With_DefinedStatus_And_DefaultDescriptions_When_Error_WithStatus500()
    {
        // Arrange
        OperationResult result = ResultMessage.Error(new ArgumentException("Error message"));
        result.TryGetError(out var error);

        // Act
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.ApplicationErrorType, problemDetails.Type);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.ApplicationErrorTitle, problemDetails.Title);
        Assert.Equal("Error message", problemDetails.Detail);
        Assert.Equal(500, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_Should_ReturnProblemDetails_With_CodeAsTypeAndTitle_When_DescriptionIsNotDefined()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("error-code", "Error message", HttpStatusCode.Conflict);
        result.TryGetError(out var error);
        var options = new ProblemDetailsOptions();

        // Act
        var problemDetails = error!.ToProblemDetails(options);

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal($"{options.BaseAddress}{options.TypeComplement}error-code", problemDetails.Type);
        Assert.Equal("error-code", problemDetails.Title);
        Assert.Equal("Error message", problemDetails.Detail);
        Assert.Equal(409, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_Should_ReturnProblemDetails_With_CodeAsType_And_DescriptionTitle()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("error-code", "Error message", HttpStatusCode.Conflict);
        result.TryGetError(out var error);
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription("error-code", "Error Title", "Error type description"));

        // Act
        var problemDetails = error!.ToProblemDetails(options);

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal($"{options.BaseAddress}{options.TypeComplement}error-code", problemDetails.Type);
        Assert.Equal("Error Title", problemDetails.Title);
        Assert.Equal("Error message", problemDetails.Detail);
        Assert.Equal(409, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_Should_ReturnProblemDetails_With_DescriptionTypeAndTitle()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("error-code", "Error message", HttpStatusCode.Conflict);
        result.TryGetError(out var error);
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription("error-code", "http://errors.com/error-type", "Error Title", "Error type description"));

        // Act
        var problemDetails = error!.ToProblemDetails(options);

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal("http://errors.com/error-type", problemDetails.Type);
        Assert.Equal("Error Title", problemDetails.Title);
        Assert.Equal("Error message", problemDetails.Detail);
        Assert.Equal(409, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_Should_ReturnProblemDetails_With_DescriptionStatus()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("error-code", "Error message", HttpStatusCode.BadRequest);
        result.TryGetError(out var error);
        var options = new ProblemDetailsOptions();
        options.Descriptor.Add(new ProblemDetailsDescription("error-code", "Error Title", "Error type description", HttpStatusCode.Conflict));

        // Act
        var problemDetails = error!.ToProblemDetails(options);

        // Assert
        Assert.NotNull(problemDetails);
        Assert.Equal(409, problemDetails.Status);
    }

    [Fact]
    public void ToProblemDetails_Should_HaveInvalidParamsExtraField_When_HaveManyInvalidParametersErrors()
    {
        // Arrange
        OperationResult result = ResultMessage.InvalidParameters("Mensagen 1", "FieldA");
        result += ResultMessage.InvalidParameters("Mensagen 2", "FieldB");
        result.TryGetError(out var error);

        // Act
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());

        // Assert

        Assert.NotNull(problemDetails);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.InvalidParametersType, problemDetails.Type);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.InvalidParametersTitle, problemDetails.Title);

        var extraFields = problemDetails.Extensions[ProblemDetailsDescriptor.InvalidParametersExtensionField] as List<InvalidParameterDetails>;
        Assert.NotNull(extraFields);
        Assert.Equal(2, extraFields.Count);

        Assert.Equal("FieldA", extraFields[0].Name);
        Assert.Equal("Mensagen 1", extraFields[0].Reason);

        Assert.Equal("FieldB", extraFields[1].Name);
        Assert.Equal("Mensagen 2", extraFields[1].Reason);
    }
}
