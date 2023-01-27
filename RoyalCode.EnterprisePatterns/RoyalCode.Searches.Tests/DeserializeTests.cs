using FluentAssertions;
using RoyalCode.Searches.Abstractions;
using System.ComponentModel;
using System.Text.Json;

namespace RoyalCode.Searches.Tests;

public class DeserializeTests
{
    [Fact]
    public void Deserialize_ResultList_With_Sorting_MustBeEquivalent()
    {
        // arrage
        var expected = new ResultList<TestModel>
        {
            Page = 1,
            Count = 1,
            ItemsPerPage = 1,
            Pages = 1,
            Sortings = new List<ISorting>
            {
                new Sorting
                {
                    OrderBy = "Name",
                    Direction = ListSortDirection.Ascending
                },
                new Sorting
                {
                    OrderBy = "Id",
                    Direction = ListSortDirection.Descending
                }
            },
            Items = new List<TestModel>
            {
                new TestModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Mateus"
                },
                new TestModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Marcos"
                },
                new TestModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Lucas"
                },
                new TestModel
                {
                    Id = Guid.NewGuid(),
                    Name = "João"
                }
            }
        };

        var json = JsonSerializer.Serialize(expected, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        // act
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = JsonSerializer.Deserialize<ResultList<TestModel>>(json, options);

        // assert
        result.Should().BeEquivalentTo(expected);
    }
}

public class TestModel
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
}