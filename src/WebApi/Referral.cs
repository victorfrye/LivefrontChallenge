using System.Text.Json.Serialization;

namespace CartonCaps.ReferralsApi.WebApi;

public class Referral
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required string Code { get; set; }

    [JsonIgnore]
    public Guid RefereeId { get; set; }

    public Referee Referee { get; set; } = null!;

    public ReferralStatus Status { get; set; } = ReferralStatus.Pending;
}