using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineSchoolBusniss;
using Stripe;
using Microsoft.AspNetCore.Authorization;

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/StripeWebHook")]
    [AllowAnonymous]
    [ApiController]
    public class StripeWebHookController : ControllerBase
    {
        private readonly clsStripeService _stripeService;
        private readonly string _webhookKey;
        private readonly ILogger<StripeWebHookController> _logger;

        public StripeWebHookController(clsStripeService stripeService,
                                       IConfiguration configuration,
                                       ILogger<StripeWebHookController> logger)
        {
            _stripeService = stripeService;
            _webhookKey    = configuration["Stripe:WebHookKey"]!;
            _logger        = logger;
        }


        // ─── POST api/StripeWebHook ───────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> stripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    "whsec_rtiEuejmiyn4Y6HwWWTZsiqSV5YduWmu"
                );

                _logger.LogInformation("Stripe webhook received | EventType={EventType} EventID={EventID}",
                    stripeEvent.Type, stripeEvent.Id);

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    _logger.LogInformation("Handling checkout.session.completed | SessionID={SessionID}", session?.Id);

                    bool result = await _stripeService.HandleCheckOutSessionCompleted(session);

                    if (!result)
                    {
                        _logger.LogError("checkout.session.completed handler failed | SessionID={SessionID}", session?.Id);
                        return StatusCode(500);
                    }

                    _logger.LogInformation("checkout.session.completed handled successfully | SessionID={SessionID}", session?.Id);
                    return Ok();
                }

                _logger.LogInformation("Stripe webhook event ignored (no handler) | EventType={EventType}", stripeEvent.Type);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook signature validation failed");
                return StatusCode(400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stripe webhook unhandled exception");
                return StatusCode(500);
            }

            return Ok();
        }
    }
}
