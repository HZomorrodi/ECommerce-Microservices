using BusinessLogicLayer.RabbitMQ;
using eCommerce.ordersMicroservice.BusinessLogicLayer.Mappers;
using eCommerce.ordersMicroservice.BusinessLogicLayer.Services;
using eCommerce.OrdersMicroservice.BusinessLogicLayer.ServiceContracts;
using eCommerce.OrdersMicroservice.BusinessLogicLayer.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;


namespace eCommerce.OrdersMicroservice.BusinessLogicLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
    {
        //TO DO: Add business logic layer services into the IoC container
        services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();

        services.AddAutoMapper(cfg => { }, typeof(OrderAddRequestToOrderMappingProfile).Assembly);

        services.AddScoped<IOrdersService, OrdersService>();
        string connectionStringTemplate = configuration.GetConnectionString("Redis")!;
        string connectionString = connectionStringTemplate
          .Replace("$REDIS_HOST", Environment.GetEnvironmentVariable("REDIS_HOST"))
          .Replace("$REDIS_PORT", Environment.GetEnvironmentVariable("REDIS_PORT"));
        services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
            });
        services.AddSingleton(sp =>
        {
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();

            ConnectionFactory factory = new()
            {
                HostName = configuration["RABBITMQ_HOST"] ?? "localhost",
                Port = int.Parse(configuration["RABBITMQ_PORT"] ?? "5672"),
                UserName = configuration["RABBITMQ_USER"] ?? "guest",
                Password = configuration["RABBITMQ_PASSWORD"] ?? "guest"
            };

            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });
        services.AddHostedService<ProductNameUpdateConsumer>();
        return services;
    }
}
