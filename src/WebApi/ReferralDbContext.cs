using Microsoft.EntityFrameworkCore;

namespace CartonCaps.ReferralsApi.WebApi;

public class ReferralDbContext(DbContextOptions<ReferralDbContext> options) : DbContext(options)
{
    public DbSet<Referral> Referrals => Set<Referral>();
    public DbSet<Referee> Referees => Set<Referee>();
}