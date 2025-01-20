using Microsoft.EntityFrameworkCore;

namespace CartonCaps.ReferralsApi.WebApi;

public static class SeedExtensions
{
    private static readonly Guid JennyReferralGuid = Guid.Parse("019484a6-2d26-773a-a3cd-f73f90f25444");
    private static readonly Guid JennyRefereeGuid = Guid.Parse("019484a6-2d26-773a-a3cd-f73f90f25445");

    private static readonly Guid ArcherReferralGuid = Guid.Parse("019484a6-2d26-773a-a3cd-f73f90f25446");
    private static readonly Guid ArcherRefereeGuid = Guid.Parse("019484a6-2d26-773a-a3cd-f73f90f25447");

    private static readonly Guid HelenReferralGuid = Guid.Parse("019484a6-2d26-773a-a3cd-f73f90f25448");
    private static readonly Guid HelenRefereeGuid = Guid.Parse("019484a6-34ed-7e01-8042-2eb91e85ace9");

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
