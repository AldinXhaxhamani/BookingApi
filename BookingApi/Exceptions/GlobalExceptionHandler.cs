using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace BookingApi.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {

        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }


        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {

            //detajet ne server side
            _logger.LogError(
                exception,
                "Exception: {Message} | Path: {Path} | Method: {Method}",
                exception.Message,
                httpContext.Request.Path,
                httpContext.Request.Method
                );

            var details = GetExceptionDetails(exception);

            httpContext.Response.StatusCode = details.StatusCode;
            httpContext.Response.ContentType = "application/json";


            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(details), cancellationToken);

            return true;


        }


        private static ExceptionDetails GetExceptionDetails(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException =>
                    new ExceptionDetails
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Title = "Unauthorized",
                        Message = exception.Message
                    },

                ValidationException validationException =>
                   new ExceptionDetails
                   {
                       StatusCode = (int)HttpStatusCode.BadRequest,
                       Title = "Validation Error",
                       Message = string.Join(", ", validationException.Errors
                                        .Select(e => e.ErrorMessage))
                   },

                InvalidOperationException =>
                    new ExceptionDetails
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Title = "Bad Request",
                        Message = exception.Message
                    },

                KeyNotFoundException =>
                    new ExceptionDetails
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Title = "Not Found",
                        Message = exception.Message
                    },

                DbUpdateException =>
                    new ExceptionDetails
                     {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Title = "Database Error",
                        Message = "Invalid data. Please check all required fields."
                    },

                _ =>
                   new ExceptionDetails
                   {
                       StatusCode = (int)HttpStatusCode.InternalServerError,
                       Title = "Internal Server Error",
                       Message = "An unexpected error occurred."
                   }
            };







        }
    }
}
