using Comprobantes.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Comprobantes.Api.Middleware;

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
        _logger.LogError(exception, "Ocurrió una excepción: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            ValidationException validationException => CreateValidationProblemDetails(validationException),
            ComprobanteNotFoundException notFoundException => CreateNotFoundProblemDetails(notFoundException),
            InvalidOperationException invalidOperationException => CreateBadRequestProblemDetails(invalidOperationException),
            DomainException domainException => CreateBadRequestProblemDetails(domainException),
            _ => CreateInternalServerErrorProblemDetails(exception)
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }

    private ProblemDetails CreateValidationProblemDetails(ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807",
            Title = "Validation Error",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = "Uno o más errores de validación ocurrieron.",
            Extensions = { ["errors"] = errors }
        };
    }

    private ProblemDetails CreateNotFoundProblemDetails(ComprobanteNotFoundException exception)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807",
            Title = "Resource Not Found",
            Status = (int)HttpStatusCode.NotFound,
            Detail = exception.Message
        };
    }

    private ProblemDetails CreateBadRequestProblemDetails(Exception exception)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807",
            Title = "Bad Request",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = exception.Message
        };
    }

    private ProblemDetails CreateInternalServerErrorProblemDetails(Exception exception)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807",
            Title = "Internal Server Error",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = "Ocurrió un error interno en el servidor. Por favor, contacte al administrador."
        };
    }
}