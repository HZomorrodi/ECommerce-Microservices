using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
namespace BusinessLogicLayer;

public static class DependencyInjection
{
    /// <summary>
    /// Extension method to add Business Logic  
    /// services to dependency injection container
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(ProductAddRequestToProductMappingProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();
        services.AddScoped<IProductService, ProductService>();
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
        services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();
        return services;
    }
}
