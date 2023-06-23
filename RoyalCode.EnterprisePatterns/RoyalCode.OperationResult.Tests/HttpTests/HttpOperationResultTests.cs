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
}
