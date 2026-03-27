using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueueManagement.Application.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);   
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong: {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var exceptionType = exception.GetType();

            var (statusCode, result) = exception switch
            {
                ValidationException validationEx => (
                    HttpStatusCode.BadRequest,
                    JsonSerializer.Serialize(new { Error = "Validation Error", Details = validationEx.Errors })
                ),
                CustomException customEx => (
                    customEx.StatusCode,
                    JsonSerializer.Serialize(new { Error = exceptionType.Name.Replace("Exception", string.Empty), Details = customEx.Message })
                ),
                DomainExceptions domainEx => (
                    HttpStatusCode.UnprocessableEntity,
                    JsonSerializer.Serialize(new { Error = "Domain Validation Error", Details = domainEx.Message })
                ),
                _ => (
                    HttpStatusCode.InternalServerError,
                    JsonSerializer.Serialize(new { Error = "Internal Server Error", Details = exception.Message })
                )
            };

            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}
