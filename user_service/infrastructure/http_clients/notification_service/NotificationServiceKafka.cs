using Confluent.Kafka;
using user_service.infrastructure.http_clients.interfaces;

namespace user_service.infrastructure.http_clients.notification_service;

public class NotificationServiceKafka(ProducerConfig config, string topicMain, string topicSendToEmail): INotificationService
{
    public async Task SendConfirmLink(string email, string code)
    {
        using var producer = new ProducerBuilder<Null, string>(config).Build();
        
        try
        {
            var deliveryReport = await producer.ProduceAsync(topicSendToEmail,
                new Message<Null, string> {Value = $"{email},{code}" });
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
        }

        producer.Flush();
    }
}
