using CartonCaps.ReferralsApi.WebApi.Models;

using Microsoft.EntityFrameworkCore;

namespace CartonCaps.ReferralsApi.WebApi;

internal class ReferralDbContext(DbContextOptions<ReferralDbContext> options) : DbContext(options)
{
    public DbSet<Referral> Referrals => Set<Referral>();
    public DbSet<Referee> Referees => Set<Referee>();
}
