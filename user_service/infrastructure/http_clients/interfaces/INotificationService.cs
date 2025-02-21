namespace user_service.infrastructure.http_clients.interfaces;

public interface INotificationService
{
    Task SendConfirmLink(string email, string code);
}
