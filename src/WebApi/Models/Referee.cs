using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CartonCaps.ReferralsApi.WebApi.Models;

[Description("The individual who was referred.")]
public class Referee
{
    [Description("The identifier of the referee.")]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [JsonIgnore]
    public Referral? Referral { get; set; } = null;

    [Description("The first, or given, name of the referee.")]
    public required string FirstName { get; set; }

    [Description("The last, or family, name of the referee.")]
    public required string LastName { get; set; }

    [Description("The phone number of the referee.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Phone { get; set; }

    [Description("The email address of the referee.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; set; }
}
