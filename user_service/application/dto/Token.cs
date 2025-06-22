using System.Text.Json.Serialization;
using user_service.services.jwt_authentification;

namespace user_service.application.dto;

public struct Token
{
    [JsonPropertyName("value")]
    public string Value { get; set; }
    [JsonPropertyName("expires_seconds")]
    public long ExpiresSeconds { get; set; }
}
