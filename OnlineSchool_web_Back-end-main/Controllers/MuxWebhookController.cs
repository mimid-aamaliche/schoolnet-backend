using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineSchoolBusniss;
using Stripe;

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/[controller]")]
    [DisableCors]
    [AllowAnonymous]
    [ApiController]
    public class MuxWebhookController : ControllerBase
    {


        private readonly MUXEventService _muxEventService;

        public MuxWebhookController(MUXEventService MuxEventService)
        {
            _muxEventService = MuxEventService;
        }




        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> muxWebHook()
        {

            var body = HttpContext.Items["RawBody"] as string;

            if (string.IsNullOrEmpty(body)) return BadRequest();

            //Handle the events
            if (!await _muxEventService.HandleMuxEvents(body))
            {
                return StatusCode(500);
            }
            else
            {
                return Ok();
            }
        }






    }
}
