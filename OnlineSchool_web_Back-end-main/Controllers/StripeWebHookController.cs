using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineSchoolBusniss;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Events;
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

        public StripeWebHookController(clsStripeService stripeService, IConfiguration configuration)
        {
            _stripeService = stripeService;
            _webhookKey = configuration["Stripe:WebHookKey"]!;

        }







        [HttpPost]
        public async Task<IActionResult> stripeWebHook()
        {


            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();


            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], "whsec_rtiEuejmiyn4Y6HwWWTZsiqSV5YduWmu");

                if (stripeEvent.Type == "checkout.session.completed")
                {

                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    bool Result = await _stripeService.HandleCheckOutSessionCompleted(stripeEvent.Data.Object as Stripe.Checkout.Session);


                    if (!Result)
                    {
                        Console.Error.WriteLine($"Failed to handle session {session.Id}");
                        return StatusCode(500);
                    }

                    return Ok();
                }

                //.........






            }
            catch (Exception ex)
            {


                return StatusCode(500);

            }

            return Ok();
        }







    }
}
