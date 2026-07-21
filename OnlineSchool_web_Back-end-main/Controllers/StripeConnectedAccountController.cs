using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineSchoolBusniss;
using OnlineSchool_Back_end.Services;
using SharedDTOs;

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/StripeConnectedAccount")]
    [ApiController]
    public class StripeConnectedAccountController : ControllerBase
    {


        private readonly clsStripeService _stripeService;
        private readonly IAuthService _authService;


        public StripeConnectedAccountController(clsStripeService stripeService,IAuthService AuthService)
        {
            _stripeService = stripeService;
            _authService = AuthService;

        }










        [HttpPost("create-account-link")]
        public async Task<IActionResult> CreateAccountLink()
        {
           

            //Authorasation part ________________________________________________________________
            if (!_authService.UserIsAuthenticated())
            {
                return Unauthorized("User is not logged in this moment");
            }


            var UserMode=_authService.getRole();
            if (UserMode == null)
            {
                return Unauthorized("User Role Claim Not Found");
            }
            if(UserMode == "student")
                return Unauthorized("Students can t have a stripe connected account");



            var UserId = _authService.GetUserID();
            if (UserId == null)
            {
                return Unauthorized("User Id Claim Not Found");
            }
            if (!int.TryParse(UserId, out int id))
            {
                return Unauthorized("User id  Format not valid");
            }
            //____________________________________________________________________________________



            //get the user stripe id
            try
            {


                clsTeacher user = clsTeacher.FindTeacher(id);
                if (user == null)
                {
                    return BadRequest("User Does not exist in db");
                }

                if (string.IsNullOrEmpty(user.TeacherStripeID))
                {
                    //user didn t create the connected account( if somthing went wrong during the sign in)
                    //here we create the connected account
                    string TeacherStripeId = await _stripeService.CreateConnectedAccount();
                    user.TeacherStripeID = TeacherStripeId;
                    if (!user.Save())
                    {
                        //somthing went wrong in the db part
                        //try again later
                        return StatusCode(500, "Database connection problems");

                    }

                }


                return Ok(new { Url = await _stripeService.CreateAccountLink(user.TeacherStripeID) });



            }
            catch (Exception ex)
            {

                return StatusCode(500, "Database connection problems");

            }



        }

        













    }
}
