using FluentAssertions;
using System.Text.Json;

namespace RoyalCode.OperationResult.Tests;

public class DeserializationTests
{
    [Fact]
    public void Deserializa_BaseResult_Success()
    {
        // arrange
        var result = BaseResult.CreateSuccess();
        var json = JsonSerializer.Serialize(result);

        // act
        var newResult = JsonSerializer.Deserialize<BaseResult>(json);

        // assert
        newResult.Should().BeEquivalentTo(result);
    }
}
