namespace user_service.infrastructure.http_clients.interfaces;

public interface INotificationServiceREST
{
    Task SendConfirmLink(string email, string code);
}
