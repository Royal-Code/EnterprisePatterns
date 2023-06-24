using RoyalCode.OperationResults.TestApi.Application.ResultsModels;
using RoyalCode.OperationResults.Tests.ApiTests;

namespace RoyalCode.OperationResults.Tests.HttpTests;

public class HttpOperationResultTests : IClassFixture<AppFixture>
{
    private readonly HttpClient client;

    public HttpOperationResultTests(AppFixture app)
    {
        client = app.CreateDefaultClient();
    }

    [Fact]
    public async Task ToOperationResultAsync_ShouldReturnSuccessResult_WhenGetSimpleValues()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/results/GetSimpleValues");

        // Act
        var response = await client.SendAsync(request);
        var result = await response.ToOperationResultAsync<SimpleValues>();
        var success = result.TryGetValue(out var values);

        // Assert
        Assert.True(success);
        Assert.NotNull(values);
        Assert.True(values.Number > 0);
        Assert.NotNull(values.Text);
    }

    [Fact]
    public async Task ToOperationResultAsync_ShouldReturnSuccessResult_WhenGetSimpleValuesWithCreatedPath()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/results/GetSimpleValuesWithCreatedPath");

        // Act
        var response = await client.SendAsync(request);
        var result = await response.ToOperationResultAsync<SimpleValues>();
        var success = result.TryGetValue(out var values);

        // Assert
        Assert.True(success);
        Assert.NotNull(values);
        Assert.True(values.Number > 0);
        Assert.NotNull(values.Text);
    }

    [Fact]
    public async Task ToOperationResultAsync_ShouldReturnErrorResult_WhenGetSimpleValuesWithError()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/results/GetSimpleValuesWithError");

        // Act
        var response = await client.SendAsync(request);
        var result = await response.ToOperationResultAsync<SimpleValues>();
        var failure = result.TryGetError(out var error);

        // Assert
        Assert.True(failure);
        Assert.NotNull(error);
        Assert.Single(error);
        Assert.Equal("Erro ao obter valores simples.", error.First().Text);
    }
}
