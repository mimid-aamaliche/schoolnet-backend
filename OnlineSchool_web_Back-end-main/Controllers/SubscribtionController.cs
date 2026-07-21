using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public SubscribtionController(clsStripeService stripeService, IAuthService AuthService)
        {
            _stripeService = stripeService;
            _authService = AuthService;

        }












        //User has access to course X endpoint
        [HttpPost("UserHasAccess", Name = "UserHasAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SubscribtionAuthResponceDTO>> UserHasAccess([FromBody][Required][Range(0, int.MaxValue)] int CourseID)
        {

            //Authorasation part ________________________________________________________________
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
                return Unauthorized("User id  Format not valid");

            }
            //____________________________________________________________________________________


            var responce = new SubscribtionAuthResponceDTO();

            if (mode == (int)enMode.enStudentMode)
            {
                //chack if the student has access to this course => he payed the fee subscribtion

                responce.HasAccess = await clsSubscribtion.StudentHasAccessAsync(id, CourseID);

                responce.message = "";
            }
            else
            {
                //check if the teacher has access to this course => he is a handler with some permissions on this course

                responce.HasAccess = false;
                responce.message = "Teacher permissions on course not implimented fully yet";
            }

            return Ok(responce);
        }








        [HttpPost("Pay", Name = "Pay")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentResponce>> Pay([FromBody][Required][Range(0, int.MaxValue)] int CourseID)
        {


            //Authorasation part ________________________________________________________________
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
                return Unauthorized("User id  Format not valid");
            }
            //____________________________________________________________________________________

            Console.WriteLine($"course id {CourseID} , student id {id}");




            //get product
            clsCourse course = clsCourse.Find(CourseID);
            //get customer
            clsStudent student = clsStudent.FindStudent(id);



            if (student == null || course == null)
            {
                return BadRequest();
            }


            //logic

            var url = await _stripeService.Pay(course, student);


            //return checkout session url

            return Ok(new { checkout_url = url });

        }













    }
}
