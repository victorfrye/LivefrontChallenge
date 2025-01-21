using System.Net.Http.Json;
using System.Text.Json;

using Aspire.Hosting;

using CartonCaps.ReferralsApi.WebApi.Models;

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;

namespace CartonCaps.ReferralsApi.Tests;

public class ApiIntegrationTests : IAsyncLifetime
{
    private DistributedApplication _app = null!;

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();

        _app = await appHost.BuildAsync();
        await _app.StartAsync();
    }

    public async Task DisposeAsync() => await _app.DisposeAsync();

    // MARK: Health Checks

    [Fact]
    public async Task GetApiLivenessReturnsOkStatusCode()
    {
        // Arrange
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.GetAsync("/alive");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetApiHealthReturnsOkStatusCode()
    {
        // Arrange
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
        await resourceNotificationService.WaitForResourceAsync("api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // MARK: OpenAPI Document

    [Fact]
    public async Task GetApiOpenApiDocumentReturnsOkResult()
    {
        // Arrange
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = _app.CreateHttpClient("api");
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

    // MARK: GET /referrals

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

    // MARK: GET /referrals/{id}

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

    // MARK: POST /referrals

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

    // MARK: PATCH /referrals/{id}/status

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

    // MARK: PUT /referrals/{id}

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
