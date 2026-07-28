using eCommerce.API.Middleware;
using eCommerce.Core;
using eCommerce.Core.Mappers;
using eCommerce.Infrastructure;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCore();
builder.Services.AddInfrastructure();
builder.Services.AddControllers().AddJsonOptions(
    options =>
    options.JsonSerializerOptions.Converters.Add
    (new JsonStringEnumConverter()));
builder.Services.AddAutoMapper(cfg => { }, typeof(ApplicationUserMappingProfile).Assembly);
//FluentValidations
builder.Services.AddFluentValidationAutoValidation();
//Add API Explorer services
builder.Services.AddEndpointsApiExplorer();
//Add Swagger generation services to create swagger specification
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(buidler =>
    {
        buidler.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
WebApplication app = builder.Build();

app.UseExceptionHandlingMiddleWare();
app.UseRouting();
// Add Endpoint that can serve the swagger.json
app.UseSwagger();
// Add swagger UI (interactive page to explore and test API Endpoint)
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
