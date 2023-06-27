using RoyalCode.OperationResults.Convertion;
using System.Net;
using System.Text.Json;

namespace RoyalCode.OperationResults.Tests.HttpTests;

/// <summary>
/// Tests of conversion of <see cref="ProblemDetailsExtended"/> to <see cref="OperationResult"/>.
/// </summary>
public class ProblemDetailsExtendedTests
{
    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_SimpleError()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error message");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails);
        var extended = JsonSerializer.Deserialize<ProblemDetailsExtended>(json)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Single(resultErrors);
        var message = resultErrors.First();
        Assert.Equal("Error message", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
    }

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_MultiplesErrors()
    {
        // Arrange
        OperationResult result = ResultMessage.Error("Error message 1");
        result += ResultMessage.Error("Error message 2");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails);
        var extended = JsonSerializer.Deserialize<ProblemDetailsExtended>(json)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Equal(2, resultErrors.Count());
        
        var message = resultErrors.First();
        Assert.Equal("Error message 1", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
        
        message = resultErrors.Last();
        Assert.Equal("Error message 2", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
    }

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_SingleInvalidParameters()
    {
        // Arrange
        OperationResult result = ResultMessage.InvalidParameters("Error message", "Property");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails);
        var extended = JsonSerializer.Deserialize<ProblemDetailsExtended>(json)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Single(resultErrors);

        var message = resultErrors.First();
        Assert.Equal("Error message", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
    }

    [Fact]
    public void ToResultErrors_ShouldReturnResultErrors_For_MultiplesInvalidParameters()
    {
        // Arrange
        OperationResult result = ResultMessage.InvalidParameters("Error message 1", "Property1");
        result += ResultMessage.InvalidParameters("Error message 2", "Property2");
        result.TryGetError(out var error);
        var problemDetails = error!.ToProblemDetails(new ProblemDetailsOptions());
        var json = JsonSerializer.Serialize(problemDetails);
        var extended = JsonSerializer.Deserialize<ProblemDetailsExtended>(json)!;

        // Act
        var resultErrors = extended.ToResultErrors();

        // Assert
        Assert.NotNull(resultErrors);
        Assert.Equal(2, resultErrors.Count());

        var message = resultErrors.First();
        Assert.Equal("Error message 1", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);

        message = resultErrors.Last();
        Assert.Equal("Error message 2", message.Text);
        Assert.Equal(HttpStatusCode.BadRequest, message.Status);
    }
}
