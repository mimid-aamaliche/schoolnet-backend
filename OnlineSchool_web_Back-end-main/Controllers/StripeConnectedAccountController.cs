using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineSchoolBusniss;
using OnlineSchool_Back_end.Services;

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/StripeConnectedAccount")]
    [ApiController]
    public class StripeConnectedAccountController : ControllerBase
    {
        private readonly clsStripeService _stripeService;
        private readonly IAuthService _authService;
        private readonly ILogger<StripeConnectedAccountController> _logger;

        public StripeConnectedAccountController(clsStripeService stripeService,
                                                IAuthService AuthService,
                                                ILogger<StripeConnectedAccountController> logger)
        {
            _stripeService = stripeService;
            _authService   = AuthService;
            _logger        = logger;
        }


        // ─── POST api/StripeConnectedAccount/create-account-link ─────────────

        [HttpPost("create-account-link")]
        public async Task<IActionResult> CreateAccountLink()
        {
            if (!_authService.UserIsAuthenticated())
            {
                _logger.LogWarning("CreateAccountLink — unauthenticated request");
                return Unauthorized("User is not logged in this moment");
            }

            var UserMode = _authService.getRole();
            if (UserMode == null)
            {
                _logger.LogWarning("CreateAccountLink — missing role claim");
                return Unauthorized("User Role Claim Not Found");
            }
            if (UserMode == "student")
            {
                _logger.LogWarning("CreateAccountLink — student tried to create a connected account (forbidden)");
                return Unauthorized("Students can t have a stripe connected account");
            }

            var UserId = _authService.GetUserID();
            if (UserId == null || !int.TryParse(UserId, out int id))
            {
                _logger.LogWarning("CreateAccountLink — missing or invalid user ID claim");
                return Unauthorized("User id  Format not valid");
            }

            _logger.LogInformation("CreateAccountLink | TeacherID={TeacherID}", id);

            try
            {
                clsTeacher user = clsTeacher.FindTeacher(id);
                if (user == null)
                {
                    _logger.LogWarning("CreateAccountLink — teacher not found in DB | TeacherID={TeacherID}", id);
                    return BadRequest("User Does not exist in db");
                }

                if (string.IsNullOrEmpty(user.TeacherStripeID))
                {
                    _logger.LogInformation("CreateAccountLink — no Stripe ID on record, creating new connected account | TeacherID={TeacherID}", id);
                    string TeacherStripeId = await _stripeService.CreateConnectedAccount();
                    user.TeacherStripeID   = TeacherStripeId;

                    if (!user.Save())
                    {
                        _logger.LogError("CreateAccountLink — failed to save new StripeID to DB | TeacherID={TeacherID}", id);
                        return StatusCode(500, "Database connection problems");
                    }

                    _logger.LogInformation("CreateAccountLink — new Stripe connected account saved | TeacherID={TeacherID} StripeID={StripeID}",
                        id, TeacherStripeId);
                }

                var accountLink = await _stripeService.CreateAccountLink(user.TeacherStripeID);
                _logger.LogInformation("CreateAccountLink — account link generated | TeacherID={TeacherID}", id);
                return Ok(new { Url = accountLink });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAccountLink — unexpected error | TeacherID={TeacherID}", id);
                return StatusCode(500, "Database connection problems");
            }
        }
    }
}
