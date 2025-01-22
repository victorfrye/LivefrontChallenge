using System.Net.Http.Json;

using CartonCaps.ReferralsApi.WebApi.Models;

namespace CartonCaps.ReferralsApi.Tests;

public class ApiPostReferralTests : BaseApiTests
{
    [Fact]
    public async Task CreateAlexMillerReferralReturnsCreatedStatus()
    {
        // Arrange
        var alexMillerReferral = new Referral()
        {
            Code = "ABC123",
            Referee = new Referee()
            {
                FirstName = "Alex",
                LastName = "Miller",
                Phone = "+16167061234",
                Email = "amiller@cartoncaps.com"
            }
        };

        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.PostAsJsonAsync("/referrals", alexMillerReferral);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateReferralWithBadEmailReturnsBadRequestStatus()
    {
        // Arrange
        var badReferral = new Referral()
        {
            Code = "EM1234",
            Referee = new Referee()
            {
                FirstName = "Alex",
                LastName = "Miller",
                Email = "bad email",
            }
        };

        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.PostAsJsonAsync("/referrals", badReferral);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateReferralWithBadPhoneReturnsBadRequestStatus()
    {
        // Arrange
        var badReferral = new Referral()
        {
            Code = "PH1234",
            Referee = new Referee()
            {
                FirstName = "Alex",
                LastName = "Miller",
                Phone = "bad phone",
            }
        };

        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.PostAsJsonAsync("/referrals", badReferral);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
