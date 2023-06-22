using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace RoyalCode.OperationResults.Tests.ApiTests;

public class ControllersResultsTests : IClassFixture<AppFixture>
{
    private readonly HttpClient client;

    public ControllersResultsTests(AppFixture app)
    {
        client = app.CreateDefaultClient();
    }

    [Fact]
    public async Task GetSimpleValues_Returns_Ok()
    {
        // Act
        var response = await client.GetAsync("/ControllersResults/GetSimpleValues");
        
        // Assert StatusCode
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Act
        var json = await response.Content.ReadAsStringAsync();
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        
        // Assert Values
        Assert.NotNull(dictionary);
        Assert.Equal(2, dictionary.Count);
        Assert.True(dictionary.ContainsKey("number"));
        Assert.True(dictionary.ContainsKey("text"));
        Assert.Equal(JsonValueKind.Number, dictionary["number"].ValueKind);
        Assert.Equal(JsonValueKind.String, dictionary["text"].ValueKind);
    }

    [Fact]
    public async Task GetSimpleValuesWithCreatedPath_Returns_Created()
    {
        // Act
        var response = await client.GetAsync("/ControllersResults/GetSimpleValuesWithCreatedPath");
        
        // Assert StatusCode
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Act
        var json = await response.Content.ReadAsStringAsync();
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        
        // Assert Values
        Assert.NotNull(dictionary);
        Assert.Equal(2, dictionary.Count);
        Assert.True(dictionary.ContainsKey("number"));
        Assert.True(dictionary.ContainsKey("text"));
        Assert.Equal(JsonValueKind.Number, dictionary["number"].ValueKind);
        Assert.Equal(JsonValueKind.String, dictionary["text"].ValueKind);

        // Act
        var location = response.Headers.Location;

        // Assert location
        Assert.NotNull(location);
        Assert.Equal("/simple-values", location.OriginalString);
    }

    [Fact]
    public async Task GetSimpleValuesWithError_Returns_BadRequest()
    {
        // Act
        var response = await client.GetAsync("/ControllersResults/GetSimpleValuesWithError");
        
        // Assert StatusCode
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Act
        var json = await response.Content.ReadAsStringAsync();
        var collection = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json);
        
        // Assert Values
        Assert.NotNull(collection);
        Assert.Single(collection);

        var dictionary = collection.First();
        Assert.Single(dictionary);
        Assert.True(dictionary.ContainsKey("text"));
        Assert.Equal(JsonValueKind.String, dictionary["text"].ValueKind);
    }

    [Fact]
    public async Task GetSimpleValuesWithErrorWithCreatedPath_Returns_BadRequest()
    {
        // Act
        var response = await client.GetAsync("/ControllersResults/GetSimpleValuesWithErrorWithCreatedPath");
        
        // Assert StatusCode
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Act
        var json = await response.Content.ReadAsStringAsync();
        var collection = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json);
        
        // Assert Values
        Assert.NotNull(collection);
        Assert.Single(collection);

        var dictionary = collection.First();
        Assert.Single(dictionary);
        Assert.True(dictionary.ContainsKey("text"));
        Assert.Equal(JsonValueKind.String, dictionary["text"].ValueKind);
    }

    [Fact]
    public async Task GetSimpleValuesWithCreatedPathAndFormat_Returns_Created()
    {
        // Act
        var response = await client.GetAsync("/ControllersResults/GetSimpleValuesWithCreatedPathAndFormat");
        
        // Assert StatusCode
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Act
        var json = await response.Content.ReadAsStringAsync();
        var number = JsonSerializer.Deserialize<JsonElement>(json);
        
        // Assert Values
        Assert.Equal(JsonValueKind.Number, number.ValueKind);

        // Act
        var location = response.Headers.Location;

        // Assert location
        Assert.NotNull(location);
        var expected = $"/simple-values/{number.GetInt32()}";
        Assert.Equal(expected, location.OriginalString);
    }

    [Fact]
    public async Task GetSimpleValuesWithErrorWithCreatedPathAndFormat_Returns_BadRequest()
    {
        // Act
        var response = await client.GetAsync("/ControllersResults/GetSimpleValuesWithErrorWithCreatedPathAndFormat");
        
        // Assert StatusCode
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Act
        var json = await response.Content.ReadAsStringAsync();
        var collection = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json);
        
        // Assert Values
        Assert.NotNull(collection);
        Assert.Single(collection);

        var dictionary = collection.First();
        Assert.Single(dictionary);
        Assert.True(dictionary.ContainsKey("text"));
        Assert.Equal(JsonValueKind.String, dictionary["text"].ValueKind);
    }

    [Theory]
    [InlineData("", true)]
    [InlineData(null, true)]
    [InlineData("Error", false)]
    public async Task GetSimpleValuesIfValidInput(string? input, bool valid)
    {
        // Prepare
        var expected = valid ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        var queryString = input is null ? "" : $"?error={input}";

        // Act
        var response = await client.GetAsync($"/ControllersResults/GetSimpleValuesIfValidInput{queryString}");
        var json = await response.Content.ReadAsStringAsync();

        // Assert StatusCode
        Assert.Equal(expected, response.StatusCode);
        
        // Assert Values
        if (valid)
        {
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            Assert.NotNull(dictionary);
            Assert.Equal(2, dictionary.Count);
            Assert.True(dictionary.ContainsKey("number"));
            Assert.True(dictionary.ContainsKey("text"));
            Assert.Equal(JsonValueKind.Number, dictionary["number"].ValueKind);
            Assert.Equal(JsonValueKind.String, dictionary["text"].ValueKind);
        }
        else
        {
            var collection = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json);

            Assert.NotNull(collection);
            Assert.Single(collection);

            var dictionary = collection.First();
            Assert.Equal(3, dictionary.Count);
            Assert.True(dictionary.ContainsKey("text"));
            Assert.Equal(JsonValueKind.String, dictionary["text"].ValueKind);
            Assert.True(dictionary.ContainsKey("property"));
            Assert.Equal(JsonValueKind.String, dictionary["property"].ValueKind);
            Assert.True(dictionary.ContainsKey("code"));
            Assert.Equal(JsonValueKind.String, dictionary["code"].ValueKind);
        }
    }

    [Fact]
    public async Task GetSimpleValues_Returns_Ok_WithProblemDetails()
    {
        // Prepare
        var message = new HttpRequestMessage(HttpMethod.Get, "/ControllersResults/GetSimpleValues");
        message.Headers.Add("X-Result", "ProblemDetails");

        // Act
        var response = await client.SendAsync(message);
        var json = await response.Content.ReadAsStringAsync();

        // Assert StatusCode
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert Values
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        Assert.NotNull(dictionary);
        Assert.Equal(2, dictionary.Count);
        Assert.True(dictionary.ContainsKey("number"));
        Assert.True(dictionary.ContainsKey("text"));
        Assert.Equal(JsonValueKind.Number, dictionary["number"].ValueKind);
        Assert.Equal(JsonValueKind.String, dictionary["text"].ValueKind);
    }

    [Fact]
    public async Task GetSimpleValuesWithError_Returns_BadRequest_WithProblemDetails()
    {
        // Prepare
        var message = new HttpRequestMessage(HttpMethod.Get, "/ControllersResults/GetSimpleValuesWithError");
        message.Headers.Add("X-Result", "ProblemDetails");

        // Act
        var response = await client.SendAsync(message);
        var json = await response.Content.ReadAsStringAsync();

        // Assert StatusCode
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Assert Values
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(json);
        Assert.NotNull(problemDetails);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.InvalidParametersTitle, problemDetails.Title);
        Assert.Equal("Erro ao obter valores simples.", problemDetails.Detail);
        Assert.Equal(400, problemDetails.Status);
        Assert.Equal(ProblemDetailsDescriptor.Defaults.InvalidParametersType, problemDetails.Type);
    }
}
