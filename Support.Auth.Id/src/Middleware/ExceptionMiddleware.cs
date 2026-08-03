using System.Diagnostics;
using System.Text.Json;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.DTOs.Response;

namespace Support.Auth.Id.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            string traceId = Activity.Current?.Id ?? context.TraceIdentifier;

            if (
                ex is not NotFoundException
                && ex is not ConflictException
                && ex is not BusinessValidationException
            )
                _logger.LogError(
                    ex,
                    "CRITICAL ERROR: {Message}. TraceId: {TraceId}",
                    ex.Message,
                    traceId
                );
            else
                _logger.LogWarning(
                    "Validation/Business Rule Violated: {Message}. TraceId: {TraceId}",
                    ex.Message,
                    traceId
                );

            await HandleExceptionAsync(context, ex, traceId);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        string traceId
    )
    {
        int statuscode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,

            ConflictException => StatusCodes.Status409Conflict,

            UnauthorizedException => StatusCodes.Status401Unauthorized,

            BusinessValidationException => StatusCodes.Status400BadRequest,

            _ => StatusCodes.Status500InternalServerError,
        };

        string safeMessage =
            statuscode == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred on our servers."
                : exception.Message;

        ErrorResponse errorResponse = new()
        {
            Message = safeMessage,
            StatusCode = statuscode,
            TraceId = traceId,
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statuscode;

        await context.Response.WriteAsJsonAsync(JsonSerializer.Serialize(errorResponse));
    }
}
