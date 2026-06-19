using System.Diagnostics;
using System.Security.Claims;

namespace ShopRestApi.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        private const string CorrelationIdHeader = "X-Correlation-Id";

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // =========================
            // 1. Correlation ID
            // =========================
            var correlationId = GetOrCreateCorrelationId(context);
            context.Response.Headers[CorrelationIdHeader] = correlationId;

            // =========================
            // 2. User ID
            // =========================
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";

            // =========================
            // 3. IP Address
            // =========================
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var method = context.Request.Method;
            var path = context.Request.Path;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var statusCode = context.Response.StatusCode;
                var elapsed = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation(
                    "HTTP {Method} {Path} => {StatusCode} in {Elapsed}ms | UserId: {UserId} | IP: {IpAddress} | CorrelationId: {CorrelationId}",
                    method,
                    path,
                    statusCode,
                    elapsed,
                    userId,
                    ipAddress,
                    correlationId);
            }
        }

        private string GetOrCreateCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var existingId))
            {
                return existingId!;
            }

            return Guid.NewGuid().ToString();
        }
    }
}