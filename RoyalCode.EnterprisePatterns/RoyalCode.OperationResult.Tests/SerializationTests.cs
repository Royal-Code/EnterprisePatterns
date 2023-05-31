using FluentAssertions;
using RoyalCode.OperationResults;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoyalCode.OperationResults.Tests;

public class SerializationTests
{
    [Fact]
    public void Serialize_BaseResult_Success()
    {
        // arrange
        var result = BaseResult.Create();

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("{}");
    }

    [Fact]
    public void Serialize_BaseResult_Success_WithErrorMessage_Then_Became_Failure()
    {
        // arrange
        var result = BaseResult.Create()
            .WithError("Error message");

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("""{"messages":[{"text":"Error message"}]}""");
    }

    [Fact]
    public void Serialize_BaseResult_Success_WithException_Then_Became_Failure()
    {
        // arrange
        var result = BaseResult.Create()
            .WithError(new Exception("Error message"));

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("""{"messages":[{"text":"Error message"}]}""");
    }

    [Fact]
    public void Serialize_WithContext_MustBeSameAs_ForWebAndWithoutNulls()
    {
        // arrange
        var result = BaseResult.Create()
            .WithError(new Exception("Error message"));

        // act
        var json = result.Serialize();
        var json2 = JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        // assert
        json.Should().Be(json2);
    }

    [Fact]
    public void Serialize_ValueResult_Class()
    {
        // arrange
        var result = ValueResult.Create(new SomeValue(12, "Some name"));

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("""{"value":{"id":12,"name":"Some name"}}""");
    }

    [Fact]
    public void Serialize_ValueResult_Record()
    {
        // arrange
        var result = ValueResult.Create(new SomeRecord(12, "Some name"));

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("""{"value":{"id":12,"name":"Some name"}}""");
    }

    [Fact]
    public void Serialize_ValueResult_Struct()
    {
        // arrange
        var result = ValueResult.Create(new SomeStruct { Id = 12, Name = "Some name" });

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("""{"value":{"id":12,"name":"Some name"}}""");
    }

    [Fact]
    public void Serialize_ResultWithTwoMessages_WithAllProperies()
    {
        // arrange
        var result = BaseResult.Create()
            .WithError(new Exception("Error message 1"), "Property1", "error-code-1", System.Net.HttpStatusCode.InternalServerError)
            .WithError(new Exception("Error message 2"), "Property2", "error-code-2", System.Net.HttpStatusCode.InternalServerError);

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("""{"messages":[{"text":"Error message 1","property":"Property1","code":"error-code-1"},{"text":"Error message 2","property":"Property2","code":"error-code-2"}]}""");
    }

    [Fact]
    public void Serialize_OperationResult_Failure()
    {
        // arrange
        OperationResult result = ResultMessage.Error("Error message");

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("""{"messages":[{"text":"Error message"}]}""");
    }

    [Fact]
    public void Serialize_OperationResult_Success()
    {
        // arrange
        OperationResult result = new();

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("{}");
    }

    [Fact]
    public void Serialize_OperationResultWithValue_Failure()
    {
        // arrange
        OperationResult<string> result = ResultMessage.Error("Error message");

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("""{"messages":[{"text":"Error message"}]}""");
    }

    [Fact]
    public void Serialize_OperationResultWithValue_Success()
    {
        // arrange
        OperationResult<string> result = "Some value";

        // act
        var json = result.Serialize();

        // assert
        json.Should().Be("\"Some value\"");
    }
}


file class SomeValue
{
    public int Id { get; }

    public string Name { get; }

    [JsonConstructor]
    public SomeValue(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

file record SomeRecord(int Id, string Name);

file struct SomeStruct
{
    public int Id { get; set; }

    public string Name { get; set; }
}