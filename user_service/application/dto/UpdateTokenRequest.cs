using System.Text.Json.Serialization;

namespace user_service.application.dto;

public class UpdateTokenRequest
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}
