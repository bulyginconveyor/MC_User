using user_service.infrastructure.http_clients.interfaces;

namespace user_service.infrastructure.http_clients.notification_service;

public static class ServicesExtensionNotificationService
{
    public static void AddNotificationService(this IServiceCollection services)
    {
        var address_notification_service = Environment.GetEnvironmentVariable("ADDRESS_NOTIFICATION_SERVICE");
        
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(address_notification_service);

        services.AddScoped<INotificationServiceREST, NotificationServiceREST>(
            ns => new NotificationServiceREST(httpClient));
    }
}
