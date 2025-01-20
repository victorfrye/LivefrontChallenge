using System.Text.Json.Serialization;

namespace CartonCaps.ReferralsApi.WebApi;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReferralStatus
{
    Pending,
    Complete,
    Expired
}