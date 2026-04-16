using System.Text.Json;
using FluentValidation;

namespace InventoryManagement.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Default to 500 Server Error for unexpected crashes
        var response = new
        {
            Status = StatusCodes.Status500InternalServerError,
            Message = "An internal server error occurred.",
            Errors = new Dictionary<string, string[]>()
        };

        // If it's a Validation error from our Application layer, format it nicely
        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            
            response = new
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "Validation Failed",
                // Group the errors by property name (e.g., "Quantity" -> ["Cannot be negative", "Must be > 0"])
                Errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key, 
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            };
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        var result = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(result);
    }
}