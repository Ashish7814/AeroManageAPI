using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Services.Interfaces;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Services.Implementation
{
    public class RabbitMQService : IMessageQueueService, IDisposable
    {
        private readonly ILogger<RabbitMQService> _logger;
        private IConnection _connection;
        private IChannel _channel;
        private readonly string _exchangeName = "airline.events";
        private readonly bool _isInitialized;

        public RabbitMQService(IConfiguration configuration, ILogger<RabbitMQService> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest",
                VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            try
            {
                _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

                // Declare exchange
                _channel.ExchangeDeclareAsync(
                    exchange: _exchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false
                ).GetAwaiter().GetResult();

                // Declare queues
                DeclareQueuesAsync().GetAwaiter().GetResult();

                _isInitialized = true;
                _logger.LogInformation("RabbitMQ connection established successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to establish RabbitMQ connection");
                _isInitialized = false;
            }
        }

        private async Task DeclareQueuesAsync()
        {
            // Flight Notifications Queue
            await _channel.QueueDeclareAsync(
                queue: "flight.notifications",
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            await _channel.QueueBindAsync("flight.notifications", _exchangeName, "flight.notification.*");

            // Gate Changes Queue
            await _channel.QueueDeclareAsync(
                queue: "flight.gate.changes",
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            await _channel.QueueBindAsync("flight.gate.changes", _exchangeName, "flight.gate.changed");

            // Flight Delays Queue
            await _channel.QueueDeclareAsync(
                queue: "flight.delays",
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            await _channel.QueueBindAsync("flight.delays", _exchangeName, "flight.delayed");

            // Status Changes Queue
            await _channel.QueueDeclareAsync(
                queue: "flight.status.changes",
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            await _channel.QueueBindAsync("flight.status.changes", _exchangeName, "flight.status.*");

            _logger.LogInformation("RabbitMQ queues declared and bound");
        }

        public async Task PublishFlightNotificationAsync(FlightNotificationDto notification)
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("RabbitMQ not initialized, skipping message publish");
                return;
            }

            try
            {
                var message = JsonSerializer.Serialize(notification);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json",
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                };

                await _channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: $"flight.notification.{notification.NotificationType.ToLower()}",
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation($"Published flight notification for flight {notification.FlightId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish flight notification");
            }
        }

        public async Task PublishGateChangeAsync(int flightId, string newGate, string reason)
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("RabbitMQ not initialized, skipping message publish");
                return;
            }

            try
            {
                var message = new
                {
                    FlightId = flightId,
                    NewGate = newGate,
                    Reason = reason,
                    Timestamp = DateTime.UtcNow
                };

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json"
                };

                await _channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: "flight.gate.changed",
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation($"Published gate change for flight {flightId} to gate {newGate}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish gate change");
            }
        }

        public async Task PublishFlightDelayAsync(int flightId, int delayMinutes, string reason)
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("RabbitMQ not initialized, skipping message publish");
                return;
            }

            try
            {
                var message = new
                {
                    FlightId = flightId,
                    DelayMinutes = delayMinutes,
                    Reason = reason,
                    Timestamp = DateTime.UtcNow
                };

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json"
                };

                await _channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: "flight.delayed",
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation($"Published flight delay for flight {flightId}: {delayMinutes} minutes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish flight delay");
            }
        }

        public async Task PublishFlightStatusChangeAsync(int flightId, string oldStatus, string newStatus)
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("RabbitMQ not initialized, skipping message publish");
                return;
            }

            try
            {
                var message = new
                {
                    FlightId = flightId,
                    OldStatus = oldStatus,
                    NewStatus = newStatus,
                    Timestamp = DateTime.UtcNow
                };

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json"
                };

                await _channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: $"flight.status.{newStatus.ToLower()}",
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation($"Published status change for flight {flightId}: {oldStatus} -> {newStatus}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish status change");
            }
        }

        public void StartConsuming()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("RabbitMQ not initialized, cannot start consuming");
                return;
            }

            try
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;

                        _logger.LogInformation($"Received message: {routingKey}");

                        // Process message based on routing key
                        ProcessMessage(routingKey, message);

                        // Acknowledge message
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message");
                        // Reject and requeue message
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                };

                // Start consuming from all queues
                _channel.BasicConsumeAsync(queue: "flight.notifications", autoAck: false, consumer: consumer).GetAwaiter().GetResult();
                _channel.BasicConsumeAsync(queue: "flight.gate.changes", autoAck: false, consumer: consumer).GetAwaiter().GetResult();
                _channel.BasicConsumeAsync(queue: "flight.delays", autoAck: false, consumer: consumer).GetAwaiter().GetResult();
                _channel.BasicConsumeAsync(queue: "flight.status.changes", autoAck: false, consumer: consumer).GetAwaiter().GetResult();

                _logger.LogInformation("Started consuming messages from RabbitMQ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start consuming messages");
            }
        }

        private void ProcessMessage(string routingKey, string message)
        {
            // This is where you would process incoming messages
            // For example, sending emails, SMS, updating external systems, etc.
            _logger.LogInformation($"Processing message with routing key: {routingKey}");

            // Example: Send email notification
            // Example: Update external display boards
            // Example: Trigger mobile push notifications
        }

        public void Dispose()
        {
            _channel?.CloseAsync().GetAwaiter().GetResult();
            _connection?.CloseAsync().GetAwaiter().GetResult();
            _logger.LogInformation("RabbitMQ connection closed");
        }
    }
}
