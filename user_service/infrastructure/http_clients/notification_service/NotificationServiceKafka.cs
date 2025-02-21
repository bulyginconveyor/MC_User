using Confluent.Kafka;
using user_service.infrastructure.http_clients.interfaces;

namespace user_service.infrastructure.http_clients.notification_service;

public class NotificationServiceKafka(IProducer<Null, string> producer, string topic): INotificationService
{
    public async Task SendConfirmLink(string email, string code)
    {
        try
        {
            var deliveryReport = await producer.ProduceAsync(topic,
                new Message<Null, string> {Value = $"{email},{code}" });
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
        }

        producer.Flush();
    }
}
