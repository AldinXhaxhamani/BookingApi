using Booking.Application.Logging;
using Confluent.Kafka;
using System.Diagnostics;
using System.Security.Claims;
using Booking.Domain.Loggings;

namespace BookingApi.Logging
{
    public class RequestLoggingMiddleware
    {

        // next is the next middleware in the pipeline
        private readonly RequestDelegate _next;
        private readonly IKafkaLoggingProducer _loggingProducer;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            IKafkaLoggingProducer loggingProducer)
        {
            _next = next;
            _loggingProducer = loggingProducer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // start timer before request executes
            var stopwatch = Stopwatch.StartNew();

            // let the request continue through the pipeline
            await _next(context);

            // stop timer after response is sent
            stopwatch.Stop();

            // read userId from JWT if authenticated
            var userId = context.User
                .FindFirstValue("sub");

            var message = new LogMessageDto
            {
                EventType = "Request",
                Level = context.Response.StatusCode >= 500
                                ? "Error"
                                : context.Response.StatusCode >= 400
                                    ? "Warning"
                                    : "Information",
                Message = $"{context.Request.Method} " +
                             $"{context.Request.Path} " +
                             $"→ {context.Response.StatusCode}",
                Endpoint = context.Request.Path,
                HttpMethod = context.Request.Method,
                StatusCode = context.Response.StatusCode,
                DurationMs = stopwatch.ElapsedMilliseconds,
                UserId = userId != null
                                ? Guid.Parse(userId) : null,
                CreatedAtUtc = DateTime.UtcNow
            };


            // fire and forget 
            
            _ = _loggingProducer.ProduceAsync(message);
        }
    }

}

