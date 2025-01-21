using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CartonCaps.ReferralsApi.WebApi;

[JsonConverter(typeof(JsonStringEnumConverter))]
[Description("The current status of the referral. The default value is 'Pending'.")]
public enum ReferralStatus
{
    Pending,
    Complete,
    Expired
}