using System.ComponentModel.DataAnnotations;
using System.Net;
using Newtonsoft.Json;
using UserService.BLL.Exceptions;

namespace UserService.API.Middleware
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
            catch (BadRequestException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
            }
            catch (TokenExpiredException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.Unauthorized);
            }
            catch (ValidationException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.UnprocessableEntity);
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

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}