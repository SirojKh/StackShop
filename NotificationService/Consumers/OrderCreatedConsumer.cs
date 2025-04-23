using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NotificationService.Interfaces;
using NotificationService.Messaging;
using NotificationService.Models;
using NotificationService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Events;
using Shared.Contracts.Interfaces;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly INotificationService _notificationService;
    private readonly IEmailClient _emailClient;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly IAuditLogger _auditLogger;
    private readonly IAnalyticsLogger _analytics;

    public OrderCreatedConsumer(INotificationService notificationService, IEmailClient emailClient, IAuditLogger auditLogger, IAnalyticsLogger analytics)
    {
        _analytics = analytics;
        _notificationService = notificationService;
        _emailClient = emailClient;
        _auditLogger = auditLogger;
        InitRabbitMQ();
    }


    private void InitRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq" };

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
                Console.WriteLine($"[NotificationService] Mottagit OrderCreatedEvent. OrderId: {orderEvent.OrderId}");

                var notification = new NotificationMessage
                {
                    Type = "OrderCreated",
                    Recipient = orderEvent.UserId.ToString(),
                    Content = $"Order {orderEvent.OrderId} has been created.",
                    Timestamp = DateTime.UtcNow
                };

                // Logga innan vi skickar e-post
                Console.WriteLine($"[NotificationService] Försöker skicka e-post till: {orderEvent.UserEmail}");

                try
                {
                    await _emailClient.SendEmailAsync(new EmailMessage
                    {
                        To = orderEvent.UserEmail,
                        Subject = "Your Order Has Been Created",
                        Body = $"Your order {orderEvent.OrderId} has been created successfully."
                    });
                    Console.WriteLine("[NotificationService] E-post skickades.");

                    // Logga till AuditService
                    await _auditLogger.LogAsync("EmailSent", orderEvent.UserId.ToString(),
                        $"E-post skickades till {orderEvent.UserEmail} (OrderId: {orderEvent.OrderId})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NotificationService] Ett fel uppstod när e-post skickades: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("[NotificationService] Fel: OrderCreatedEvent var null.");
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
