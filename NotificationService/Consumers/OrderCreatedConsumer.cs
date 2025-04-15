using System.Text;
using System.Text.Json;
using NotificationService.Messaging;
using NotificationService.Messaging.DTOs;
using NotificationService.Models;
using NotificationService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Consumers;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly INotificationService _notificationService;
    private IConnection? _connection;
    private IModel? _channel;

    public OrderCreatedConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
        InitRabbitMQ();
    }

    private void InitRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq" };

        // Försök ansluta med retry (kan ta några sekunder innan RabbitMQ är redo)
        const int maxRetries = 5;
        int retries = 0;
        while (retries < maxRetries)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: EventBusConstants.OrderCreatedQueue,
                                      durable: false,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);
                Console.WriteLine("[✓] Ansluten till RabbitMQ och OrderCreatedQueue deklarerad.");
                return;
            }
            catch (Exception ex)
            {
                retries++;
                Console.WriteLine($"[!] Misslyckad anslutning till RabbitMQ (försök {retries}): {ex.Message}");
                Thread.Sleep(3000); // Vänta 3 sek innan ny retry
            }
        }

        throw new Exception("Kunde inte ansluta till RabbitMQ efter flera försök.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

            if (orderEvent != null)
            {
                var notification = new NotificationMessage
                {
                    Type = "OrderCreated",
                    Recipient = orderEvent.UserId.ToString(),
                    Content = $"Order {orderEvent.OrderId} has been created.",
                    Timestamp = DateTime.UtcNow
                };
                await _notificationService.SendNotificationAsync(notification);
            }
        };

        _channel.BasicConsume(queue: EventBusConstants.OrderCreatedQueue,
                              autoAck: true,
                              consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
