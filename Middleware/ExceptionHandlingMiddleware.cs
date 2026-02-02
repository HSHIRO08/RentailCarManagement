using System.Net;
using System.Text.Json;
using RentailCarManagement.DTOs.Common;
using RentailCarManagement.Exceptions;

namespace RentailCarManagement.Middleware;

/// <summary>
/// Global Exception Handler Middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message, (List<string>?)null),
            UnauthorizedException => (HttpStatusCode.Forbidden, exception.Message, null),
            ValidationException validationEx => (HttpStatusCode.BadRequest, validationEx.Message, validationEx.Errors),
            CarNotAvailableException => (HttpStatusCode.Conflict, exception.Message, null),
            InvalidRentalDateException => (HttpStatusCode.BadRequest, exception.Message, null),
            PaymentFailedException => (HttpStatusCode.PaymentRequired, exception.Message, null),
            BusinessException => (HttpStatusCode.BadRequest, exception.Message, null),
            _ => (HttpStatusCode.InternalServerError, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.", null)
        };

        response.StatusCode = (int)statusCode;

        var apiResponse = new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await response.WriteAsync(JsonSerializer.Serialize(apiResponse, options));
    }
}

/// <summary>
/// Extension method cho middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
