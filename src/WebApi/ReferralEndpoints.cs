using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.ReferralsApi.WebApi;

public static class ReferralEndpoints
{
    public static void MapReferralEndpoints(this WebApplication app)
    {
        app.MapGetReferralsEndpoint();
        app.MapGetReferralByIdEndpoint();
        app.MapPostReferralEndpoint();
        app.MapPatchReferralStatusEndpoint();
        app.MapPutReferralEndpoint();
    }

    private static void MapGetReferralsEndpoint(this WebApplication app)
    {
        app.MapGet("/referrals", async (ReferralDbContext db, [FromQuery] string code, [FromQuery] ReferralStatus? status, CancellationToken cancellationToken) =>
        {
            var query = db.Referrals
                .Include(r => r.Referee)
                .Where(r => r.Code == code);

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            return Results.Ok(await query.ToListAsync(cancellationToken: cancellationToken));
        })
        .WithName("RetrieveReferrals")
        .WithSummary("Retrieve a collection of referrals.")
        .WithDescription("This GET method retrieves a collection of referrals based on query parameters. The 'code' parameter is required and filters referrals with the referrer's code. The 'status' parameter is optional and can be appended to filter referrals of a given status.");
    }

    private static void MapGetReferralByIdEndpoint(this WebApplication app)
    {
        app.MapGet("/referrals/{id:guid}", async (ReferralDbContext db, Guid id, CancellationToken cancellationToken) =>
        {
            var referral = await db.Referrals
                .Include(r => r.Referee)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken);

            return referral is null ? Results.NotFound() : Results.Ok(referral);
        })
        .WithName("RetrieveReferralById")
        .WithSummary("Retrieve a referral by its ID.")
        .WithDescription("This GET method retrieves an existing referral specified by the referral ID in the path. If no existing referral is found, a 404 response is returned.")
        .Produces<Referral>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }

    private static void MapPostReferralEndpoint(this WebApplication app)
    {
        app.MapPost("/referrals", async (ReferralDbContext db, Referral referral, CancellationToken cancellationToken) =>
        {
            var results = referral.Validate(new ValidationContext(referral));

            if (results.Any())
            {
                return Results.ValidationProblem(results.ToDictionary(
                    static result => result.MemberNames.FirstOrDefault() ?? string.Empty,
                    static result => new string[] { result.ErrorMessage ?? string.Empty }));
            }

            db.Referrals.Add(referral);
            await db.SaveChangesAsync(cancellationToken);
            return Results.Created($"/referrals/{referral.Id}", referral);
        })
        .WithName("CreateReferral")
        .WithSummary("Create a new referral.")
        .WithDescription("This POST method creates a new referral using the provided information. The request body must include the code from the referrer and referee first and last name. By default, the created referral has status 'Pending' but can be overridden if provided on the request body.")
        .Produces<Referral>(StatusCodes.Status201Created);
    }

    private static void MapPatchReferralStatusEndpoint(this WebApplication app)
    {
        app.MapPatch("/referrals/{id:guid}/status", async (ReferralDbContext db, Guid id, [FromBody] ReferralStatus status, CancellationToken cancellationToken) =>
        {
            var referral = await db.Referrals
                .Include(r => r.Referee)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken);

            if (referral is null)
            {
                return Results.NotFound();
            }

            referral.Status = status;
            await db.SaveChangesAsync(cancellationToken);

            return Results.Accepted($"/referrals/{referral.Id}", referral);
        })
        .WithName("UpdateReferralStatus")
        .WithSummary("Update the status of a referral.")
        .WithDescription("This PATCH method updates the status of an existing referral specified by the referral ID in the path. The request body must include the new status as a string for the referral. If no existing referral is found, a 404 response is returned.")
        .Produces<Referral>(StatusCodes.Status202Accepted)
        .Produces(StatusCodes.Status404NotFound);
    }

    private static void MapPutReferralEndpoint(this WebApplication app)
    {
        app.MapPut("/referrals/{id:guid}", async (ReferralDbContext db, Guid id, [FromBody] Referral referral, CancellationToken cancellationToken) =>
        {
            var existingReferral = await db.Referrals
                .Include(r => r.Referee)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken: cancellationToken);

            referral.Id = id;

            if (existingReferral is null)
            {
                var results = referral.Validate(new ValidationContext(referral));

                if (results.Any())
                {
                    return Results.ValidationProblem(results.ToDictionary(
                        static result => result.MemberNames.FirstOrDefault() ?? string.Empty,
                        static result => new string[] { result.ErrorMessage ?? string.Empty }));
                }

                db.Add(referral);
                return Results.Created($"/referrals/{referral.Id}", referral);
            }

            existingReferral = referral;

            await db.SaveChangesAsync(cancellationToken);
            return Results.Accepted($"/referrals/{referral.Id}", referral);
        })
        .WithName("UpdateReferral")
        .WithSummary("Create or update a referral by ID.")
        .WithDescription("This PUT method updates a referral specified by the referral ID in the path. The request body must include the full updated information for the referral. This operation will accept and override the existing referral with the new information. If no matching referral exists, it will be created with the provided information.")
        .Produces<Referral>(StatusCodes.Status201Created)
        .Produces<Referral>(StatusCodes.Status202Accepted);
    }
}
