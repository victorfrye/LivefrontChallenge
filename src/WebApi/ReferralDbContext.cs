using CartonCaps.ReferralsApi.WebApi.Models;

using Microsoft.EntityFrameworkCore;

namespace CartonCaps.ReferralsApi.WebApi;

internal class ReferralDbContext(DbContextOptions<ReferralDbContext> options) : DbContext(options)
{
    internal DbSet<Referral> Referrals => Set<Referral>();
    internal DbSet<Referee> Referees => Set<Referee>();
}
