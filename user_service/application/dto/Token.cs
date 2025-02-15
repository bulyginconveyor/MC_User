namespace user_service.application.dto;

public struct Token
{
    public string Value { get; set; }
    public int ExpiresSeconds { get; set; }
}
