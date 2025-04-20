using System.Text;
using System.Text.Json;
using OrderingService.Interfaces;
using RabbitMQ.Client; // Se till att du har rätt namespace här

namespace OrderingService.Services
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly IModel _channel;

        public RabbitMqPublisher()
        {
            var factory = new ConnectionFactory { HostName = "rabbitmq" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
        }

        public void Publish<T>(T message, string queueName)
        {
            _channel.QueueDeclare(queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body);

            Console.WriteLine($"[Ordering] Event skickat till queue: {queueName}");
        }
    }
}