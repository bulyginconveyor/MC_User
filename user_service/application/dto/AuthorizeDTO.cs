using System.Text.Json.Serialization;

namespace user_service.application.dto;

public class AuthorizeDTO
{
    [JsonPropertyName("access_token")]
    public Token AccessToken { get; set; }
    [JsonPropertyName("refresh_token")]
    public Token RefreshToken { get; set; }
}
