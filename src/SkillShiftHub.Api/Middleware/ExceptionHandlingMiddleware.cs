using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using SkillShiftHub.Application.Exceptions;

namespace SkillShiftHub.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationAppException ex)
        {
            await WriteProblemAsync(context, ex.StatusCode, ex.Error, ex.Message, ex.Errors);
        }
        catch (AppException ex)
        {
            await WriteProblemAsync(context, ex.StatusCode, ex.Error, ex.Message, ex.Details);
        }
        catch (Exception ex)
        {
            var correlationId = context.Items.TryGetValue("CorrelationId", out var value) ? value?.ToString() : null;
            _logger.LogError(ex, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);
            await WriteProblemAsync(
                context,
                HttpStatusCode.InternalServerError,
                "InternalServerError",
                "Ocorreu um erro inesperado. Tente novamente mais tarde.",
                new
                {
                    correlationId,
                    diagnostics = _environment.IsDevelopment() ? ex.ToString() : null
                });
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode status, string error, string message, object? details)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var payload = new Dictionary<string, object?>
        {
            ["error"] = error,
            ["message"] = message
        };

        if (details is not null)
        {
            payload["details"] = details;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions));
    }
}

