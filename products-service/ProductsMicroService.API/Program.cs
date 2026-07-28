using BusinessLogicLayer;
using DataAccessLayer;
using FluentValidation.AspNetCore;
using ProductsMicroService.API.APIEndpoints;
using ProductsMicroService.API.Middleware;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBusinessLogicLayer();
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.ConfigureHttpJsonOptions(
    options =>
    options.SerializerOptions.Converters.Add
    (new JsonStringEnumConverter()));
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
var app = builder.Build();

app.UseExceptionHandlingMiddleWare();
app.UseRouting();
app.UseCors();
// Add Endpoint that can serve the swagger.json
app.UseSwagger();
// Add swagger UI (interactive page to explore and test API Endpoint)
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); 
app.MapProductAPIEndpoints();
app.Run();
