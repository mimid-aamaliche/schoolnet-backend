using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OnlineSchoolBusniss;

namespace OnlineSchool_Back_end.Middlewares
{
    public class muxTraficValidatorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string? _webhookKey;
        private readonly ILogger<muxTraficValidatorMiddleware>? _logger;

        public muxTraficValidatorMiddleware(RequestDelegate next,
                                           IConfiguration configuration,
                                           ILogger<muxTraficValidatorMiddleware>? logger = null)
        {
            _next       = next;
            _webhookKey = configuration["MUXVideoService:WebHookKey"];
            _logger     = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request is headed for the MUX webhook endpoint
            if (context.Request.Path.Value == null ||
               !context.Request.Path.Value.StartsWith("/api/MuxWebhook", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            _logger?.LogInformation("MUX Webhook middleware intercepted request | Path={Path}", context.Request.Path);

            // Enable buffering so the request body can be read safely
            context.Request.EnableBuffering();

            string body;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0; // reset position for downstream consumers
            }

            context.Items["RawBody"] = body;

            // Optional development bypass: if WebHookKey is missing or set to "disabled" / "bypass"
            if (string.IsNullOrEmpty(_webhookKey) || _webhookKey.Equals("disabled", StringComparison.OrdinalIgnoreCase) || _webhookKey.Equals("bypass", StringComparison.OrdinalIgnoreCase))
            {
                _logger?.LogWarning("MUX Webhook signature validation skipped (WebHookKey is empty or set to bypass/disabled)");
                await _next(context);
                return;
            }

            if (context.Request.Headers.TryGetValue("Mux-Signature", out var signatureHeader))
            {
                var muxSignatureElements = signatureHeader.ToString().Split(',');

                if (muxSignatureElements.Length >= 2)
                {
                    var timestamp = muxSignatureElements[0].Split('=')[1];
                    var signature = muxSignatureElements[1].Split('=')[1];

                    string payload = $"{timestamp}.{body}";
                    string computedSignature = MyEncriptionFunctions.hmachashing(payload, _webhookKey, new HMACSHA256());

                    if (string.Equals(signature, computedSignature, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger?.LogInformation("MUX Webhook signature verified successfully");
                        await _next(context);
                        return;
                    }
                    else
                    {
                        _logger?.LogError("MUX Webhook signature mismatch | Received={Received} Computed={Computed}",
                            signature, computedSignature);
                    }
                }
                else
                {
                    _logger?.LogError("MUX Webhook Mux-Signature header has invalid format: {Header}", signatureHeader.ToString());
                }
            }
            else
            {
                _logger?.LogError("MUX Webhook request missing Mux-Signature header");
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: MUX signature validation failed");
        }
    }
}
