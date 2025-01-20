using System.Text.Json.Serialization;

namespace CartonCaps.ReferralsApi.WebApi;

public class Referee
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [JsonIgnore]
    public Referral? Referral { get; set; } = null;

    public required string FirstName { get; set; }

    public required string LastName { get; set; }
}