using FluentAssertions;
using System.Text.Json;

namespace RoyalCode.OperationResult.Tests;

public class SerializationTests
{
    [Fact]
    public void Serializa_BaseResult_Success()
    {
        // arrange
        var result = BaseResult.CreateSuccess();

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("{\"Messages\":[],\"Success\":true}");
    }

    [Fact]
    public void Serializa_BaseResult_Success_WithWarningMessage()
    {
        // arrange
        var result = BaseResult.CreateSuccess();
        result.AddWarning("Warning message");

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("""{"Messages":[{"Type":1,"Text":"Warning message","Property":null,"Code":null,"Exception":null}],"Success":true}""");
    }

    [Fact]
    public void Serializa_BaseResult_Success_WithInfoMessage()
    {
        // arrange
        var result = BaseResult.CreateSuccess()
            .WithInfo("Info message");

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("""{"Messages":[{"Type":2,"Text":"Info message","Property":null,"Code":null,"Exception":null}],"Success":true}""");
    }

    [Fact]
    public void Serializa_BaseResult_Success_WithSuccessMessage()
    {
        // arrange
        var result = BaseResult.CreateSuccess()
            .WithSuccess("Success message");

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("""{"Messages":[{"Type":3,"Text":"Success message","Property":null,"Code":null,"Exception":null}],"Success":true}""");
    }

    [Fact]
    public void Serializa_BaseResult_Success_WithErrorMessage_Then_Became_Failure()
    {
        // arrange
        var result = BaseResult.CreateSuccess()
            .WithError("Error message");

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("""{"Messages":[{"Type":0,"Text":"Error message","Property":null,"Code":null,"Exception":null}],"Success":false}""");
    }

    [Fact]
    public void Serializa_BaseResult_Success_WithException_Then_Became_Failure()
    {
        // arrange
        var result = BaseResult.CreateSuccess()
            .WithError(new Exception("Error message"));

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("""{"Messages":[{"Type":0,"Text":"Error message","Property":null,"Code":null,"Exception":{"Message":"Error message","StackTrace":null,"FullNameOfExceptionType":"System.Exception","InnerException":null}}],"Success":false}""");
    }
}