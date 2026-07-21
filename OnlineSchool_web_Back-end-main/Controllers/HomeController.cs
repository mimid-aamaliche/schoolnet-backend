using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineSchool_Back_end.Services;
using OnlineSchoolBusniss;
using SharedDTOs;


namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/Home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly HomePageService _homePageService;



        public HomeController(IAuthService AuthService, HomePageService homePageService)
        {

            _authService = AuthService;
            _homePageService = homePageService;
        }




        [HttpGet("", Name = "GetHomePage")]
        public async Task<ActionResult> GetHomePage()
        {
            if (!_authService.UserIsAuthenticated())
            {
                return Unauthorized("User is not logged in this moment");
            }

            var UserMode = _authService.getRole();
            if (UserMode == null)
            {
                return Unauthorized("User Role Claim Not Found");
            }
            int mode = UserMode == "student" ? 1 : 2;
            var UserId = _authService.GetUserID();


            if (UserId == null)
            {
                return Unauthorized("User Id Claim Not Found");
            }

            if (!int.TryParse(UserId, out int id))
            {
                return Unauthorized("Current User Info is NOt Valid format");
            }


            Console.WriteLine("1");

            if (mode == (int)enMode.enStudentMode)
            {
                //student 
                return Ok(new
                {
                    data = await _homePageService.StudentHomePageData(id),
                    role = (int)enMode.enStudentMode
                });
            }
            else
            {
                //teacher
                return Ok(new
                {
                    data = await _homePageService.TeacherHomePageData(id),
                    role = (int)enMode.enTeacherMode
                });
            }


        }


























    }
}
