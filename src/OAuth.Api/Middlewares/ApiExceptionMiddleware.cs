using Microsoft.AspNetCore.Http;
using OCCBPackage.Exceptions;
using OCCBPackage.Mvc;
using System;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace OAuth.Api.Middlewares
{
    internal class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ApiExceptionMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleApiExceptionAsync(context, ex);
            }
        }

        private Task HandleApiExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = exception switch
            {
                TimeoutException _ => StatusCodes.Status504GatewayTimeout,
                GatewayTimeoutException _ => StatusCodes.Status504GatewayTimeout,
                ServiceUnavailableException _ => StatusCodes.Status503ServiceUnavailable,
                BadGatewayException _ => StatusCodes.Status502BadGateway,
                NotImplementedException _ => StatusCodes.Status501NotImplemented,
                UnsupportedMediaTypeException _ => StatusCodes.Status415UnsupportedMediaType,
                PayloadTooLargeException _ => StatusCodes.Status413PayloadTooLarge,
                PreconditionFailedException _ => StatusCodes.Status412PreconditionFailed,
                MethodNotAllowedException _ => StatusCodes.Status405MethodNotAllowed,
                NotFoundException _ => StatusCodes.Status404NotFound,
                AuthenticationException _ => StatusCodes.Status401Unauthorized,
                BadRequestException _ => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError,
            };

            context.Response.ContentType = MediaTypeNames.Application.Json;

            var response = JsonSerializer.Serialize(new ApiProblemDetails(context, exception), _jsonSerializerOptions);

            return context.Response.WriteAsync(response);
        }
    }
}
