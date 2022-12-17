using FluentAssertions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoyalCode.OperationResult.Tests;

public class DeserializationTests
{
    [Fact]
    public void Deserialize_BaseResult_Success()
    {
        // arrange
        var result = BaseResult.CreateSuccess();
        var json = JsonSerializer.Serialize(result);

        // act
        var newResult = ResultsSerializeContext.Deserialize(json);

        // assert
        newResult.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void Deserialize_BaseResult_Success_WithWarningMessage()
    {
        // arrange
        var result = BaseResult.CreateSuccess();
        result.AddWarning("Warning message");
        var json = JsonSerializer.Serialize(result);

        // act
        var newResult = ResultsSerializeContext.Deserialize(json);

        // assert
        newResult.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void Deserialize_BaseResult_Success_WithException()
    {
        // arrange
        var result = BaseResult.CreateSuccess();
        result.AddError(new Exception("Exception message"));
        var json = JsonSerializer.Serialize(result);

        // act
        var newResult = ResultsSerializeContext.Deserialize(json);

        // assert
        newResult.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void Deserialize_ValueResult_Class()
    {
        // arrange
        var result = ValueResult.CreateSuccess(new SomeValue(12, "Some name"));
        var json = JsonSerializer.Serialize(result);

        // act
        var newResult = ResultsSerializeContext.Deserialize<SomeValue>(json);

        // assert
        newResult.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void Deserialize_ValueResult_Record()
    {
        // arrange
        var result = ValueResult.CreateSuccess(new SomeRecord(12, "Some name"));
        var json = JsonSerializer.Serialize(result);

        // act
        var newResult = ResultsSerializeContext.Deserialize<SomeRecord>(json);

        // assert
        newResult.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void Deserialize_ValueResult_Struct()
    {
        // arrange
        var result = ValueResult.CreateSuccess(new SomeStruct { Id = 12, Name = "Some name" });
        var json = JsonSerializer.Serialize(result);

        // act
        var newResult = ResultsSerializeContext.Deserialize<SomeStruct>(json);

        // assert
        newResult.Should().BeEquivalentTo(result);
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