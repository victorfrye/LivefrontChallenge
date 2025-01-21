using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;

namespace CartonCaps.ReferralsApi.Tests;

public class ApiDocumentIntegrationTests
{
    [Fact]
    public async Task GetApiOpenApiDocumentReturnsOkResult()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddStandardResilienceHandler());

        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        // Act
        var httpClient = app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.GetAsync("/openapi/v1.json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var reader = new OpenApiJsonReader();

        var result = reader.Read((MemoryStream)await response.Content.ReadAsStreamAsync(), new());

        Assert.NotNull(result.Document);
        Assert.Equal("Carton Caps Referrals API", result.Document.Info.Title);
        Assert.Equal("v1", result.Document.Info.Version);

        Assert.Equal(3, result.Document.Paths.Count);

        Assert.Contains(result.Document.Paths, p => p.Key == "/referrals");
        var referralsPath = result.Document.Paths["/referrals"];
        Assert.Equal(2, referralsPath.Operations.Count);
        Assert.Contains(referralsPath.Operations, o => o.Key == OperationType.Get);
        Assert.Contains(referralsPath.Operations, o => o.Key == OperationType.Post);

        Assert.Contains(result.Document.Paths, p => p.Key == "/referrals/{id}");
        var referralByIdPath = result.Document.Paths["/referrals/{id}"];
        Assert.Equal(2, referralByIdPath.Operations.Count);
        Assert.Contains(referralByIdPath.Operations, o => o.Key == OperationType.Get);
        Assert.Contains(referralByIdPath.Operations, o => o.Key == OperationType.Put);

        Assert.Contains(result.Document.Paths, p => p.Key == "/referrals/{id}/status");
        var referralStatusPath = result.Document.Paths["/referrals/{id}/status"];
        Assert.Single(referralStatusPath.Operations);
        Assert.Contains(referralStatusPath.Operations, o => o.Key == OperationType.Patch);
    }
}
