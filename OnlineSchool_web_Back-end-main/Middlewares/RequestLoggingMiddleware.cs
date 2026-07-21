using System.Diagnostics;

namespace OnlineSchool_Back_end.Middlewares
{
    /// <summary>
    /// Logs every HTTP request and its response automatically.
    /// Placed early in the pipeline so it wraps all other middleware.
    ///
    /// Log format:
    ///   → [METHOD] /path/route  (request)
    ///   ← [STATUS] /path/route  123ms  (response)
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next,
                                        ILogger<RequestLoggingMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw     = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path   = context.Request.Path + context.Request.QueryString;

            _logger.LogInformation("→ {Method} {Path}", method, path);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex,
                    "✕ Unhandled exception on {Method} {Path} after {ElapsedMs}ms",
                    method, path, sw.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                sw.Stop();
                var status = context.Response.StatusCode;
                var level  = status >= 500 ? LogLevel.Error
                           : status >= 400 ? LogLevel.Warning
                           : LogLevel.Information;

                _logger.Log(level,
                    "← {Status} {Method} {Path}  ({ElapsedMs}ms)",
                    status, method, path, sw.ElapsedMilliseconds);
            }
        }
    }
}
