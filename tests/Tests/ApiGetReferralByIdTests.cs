using System.Text.Json;

using CartonCaps.ReferralsApi.WebApi.Models;

namespace CartonCaps.ReferralsApi.Tests;

public class ApiGetReferralByIdTests : BaseApiTests
{
    [Fact]
    public async Task GetArcherKingReferralByIdReturnsOkResult()
    {
        // Arrange
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.GetAsync("/referrals/01948986-814b-7dc7-be3b-1542b8650112");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonSerializer.Deserialize<Referral>(await response.Content.ReadAsStreamAsync(), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(Guid.Parse("01948986-814b-7dc7-be3b-1542b8650112"), result.Id);
    }

    [Fact]
    public async Task GetNotExistingReferralByIdReturnsNotFoundResult()
    {
        // Arrange
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.GetAsync("/referrals/01948b2d-3808-7cfc-97b2-1aa833f459a5");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
