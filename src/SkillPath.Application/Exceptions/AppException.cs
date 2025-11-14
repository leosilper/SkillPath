using System;
using System.Collections.Generic;
using System.Net;

namespace SkillPath.Application.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(string error, string message, HttpStatusCode statusCode, object? details = null)
        : base(message)
    {
        Error = error;
        StatusCode = statusCode;
        Details = details;
    }

    public string Error { get; }
    public HttpStatusCode StatusCode { get; }
    public object? Details { get; }
}

public sealed class ValidationAppException : AppException
{
    public ValidationAppException(IDictionary<string, string[]> errors)
        : base("ValidationError", "One or more validation failures occurred.", HttpStatusCode.BadRequest, errors)
    {
        Errors = errors;
    }

    public IDictionary<string, string[]> Errors { get; }
}

public sealed class ConflictAppException : AppException
{
    public ConflictAppException(string message)
        : base("Conflict", message, HttpStatusCode.Conflict)
    {
    }
}

public sealed class NotFoundAppException : AppException
{
    public NotFoundAppException(string resource, string? message = null)
        : base("NotFound", message ?? $"{resource} not found.", HttpStatusCode.NotFound, new { resource })
    {
        Resource = resource;
    }

    public string Resource { get; }
}

public sealed class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string? message = null)
        : base("Unauthorized", message ?? "Token inválido ou ausente.", HttpStatusCode.Unauthorized)
    {
    }
}

public sealed class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string? message = null)
        : base("Forbidden", message ?? "Você não tem permissão para acessar este recurso.", HttpStatusCode.Forbidden)
    {
    }
}

