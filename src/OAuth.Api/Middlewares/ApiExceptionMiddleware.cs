using Microsoft.AspNetCore.Http;
using OAuth.Api.Models;
using OAuth.Core.Exceptions;
using System;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace OAuth.Api.Middlewares
{
    internal class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiExceptionMiddleware(RequestDelegate next) =>
            _next = next ?? throw new ArgumentNullException(nameof(next));

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
            ApiProblemDetails apiProblemDetails;
            switch (exception)
            {
                case TimeoutException tex:
                    context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                    apiProblemDetails = new ApiProblemDetails(context, tex);
                    break;

                case GatewayTimeoutException gte:
                    context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                    apiProblemDetails = new ApiProblemDetails(context, gte);
                    break;

                case ServiceUnavailableException sue:
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    apiProblemDetails = new ApiProblemDetails(context, sue);
                    break;

                case BadGatewayException bge:
                    context.Response.StatusCode = StatusCodes.Status502BadGateway;
                    apiProblemDetails = new ApiProblemDetails(context, bge);
                    break;

                case NotImplementedException nie:
                    context.Response.StatusCode = StatusCodes.Status501NotImplemented;
                    apiProblemDetails = new ApiProblemDetails(context, nie);
                    break;

                case UserException ue:
                    context.Response.StatusCode = StatusCodes.Status451UnavailableForLegalReasons;
                    apiProblemDetails = new ApiProblemDetails(context, ue);
                    break;

                case UnsupportedMediaTypeException umte:
                    context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                    apiProblemDetails = new ApiProblemDetails(context, umte);
                    break;

                case PayloadTooLargeException ptle:
                    context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                    apiProblemDetails = new ApiProblemDetails(context, ptle);
                    break;

                case PreconditionFailedException pfe:
                    context.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                    apiProblemDetails = new ApiProblemDetails(context, pfe);
                    break;

                case MethodNotAllowedException mnae:
                    context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    apiProblemDetails = new ApiProblemDetails(context, mnae);
                    break;

                case NotFoundException nfe:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    apiProblemDetails = new ApiProblemDetails(context, nfe);
                    break;

                case BadRequestException bre:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    apiProblemDetails = new ApiProblemDetails(context, bre);
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    apiProblemDetails = new ApiProblemDetails(context, exception);
                    break;
            }

            context.Response.ContentType = MediaTypeNames.Application.Json;
            var response = JsonSerializer.Serialize(apiProblemDetails, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(response);
        }
    }
}
