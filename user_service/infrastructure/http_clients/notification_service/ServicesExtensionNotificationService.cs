using Confluent.Kafka;
using user_service.infrastructure.http_clients.interfaces;
using static System.String;

namespace user_service.infrastructure.http_clients.notification_service;

public static class ServicesExtensionNotificationService
{
    public static void AddNotificationServiceREST(this IServiceCollection services)
    {
        var address_notification_service = Environment.GetEnvironmentVariable("ADDRESS_NOTIFICATION_SERVICE");
        
        var httpClient = new HttpClient();
        if (IsNullOrWhiteSpace(address_notification_service))
            throw new Exception($"Не задан адрес сервиса уведомлений (ADDRESS_NOTIFICATION_SERVICE in .env)");
        httpClient.BaseAddress = new Uri(address_notification_service);

        services.AddScoped<INotificationService, NotificationServiceREST>(
            ns => new NotificationServiceREST(httpClient));
    }

    public static void AddNotificationServiceKafka(this IServiceCollection services)
    {
        var server = Environment.GetEnvironmentVariable("KAFKA_SERVER");
        var topicMain = Environment.GetEnvironmentVariable("KAFKA_TOPIC_MAIN");
        var topicSendToEmail = Environment.GetEnvironmentVariable("KAFKA_TOPIC_SENDTOEMAIL");
        
        if(IsNullOrEmpty(server) || IsNullOrEmpty(topicMain) || IsNullOrEmpty(topicSendToEmail))
            throw new Exception("Не указаны адреса серверов и топиков! Исправляй, сука!");
        
        var config = new ProducerConfig
        {
            BootstrapServers = server,
            AllowAutoCreateTopics = true,
            EnableIdempotence = true,
            Acks = Acks.All,
        };
        
        services.AddScoped<INotificationService>(s => new NotificationServiceKafka(config, topicMain, topicSendToEmail));
    }
}
