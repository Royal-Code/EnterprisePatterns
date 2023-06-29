using Microsoft.AspNetCore.Http.Json;
using RoyalCode.OperationResults.Convertion;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RoyalCode.OperationResults.Tests.HttpTests;

/// <summary>
/// Tests of conversion of <see cref="ProblemDetailsExtended"/> to <see cref="OperationResult"/>.
/// </summary>
public class ProblemDetailsExtendedTests
{
    private static readonly JsonTypeInfo<ProblemDetailsExtended>
        JsonType = ProblemDetailsSerializer.DefaultProblemDetailsExtended;

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_SimpleError()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error message");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails, ProblemDetailsSerializer.JsonSerializerOptions);
        var extended = JsonSerializer.Deserialize(json, JsonType)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Single(resultErrors);
        var message = resultErrors.First();
        Assert.Equal("Error message", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        Assert.NotNull(message.Code);
    }

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_MultiplesErrors()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error message 1");
        result += ResultMessage.Error("Error message 2");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails, ProblemDetailsSerializer.JsonSerializerOptions);
        var extended = JsonSerializer.Deserialize(json, JsonType)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Equal(2, resultErrors.Count);
        
        var message = resultErrors.First();
        Assert.Equal("Error message 1", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        Assert.NotNull(message.Code);

        message = resultErrors.Last();
        Assert.Equal("Error message 2", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        Assert.NotNull(message.Code);
    }

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_SingleInvalidParameters()
    {
        // Arrange
        OperationResult result = ResultMessage.InvalidParameters("Error message", "Property");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails, ProblemDetailsSerializer.JsonSerializerOptions);
        var extended = JsonSerializer.Deserialize(json, JsonType)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Single(resultErrors);

        var message = resultErrors.First();
        Assert.Equal("Error message", message.Text);
        Assert.Equal("Property", message.Property);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        Assert.NotNull(message.Code);
    }

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_MultiplesInvalidParameters()
    {
        // Arrange
        OperationResult result = ResultMessage.InvalidParameters("Error message 1", "Property1");
        result += ResultMessage.InvalidParameters("Error message 2", "Property2");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails, ProblemDetailsSerializer.JsonSerializerOptions);
        var extended = JsonSerializer.Deserialize(json, JsonType)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Equal(2, resultErrors.Count);

        var message = resultErrors.First();
        Assert.Equal("Error message 1", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        Assert.NotNull(message.Code);

        message = resultErrors.Last();
        Assert.Equal("Error message 2", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        Assert.NotNull(message.Code);
    }

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_MultiplesErrorsAndInvalidParameters()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error message 1");
        result += ResultMessage.InvalidParameters("Error message 2", "Property2");
        result += ResultMessage.InvalidParameters("Error message 3", "Property3");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails, ProblemDetailsSerializer.JsonSerializerOptions);
        var extended = JsonSerializer.Deserialize(json, JsonType)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Equal(3, resultErrors.Count);

        var message = resultErrors.First();
        Assert.Equal("Error message 1", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        Assert.NotNull(message.Code);

        message = resultErrors.ElementAt(1);
        Assert.Equal("Error message 2", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        Assert.NotNull(message.Code);

        message = resultErrors.Last();
        Assert.Equal("Error message 3", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        Assert.NotNull(message.Code);
    }

    private static JsonSerializerOptions JsonSerializerOptions => new JsonOptions().SerializerOptions;

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_SingleNotFound()
    {
        // Arrange
        OperationResult result = ResultMessage.NotFound("Error message", "Property");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails, ProblemDetailsSerializer.JsonSerializerOptions);
        var extended = JsonSerializer.Deserialize(json, JsonType)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Single(resultErrors);

        var message = resultErrors.First();
        Assert.Equal("Error message", message.Text);
        Assert.Equal("Property", message.Property);
        Assert.Equal(HttpStatusCode.NotFound, message.Status);
        Assert.NotNull(message.Code);
    }

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_MultiplesNotFound()
    {
        // Arrange
        OperationResult result = ResultMessage.NotFound("Error message 1", "Property1");
        result += ResultMessage.NotFound("Error message 2", "Property2");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails, ProblemDetailsSerializer.JsonSerializerOptions);
        var extended = JsonSerializer.Deserialize(json, JsonType)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Equal(2, resultErrors.Count);

        var message = resultErrors.First();
        Assert.Equal("Error message 1", message.Text);
        Assert.Equal("Property1", message.Property);
        Assert.Equal(HttpStatusCode.NotFound, message.Status);
        Assert.NotNull(message.Code);

        message = resultErrors.Last();
        Assert.Equal("Error message 2", message.Text);
        Assert.Equal("Property2", message.Property);
        Assert.Equal(HttpStatusCode.NotFound, message.Status);
        Assert.NotNull(message.Code);
    }

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_MultiplesErrorsAndNotFound()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error message 1");
        result += ResultMessage.NotFound("Error message 2", "Property2");
        result += ResultMessage.NotFound("Error message 3", "Property3");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails, ProblemDetailsSerializer.JsonSerializerOptions);
        var extended = JsonSerializer.Deserialize(json, JsonType)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Equal(3, resultErrors.Count);

        var message = resultErrors.First();
        Assert.Equal("Error message 1", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        Assert.NotNull(message.Code);

        message = resultErrors.ElementAt(1);
        Assert.Equal("Error message 2", message.Text);
        Assert.Equal("Property2", message.Property);
        Assert.Equal(HttpStatusCode.NotFound, message.Status);
        Assert.NotNull(message.Code);

        message = resultErrors.Last();
        Assert.Equal("Error message 3", message.Text);
        Assert.Equal("Property3", message.Property);
        Assert.Equal(HttpStatusCode.NotFound, message.Status);
        Assert.NotNull(message.Code);
    }
}
