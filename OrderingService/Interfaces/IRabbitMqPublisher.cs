namespace OrderingService.Interfaces;

public interface IRabbitMqPublisher
{
    void Publish<T>(T message, string queueName);
}