namespace user_service.application.dto;

public class AuthorizeDTO
{
    public Token AccessToken { get; set; }
    public Token RefreshToken { get; set; }
}
