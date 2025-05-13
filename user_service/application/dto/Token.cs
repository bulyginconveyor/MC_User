using user_service.services.jwt_authentification;

namespace user_service.application.dto;

public struct Token
{
    public string Value { get; set; }
    public long ExpiresSeconds { get; set; }
}
