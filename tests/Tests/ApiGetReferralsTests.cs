using System.Text.Json;

using CartonCaps.ReferralsApi.WebApi.Models;

namespace CartonCaps.ReferralsApi.Tests;

public class ApiGetReferralsTests : BaseApiTests
{
    [Fact]
    public async Task GetJaneDoeReferralsReturnsOkResult()
    {
        // Arrange
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.GetAsync("/referrals?code=XY7G4D");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonSerializer.Deserialize<IList<Referral>>(await response.Content.ReadAsStreamAsync(), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.All(result, r => Assert.Equal("XY7G4D", r.Code));
    }

    [Fact]
    public async Task GetJaneDoeCompleteReferralsReturnsOkResult()
    {
        // Arrange
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.GetAsync("/referrals?code=XY7G4D&status=Complete");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonSerializer.Deserialize<IList<Referral>>(await response.Content.ReadAsStreamAsync(), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.All(result, r => Assert.Equal("XY7G4D", r.Code));
        Assert.All(result, r => Assert.Equal(ReferralStatus.Complete, r.Status));
    }
}
