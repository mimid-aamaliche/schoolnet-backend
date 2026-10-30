using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineSchool_Back_end.Services;
using OnlineSchoolBusniss;
using SharedDTOs;

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/Subscribtion")]
    [ApiController]
    public class SubscribtionController : ControllerBase
    {
        private readonly clsStripeService _stripeService;
        private readonly IAuthService _authService;
        private readonly ILogger<SubscribtionController> _logger;

        public SubscribtionController(clsStripeService stripeService,
                                      IAuthService AuthService,
                                      ILogger<SubscribtionController> logger)
        {
            _stripeService = stripeService;
            _authService   = AuthService;
            _logger        = logger;
        }


        // ─── POST api/Subscribtion/UserHasAccess ──────────────────────────────

        [HttpPost("UserHasAccess", Name = "UserHasAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SubscribtionAuthResponceDTO>> UserHasAccess(
            [FromBody][Required][Range(0, int.MaxValue)] int CourseID)
        {
            if (!_authService.UserIsAuthenticated())
            {
                _logger.LogWarning("UserHasAccess — unauthenticated request | CourseID={CourseID}", CourseID);
                return Unauthorized("User is not logged in this moment");
            }

            var UserMode = _authService.getRole();
            if (UserMode == null)
            {
                _logger.LogWarning("UserHasAccess — missing role claim");
                return Unauthorized("User Role Claim Not Found");
            }

            var UserId = _authService.GetUserID();
            if (UserId == null || !int.TryParse(UserId, out int id))
            {
                _logger.LogWarning("UserHasAccess — missing or invalid user ID claim");
                return Unauthorized("User id  Format not valid");
            }

            int mode = UserMode == "student" ? 1 : 2;
            _logger.LogInformation("UserHasAccess check | UserID={UserID} Role={Role} CourseID={CourseID}", id, UserMode, CourseID);

            var responce = new SubscribtionAuthResponceDTO();

            if (mode == (int)enMode.enStudentMode)
            {
                responce.HasAccess = await clsSubscribtion.StudentHasAccessAsync(id, CourseID);
                responce.message   = "";
                _logger.LogInformation("UserHasAccess result | StudentID={StudentID} CourseID={CourseID} HasAccess={HasAccess}",
                    id, CourseID, responce.HasAccess);
            }
            else
            {
                responce.HasAccess = false;
                responce.message   = "Teacher permissions on course not implimented fully yet";
                _logger.LogWarning("UserHasAccess — teacher access check not yet implemented | TeacherID={TeacherID} CourseID={CourseID}",
                    id, CourseID);
            }

            return Ok(responce);
        }


        // ─── POST api/Subscribtion/Pay ────────────────────────────────────────

        [HttpPost("Pay", Name = "Pay")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentResponce>> Pay(
            [FromBody][Required][Range(0, int.MaxValue)] int CourseID)
        {
            if (!_authService.UserIsAuthenticated())
            {
                _logger.LogWarning("Pay — unauthenticated request | CourseID={CourseID}", CourseID);
                return Unauthorized("User is not logged in this moment");
            }

            var UserMode = _authService.getRole();
            if (UserMode == null)
            {
                _logger.LogWarning("Pay — missing role claim");
                return Unauthorized("User Role Claim Not Found");
            }

            var UserId = _authService.GetUserID();
            if (UserId == null || !int.TryParse(UserId, out int id))
            {
                _logger.LogWarning("Pay — missing or invalid user ID claim");
                return Unauthorized("User id  Format not valid");
            }

            _logger.LogInformation("Pay initiated | StudentID={StudentID} CourseID={CourseID}", id, CourseID);

            clsCourse  course  = clsCourse.Find(CourseID);
            clsStudent student = clsStudent.FindStudent(id);

            if (student == null || course == null)
            {
                _logger.LogWarning("Pay — student or course not found | StudentID={StudentID} CourseID={CourseID} StudentFound={SF} CourseFound={CF}",
                    id, CourseID, student != null, course != null);
                return BadRequest();
            }

            var url = await _stripeService.Pay(course, student);

            _logger.LogInformation("Pay — Stripe checkout session created | StudentID={StudentID} CourseID={CourseID}", id, CourseID);
            return Ok(new { checkout_url = url });
        }
    }
}
