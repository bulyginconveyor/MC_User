using user_service.infrastructure.http_clients.interfaces;

namespace user_service.infrastructure.http_clients.notification_service;

public class NotificationServiceREST(HttpClient httpClient) : INotificationService
{
    public async Task SendConfirmLink(string email, string code)
        => await httpClient.PostAsync(
            $"/api/email_sender?registerDataEmail={email}&code={code}", 
            null
            );
}
