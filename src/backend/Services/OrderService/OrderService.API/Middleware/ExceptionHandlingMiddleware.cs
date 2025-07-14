using System.Net;
using FluentValidation;
using Grpc.Core;
using Newtonsoft.Json;
using OrderService.Application.Exceptions;

namespace OrderService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
            }
            catch (ValidationException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.UnprocessableEntity);
            }
            catch (RpcException ex)
            {
                await HandleExceptionAsync(context, ex, MapRpcStatusCodeToHttp(ex.StatusCode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new { error = ex.Message };
            var jsonResponse = JsonConvert.SerializeObject(response);

            _logger.LogWarning("Request @{path} finished with code @{statusCode} and message @{message}", context.Request.Path, statusCode, ex.Message);

            await context.Response.WriteAsync(jsonResponse);
        }

        private HttpStatusCode MapRpcStatusCodeToHttp(StatusCode rpcSatusCode)
        {
            return rpcSatusCode switch
            {
                StatusCode.Cancelled => HttpStatusCode.Conflict,
                StatusCode.InvalidArgument => HttpStatusCode.BadRequest,
                StatusCode.NotFound => HttpStatusCode.NotFound,
                StatusCode.OutOfRange => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError,
            };
        }
    }
}
