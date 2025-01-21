using Microsoft.EntityFrameworkCore;

namespace CartonCaps.ReferralsApi.WebApi;

public static class SeedExtensions
{
    private static readonly Guid JennyReferralGuid = Guid.Parse("01948986-41e1-7412-85c9-790850ef1d9e");
    private static readonly Guid JennyRefereeGuid = Guid.Parse("01948986-687c-7532-8f82-4b4c1134db3d");

    private static readonly Guid ArcherReferralGuid = Guid.Parse("01948986-814b-7dc7-be3b-1542b8650112");
    private static readonly Guid ArcherRefereeGuid = Guid.Parse("01948986-f2b5-775f-b50f-ac0a7c9ffb4a");

    private static readonly Guid HelenReferralGuid = Guid.Parse("01948987-0ddc-7591-9750-74604d5b3c28");
    private static readonly Guid HelenRefereeGuid = Guid.Parse("01948987-1ffb-7ab6-ab76-32d4eef94e3b");

    private static readonly string ReferralCode = "XY7G4D";

    public static void SeedMockData(this DbContext context)
    {
        var jennyReferral = context.Set<Referral>().FirstOrDefault(r => JennyReferralGuid == r.Id);

        if (jennyReferral is null)
        {
            context.Add(new Referral
            {
                Id = JennyReferralGuid,
                Code = ReferralCode,
                Referee = new Referee
                {
                    Id = JennyRefereeGuid,
                    FirstName = "Jenny",
                    LastName = "Smith"
                },
                Status = ReferralStatus.Complete,
            });

            context.SaveChanges();
        }

        var archerReferral = context.Set<Referral>().FirstOrDefault(r => ArcherReferralGuid == r.Id);

        if (archerReferral is null)
        {
            context.Add(new Referral
            {
                Id = ArcherReferralGuid,
                Code = ReferralCode,
                Referee = new Referee
                {
                    Id = ArcherRefereeGuid,
                    FirstName = "Archer",
                    LastName = "King"
                },
                Status = ReferralStatus.Complete,
            });

            context.SaveChanges();
        }

        var helenReferral = context.Set<Referral>().FirstOrDefault(r => HelenReferralGuid == r.Id);

        if (helenReferral is null)
        {
            context.Add(new Referral
            {
                Id = HelenReferralGuid,
                Code = ReferralCode,
                Referee = new Referee
                {
                    Id = HelenRefereeGuid,
                    FirstName = "Helen",
                    LastName = "Yang"
                },
                Status = ReferralStatus.Pending,
            });

            context.SaveChanges();
        }
    }

    public static async Task SeedMockDataAsync(this DbContext context, CancellationToken cancellationToken)
    {
        var jennyReferral = await context.Set<Referral>().FirstOrDefaultAsync(r => JennyReferralGuid == r.Id, cancellationToken: cancellationToken);

        if (jennyReferral is null)
        {
            context.Add(new Referral
            {
                Id = JennyReferralGuid,
                Code = ReferralCode,
                Referee = new Referee
                {
                    Id = JennyRefereeGuid,
                    FirstName = "Jenny",
                    LastName = "Smith"
                },
                Status = ReferralStatus.Complete,
            });

            await context.SaveChangesAsync(cancellationToken);
        }

        var archerReferral = await context.Set<Referral>().FirstOrDefaultAsync(r => ArcherReferralGuid == r.Id, cancellationToken: cancellationToken);

        if (archerReferral is null)
        {
            context.Add(new Referral
            {
                Id = ArcherReferralGuid,
                Code = ReferralCode,
                Referee = new Referee
                {
                    Id = ArcherRefereeGuid,
                    FirstName = "Archer",
                    LastName = "King"
                },
                Status = ReferralStatus.Complete,
            });

            await context.SaveChangesAsync(cancellationToken);
        }

        var helenReferral = await context.Set<Referral>().FirstOrDefaultAsync(r => HelenReferralGuid == r.Id, cancellationToken: cancellationToken);

        if (helenReferral is null)
        {
            context.Add(new Referral
            {
                Id = HelenReferralGuid,
                Code = ReferralCode,
                Referee = new Referee
                {
                    Id = HelenRefereeGuid,
                    FirstName = "Helen",
                    LastName = "Yang"
                },
                Status = ReferralStatus.Pending,
            });

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}