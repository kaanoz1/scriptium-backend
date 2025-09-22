using ScriptiumBackend.Models;
using ScriptiumBackend.DB;

namespace ScriptiumBackend.MiddleWare
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var db = context.RequestServices.GetRequiredService<ApplicationDbContext>();

            var identifier = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var endpoint = context.Request.Path.ToString();
            var method = context.Request.Method;

            await _next(context);

            context.Response.OnCompleted(async () =>
            {
                int statusCode = context.Response.StatusCode;

                var requestLog = new RequestLog
                {
                    Identifier = identifier,
                    Endpoint = endpoint,
                    Method = method,
                    StatusCode = statusCode,
                    OccurredAt = DateTime.UtcNow
                };

                db.RequestLogs.Add(requestLog);

                try
                {
                    await db.SaveChangesAsync();
                    _logger.LogInformation($"Request logged successfully: {requestLog}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occurred while saving the request log. Error Details: {ex}");
                }
            });
        }
    }
}