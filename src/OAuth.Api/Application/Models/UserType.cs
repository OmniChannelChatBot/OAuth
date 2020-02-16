using System.Text.Json.Serialization;

namespace OAuth.Api.Application.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserType
    {
        Person = 1,
        Company = 2,
        ExternalChannel = 3,
        CompanyMember = 4,
        Admin = 5
    }
}
