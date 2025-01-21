using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CartonCaps.ReferralsApi.WebApi;

[Description("The principal object representing a referral from a referrer to a referee.")]
public class Referral : IValidatableObject
{
    [Description("The identifier of the referral.")]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [Description("The code from the referrer used to initiate the referral.")]
    public required string Code { get; set; }

    [JsonIgnore]
    public Guid RefereeId { get; set; }

    public Referee Referee { get; set; } = null!;

    public ReferralStatus Status { get; set; } = ReferralStatus.Pending;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Referee.Email is not null)
        {
            EmailAddressAttribute emailAddressAttribute = new();
            if (!emailAddressAttribute.IsValid(Referee.Email))
            {
                yield return new ValidationResult(
                    $"The value '{Referee.Email}' is not a valid email address.",
                    [nameof(Referee.Email)]);
            }
        }

        if (Referee.Phone is not null)
        {
            PhoneAttribute phoneAttribute = new();
            if (!phoneAttribute.IsValid(Referee.Phone))
            {
                yield return new ValidationResult(
                    $"The value '{Referee.Phone}' is not a valid phone number.",
                    [nameof(Referee.Phone)]);
            }
        }
    }
}
