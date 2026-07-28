using BusinessLogicLayer.DTO;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BusinessLogicLayer.RabbitMQ
{
    public class ProductNameUpdateConsumer(
        IConnection connection,
        ILogger<ProductNameUpdateConsumer> logger,
        IDistributedCache distributedCache) : BackgroundService
    {
        private readonly IConnection _connection = connection;
        private readonly ILogger<ProductNameUpdateConsumer> _logger = logger;

        private IChannel? _channel;
        private string? _consumerTag;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = await _connection.CreateChannelAsync();

            const string exchangeName = "products.header.exchange";
            const string queueName = "orders.product.update.name.queue";
            const string routingKey = "product.update.name";

            await _channel.ExchangeDeclareAsync(
                exchangeName,
                ExchangeType.Headers,
                durable: true);

            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: string.Empty,
                arguments: new Dictionary<string, object>
                {
                    ["x-match"] = "all",
                    ["event"] = "update",
                    ["entity"] = "product",
                    ["field"] = "name"
                });
            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: string.Empty,
                arguments: new Dictionary<string, object>
                {
                    ["x-match"] = "any",
                    ["event"] = "delete",
                    ["field"] = "name"
                });



            AsyncEventingBasicConsumer consumer = new(_channel);

            consumer.ReceivedAsync += OnMessageReceived;

            _consumerTag = await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer);

            _logger.LogInformation("RabbitMQ consumer started.");

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected during shutdown.
            }

        }
        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs args)
        {
            if (args.BasicProperties.Headers is null ||
                !args.BasicProperties.Headers.TryGetValue("event", out object? headerValue))
            {
                _logger.LogWarning("Message received without 'event' header.");
                return;
            }

            string eventName = Encoding.UTF8.GetString((byte[])headerValue);

            switch (eventName)
            {
                case "update":
                    {
                        ProductDTO? message =
                            JsonSerializer.Deserialize<ProductDTO>(args.Body.Span);

                        if (message != null)
                        {
                            await HandleProductNameUpdated(message);
                        }
                        break;
                    }

                case "delete":
                    {
                        ProductDeletionMessage? message =
                            JsonSerializer.Deserialize<ProductDeletionMessage>(args.Body.Span);

                        if (message != null)
                        {
                            await HandleProductDeleted(message);
                        }
                        break;
                    }

                default:
                    _logger.LogWarning("Unknown event '{Event}'", eventName);
                    break;
            }
        }
        private async Task HandleProductDeleted(ProductDeletionMessage message)
        {
            _logger.LogInformation(
    "Product {ProductId} deleted to {ProductName}",
    message.ProductId,
    message.ProductName);
            string cacheKey = $"product:{message.ProductId}";
            await distributedCache.RemoveAsync(cacheKey);

            // TODO:
            // Update MongoDB
        }

        private async Task HandleProductNameUpdated(ProductDTO productDTO)
        {
            string cacheKey = $"product:{productDTO.ProductId}";
            string productJson = JsonSerializer.Serialize(productDTO);
            DistributedCacheEntryOptions distributedCacheEntryOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300),
                SlidingExpiration = TimeSpan.FromSeconds(100)
            };

            await distributedCache.SetStringAsync(cacheKey, productJson, distributedCacheEntryOptions);
            _logger.LogInformation(
                "Product {ProductId} renamed to {ProductName}",
                productDTO.ProductId,
                productDTO.ProductName);
            // TODO:
            // Update MongoDB
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RabbitMQ consumer...");

            if (_channel != null && _consumerTag != null)
            {
                await _channel.BasicCancelAsync(_consumerTag);
                await _channel.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
