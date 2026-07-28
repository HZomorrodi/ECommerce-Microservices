using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BusinessLogicLayer.RabbitMQ
{
    public class RabbitMQPublisher(IConnection connection) : IRabbitMQPublisher
    {
        public async Task Publish<T>(Dictionary<string, object> headers, T message)
        {
            await using IChannel channel = await connection.CreateChannelAsync();

            byte[] messageBodyInBytes = JsonSerializer.SerializeToUtf8Bytes(message);

            //Create exchange
            string exchangeName = Environment.GetEnvironmentVariable("RABBITMQ_Products_Exchange") ?? "products.exchange";
            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Headers, durable: true);
            BasicProperties properties = new()
            {
                Headers = headers
            };
            //Publish message
            await channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: string.Empty,
                mandatory: false,
                basicProperties: properties,
                body: messageBodyInBytes);
        }
    }
}
