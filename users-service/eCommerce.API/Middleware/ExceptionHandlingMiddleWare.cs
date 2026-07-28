using eCommerce.API.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace eCommerce.API.Middleware;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class ExceptionHandlingMiddleWare(RequestDelegate next, ILogger<ExceptionHandlingMiddleWare> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleWare> logger = logger;

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            if (ex.InnerException is not null)
            {
                logger.LogError("{ExceptionType} {ErrorMessage} ", ex.InnerException.GetType().FullName, ex.InnerException.Message);
            }
            else
            {
                logger.LogError("{ExceptionType} {ErrorMessage} ", ex.GetType().FullName, ex.Message);
            }
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsJsonAsync(new { ex.Message, Type = ex.GetType().FullName });
        }
    }
 }

// Extension method used to add the middleware to the HTTP request pipeline.
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleWare(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleWare>();
    }
}
