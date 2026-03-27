using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QueueManagement.API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        private string ParseDeviceName(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            // Kiểm tra các loại thiết bị phổ biến
            if (userAgent.Contains("iPhone")) return "iPhone";
            if (userAgent.Contains("iPad")) return "iPad";
            if (userAgent.Contains("Android")) return "Android Device";
            if (userAgent.Contains("Windows")) return "Windows PC";
            if (userAgent.Contains("Mac")) return "Mac";
            if (userAgent.Contains("Linux")) return "Linux";

            return "Unknown Device";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;
            var traceId = context.TraceIdentifier;
            var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var userAgent = request.Headers["User-Agent"].ToString();
            var deviceName = ParseDeviceName(userAgent);
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            // LOG 1: Request bắt đầu
            _logger.LogInformation("[REQUEST] {Method} {Path} | User: {UserId} | Device: {DeviceName} | IP: {IPAddress} | TraceId: {TraceId}",
                request.Method, request.Path, userId, deviceName, ipAddress, traceId);

            context.Response.OnCompleted(() =>
            {
                stopwatch.Stop();

                var statusCode = context.Response.StatusCode;
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                // LOG 2: Request kết thúc
                if (statusCode >= 500)
                {
                    _logger.LogError("[ERROR] {Method} {Path} | Status: {StatusCode} | Time: {Elapsed}ms | User: {UserId} | TraceId: {TraceId}",
                        request.Method, request.Path, statusCode, elapsedMs, userId, traceId);  // ĐÃ THÊM THAM SỐ
                }
                else if (elapsedMs > 500)
                {
                    _logger.LogWarning("[SLOW] {Method} {Path} | Status: {StatusCode} | Time: {Elapsed}ms | User: {UserId} | TraceId: {TraceId}",
                        request.Method, request.Path, statusCode, elapsedMs, userId, traceId);
                }
                else
                {
                    _logger.LogInformation("[RESPONSE] {Method} {Path} | Status: {StatusCode} | Time: {Elapsed}ms | User: {UserId} | TraceId: {TraceId}",
                        request.Method, request.Path, statusCode, elapsedMs, userId, traceId);
                }

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}