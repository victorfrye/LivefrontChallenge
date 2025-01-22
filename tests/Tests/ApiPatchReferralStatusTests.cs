using System.Net.Http.Json;

using CartonCaps.ReferralsApi.WebApi.Models;

namespace CartonCaps.ReferralsApi.Tests;

public class ApiPatchReferralStatusTests : BaseApiTests
{
    [Fact]
    public async Task UpdateHelenYangReferralStatusReturnsAcceptedStatus()
    {
        // Arrange
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.PatchAsJsonAsync("/referrals/01948987-0ddc-7591-9750-74604d5b3c28/status", ReferralStatus.Complete);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }

    [Fact]
    public async Task UpdateNotExistingReferralStatusReturnsNotFoundStatus()
    {
        // Arrange
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.PatchAsJsonAsync("/referrals/01948b2b-fb14-74be-bbdb-9cdcaac7cf04/status", ReferralStatus.Complete);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
