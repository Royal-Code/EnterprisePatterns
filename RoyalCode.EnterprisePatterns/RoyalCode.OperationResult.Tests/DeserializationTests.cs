using FluentAssertions;
using RoyalCode.OperationResult.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoyalCode.OperationResult.Tests;

public class DeserializationTests
{
    private readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Deserialize_BaseResult_Success()
    {
        // arrange
        var result = BaseResult.Create();
        var json = JsonSerializer.Serialize(result, options);

        // act
        var newResult = DeserializableResult.Deserialize(json);

        // assert
        newResult.Should().BeEquivalentTo<IOperationResult>(result);
    }

    [Fact]
    public void Deserialize_BaseResult_Success_WithException()
    {
        // arrange
        var result = BaseResult.Create();
        result.AddError(new Exception("Exception message"));
        var json = JsonSerializer.Serialize(result, options);

        // act
        var newResult = DeserializableResult.Deserialize(json);

        // assert
        newResult.Should().BeEquivalentTo<IOperationResult>(result, o =>
        {
            return o.ExcludingMissingMembers().For(r => r.Messages).Exclude(m => m.Exception);
        });
    }

    [Fact]
    public void Deserialize_ValueResult_Class()
    {
        // arrange
        var result = ValueResult.Create(new SomeValue(12, "Some name"));
        var json = JsonSerializer.Serialize(result, options);

        // act
        var newResult = DeserializableResult.Deserialize<SomeValue>(json);

        // assert
        newResult.Should().BeEquivalentTo<IOperationResult<SomeValue>>(result);
    }

    [Fact]
    public void Deserialize_ValueResult_Record()
    {
        // arrange
        var result = ValueResult.Create(new SomeRecord(12, "Some name"));
        var json = JsonSerializer.Serialize(result, options);

        // act
        var newResult = DeserializableResult.Deserialize<SomeRecord>(json);

        // assert
        newResult.Should().BeEquivalentTo<IOperationResult<SomeRecord>>(result);
    }

    [Fact]
    public void Deserialize_ValueResult_Struct()
    {
        // arrange
        var result = ValueResult.Create(new SomeStruct { Id = 12, Name = "Some name" });
        var json = JsonSerializer.Serialize(result, options);

        // act
        var newResult = DeserializableResult.Deserialize<SomeStruct>(json);

        // assert
        newResult.Should().BeEquivalentTo<IOperationResult<SomeStruct>>(result);
    }

    [Fact]
    public void Deserialize_Success_ValueResult_WithContext_MustBeSameAs_WithoutContext()
    {
        // arrange
        var result = ValueResult.Create(new SomeStruct { Id = 12, Name = "Some name" });
        var json = result.Serialize();

        // act
        var r1 = DeserializableResult.Deserialize<SomeStruct>(json);
        var r2 = JsonSerializer.Deserialize<DeserializableResult<SomeStruct>>(json, options)!;

        // assert
        r1.Should().BeEquivalentTo<IOperationResult<SomeStruct>>(r2);
    }

    [Fact]
    public void Deserialize_Failure_ValueResult_WithContext_MustBeSameAs_WithoutContext()
    {
        // arrange
        var result = ValueResult.Create(new SomeStruct { Id = 12, Name = "Some name" });
        result.WithValidationError("Some error", "some property");
        var json = result.Serialize();

        // act
        var r1 = DeserializableResult.Deserialize<SomeStruct>(json);
        var r2 = JsonSerializer.Deserialize<DeserializableResult<SomeStruct>>(json, options)!;

        // assert
        r1.Should().BeEquivalentTo<IOperationResult<SomeStruct>>(r2);
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