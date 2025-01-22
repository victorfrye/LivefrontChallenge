using Aspire.Hosting;

namespace CartonCaps.ReferralsApi.Tests;

public abstract class BaseApiTests : IAsyncLifetime
{
    protected DistributedApplication _app = null!;

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();

        _app = await appHost.BuildAsync();
        await _app.StartAsync();
    }

    public async Task DisposeAsync() => await _app.DisposeAsync();
}
