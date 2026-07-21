using Microsoft.AspNetCore.Mvc;
using OnlineSchoolBusniss;
using SharedDTOs;
using OnlineSchool_Back_end.Services;

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/Home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly HomePageService _homePageService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IAuthService AuthService,
                              HomePageService homePageService,
                              ILogger<HomeController> logger)
        {
            _authService     = AuthService;
            _homePageService = homePageService;
            _logger          = logger;
        }


        // ─── GET api/Home ─────────────────────────────────────────────────────

        [HttpGet("", Name = "GetHomePage")]
        public async Task<ActionResult> GetHomePage()
        {
            if (!_authService.UserIsAuthenticated())
            {
                _logger.LogWarning("GetHomePage — unauthenticated request");
                return Unauthorized("User is not logged in this moment");
            }

            var UserMode = _authService.getRole();
            if (UserMode == null)
            {
                _logger.LogWarning("GetHomePage — missing role claim");
                return Unauthorized("User Role Claim Not Found");
            }

            var UserId = _authService.GetUserID();
            if (UserId == null)
            {
                _logger.LogWarning("GetHomePage — missing user ID claim");
                return Unauthorized("User Id Claim Not Found");
            }

            if (!int.TryParse(UserId, out int id))
            {
                _logger.LogWarning("GetHomePage — user ID claim is not a valid integer: {UserId}", UserId);
                return Unauthorized("Current User Info is NOt Valid format");
            }

            int mode = UserMode == "student" ? 1 : 2;
            _logger.LogInformation("GetHomePage | UserID={UserID} Role={Role}", id, UserMode);

            if (mode == (int)enMode.enStudentMode)
            {
                var data = await _homePageService.StudentHomePageData(id);
                _logger.LogInformation("Student home data fetched | UserID={UserID}", id);
                return Ok(new { data, role = (int)enMode.enStudentMode });
            }
            else
            {
                var data = await _homePageService.TeacherHomePageData(id);
                _logger.LogInformation("Teacher home data fetched | UserID={UserID}", id);
                return Ok(new { data, role = (int)enMode.enTeacherMode });
            }
        }
    }
}
