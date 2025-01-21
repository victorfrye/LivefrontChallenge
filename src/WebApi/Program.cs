using System.Text.Json;

using CartonCaps.ReferralsApi.WebApi;

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<ReferralDbContext>(connectionName: "db",
    configureSettings: static settings => settings.DisableRetry = true,
    configureDbContextOptions: static options =>
        options
            .UseSeeding((context, _) => context.SeedMockData())
            .UseAsyncSeeding(async (context, _, cancellationToken) => await context.SeedMockDataAsync(cancellationToken))
);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
    });


builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(static (document, _, _) =>
    {
        document.Info = new()
        {
            Title = "Carton Caps Referrals API",
            Version = "v1",
            Description = "Web API for managing referrals."
        };
        return Task.CompletedTask;
    });

    options.AddOperationTransformer(static (operation, _, _) =>
    {
        operation.Responses.Add("400", new OpenApiResponse { Description = "Bad request" });
        operation.Responses.Add("500", new OpenApiResponse { Description = "Internal server error" });
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapOpenApi().CacheOutput();

app.MapReferralEndpoints();

app.UseHttpsRedirection();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ReferralDbContext>();

    await dbContext.Database.EnsureCreatedAsync();
}

await app.RunAsync();
