using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineSchoolBusniss;

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/[controller]")]
    [DisableCors]
    [AllowAnonymous]
    [ApiController]
    public class MuxWebhookController : ControllerBase
    {
        private readonly MUXEventService _muxEventService;
        private readonly ILogger<MuxWebhookController> _logger;

        public MuxWebhookController(MUXEventService MuxEventService,
                                    ILogger<MuxWebhookController> logger)
        {
            _muxEventService = MuxEventService;
            _logger          = logger;
        }


        // ─── POST api/MuxWebhook ──────────────────────────────────────────────
        // Supports /api/MuxWebhook, /api/MuxWebhook/mux/webhook, /api/MuxWebhook/webhook

        [HttpPost]
        [HttpPost("mux/webhook")]
        [HttpPost("webhook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> muxWebHook()
        {
            var body = HttpContext.Items["RawBody"] as string;

            if (string.IsNullOrEmpty(body))
            {
                HttpContext.Request.EnableBuffering();
                using var reader = new StreamReader(HttpContext.Request.Body, System.Text.Encoding.UTF8, leaveOpen: true);
                body = await reader.ReadToEndAsync();
                HttpContext.Request.Body.Position = 0;
            }

            if (string.IsNullOrEmpty(body))
            {
                _logger.LogWarning("MuxWebhook — empty or missing request body");
                return BadRequest();
            }

            _logger.LogInformation("MuxWebhook received | BodyLength={BodyLength}", body.Length);

            bool handled = await _muxEventService.HandleMuxEvents(body);

            if (!handled)
            {
                _logger.LogError("MuxWebhook — event handler returned false (processing failed)");
                return StatusCode(500);
            }

            _logger.LogInformation("MuxWebhook — event handled successfully");
            return Ok();
        }
    }
}
