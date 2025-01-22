using System.Net.Http.Json;

using CartonCaps.ReferralsApi.WebApi.Models;

namespace CartonCaps.ReferralsApi.Tests;

public class ApiPutReferralByIdTests : BaseApiTests
{
    [Fact]
    public async Task PutMarkJohnsonReferralReturnsCreatedOrAcceptedStatus()
    {
        // Arrange
        var markJohnsonReferral = new Referral()
        {
            Code = "ABC123",
            Referee = new Referee()
            {
                FirstName = "Mark",
                LastName = "Johnson",
                Phone = "+1 (616) 706-1234",
                Email = "mjohnson@cartoncaps.com"
            }
        };

        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.PutAsJsonAsync($"/referrals/01948aac-40b8-7359-9df4-7e1cd6249e1d", markJohnsonReferral);

        // Assert
        var expectedStatusCodes = new HashSet<HttpStatusCode>() { HttpStatusCode.Created, HttpStatusCode.Accepted };
        Assert.Contains(response.StatusCode, expectedStatusCodes);
    }

    [Fact]
    public async Task PutReferralWithBadEmailReturnsBadRequestStatus()
    {
        // Arrange
        var badReferral = new Referral()
        {
            Code = "ABC123",
            Referee = new Referee()
            {
                FirstName = "Mark",
                LastName = "Johnson",
                Email = "bad email"
            }
        };

        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.PutAsJsonAsync($"/referrals/01948b2a-d04b-7cef-94a6-a8f385b1afc5", badReferral);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutReferralWithBadPhoneReturnsBadRequestStatus()
    {
        // Arrange
        var badReferral = new Referral()
        {
            Code = "ABC123",
            Referee = new Referee()
            {
                FirstName = "Mark",
                LastName = "Johnson",
                Phone = "bad phone"
            }
        };

        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.PutAsJsonAsync($"/referrals/01948b2a-b972-70d3-98a3-1c8d80760b7c", badReferral);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
