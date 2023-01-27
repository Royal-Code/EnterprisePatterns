using FluentAssertions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoyalCode.OperationResult.Tests;

public class SerializationTests
{
    [Fact]
    public void Serialize_BaseResult_Success()
    {
        // arrange
        var result = BaseResult.CreateSuccess();

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("{\"Messages\":[],\"Success\":true}");
    }

    [Fact]
    public void Serialize_BaseResult_Success_WithWarningMessage()
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
    public void Serialize_BaseResult_Success_WithInfoMessage()
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
    public void Serialize_BaseResult_Success_WithSuccessMessage()
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
    public void Serialize_BaseResult_Success_WithErrorMessage_Then_Became_Failure()
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
    public void Serialize_BaseResult_Success_WithException_Then_Became_Failure()
    {
        // arrange
        var result = BaseResult.CreateSuccess()
            .WithError(new Exception("Error message"));

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("""{"Messages":[{"Type":0,"Text":"Error message","Property":null,"Code":null,"Exception":{"Message":"Error message","StackTrace":null,"FullNameOfExceptionType":"System.Exception","InnerException":null}}],"Success":false}""");
    }

    [Fact]
    public void Serialize_WithContext_MustBeSameAs_ForWebAndWithoutNulls()
    {
        // arrange
        var result = BaseResult.CreateSuccess()
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
        var result = ValueResult.CreateSuccess(new SomeValue(12, "Some name"));

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("""{"Value":{"Id":12,"Name":"Some name"},"Messages":[],"Success":true}""");
    }

    [Fact]
    public void Serialize_ValueResult_Record()
    {
        // arrange
        var result = ValueResult.CreateSuccess(new SomeRecord(12, "Some name"));

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("""{"Value":{"Id":12,"Name":"Some name"},"Messages":[],"Success":true}""");
    }

    [Fact]
    public void Serialize_ValueResult_Struct()
    {
        // arrange
        var result = ValueResult.CreateSuccess(new SomeStruct { Id = 12, Name = "Some name" });

        // act
        var json = JsonSerializer.Serialize(result);

        // assert
        json.Should().Be("""{"Value":{"Id":12,"Name":"Some name"},"Messages":[],"Success":true}""");
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