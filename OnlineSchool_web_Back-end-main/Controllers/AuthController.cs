using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineSchoolBusniss;
using SharedDTOs;
using OnlineSchool_Back_end.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {



        private readonly clsStripeService _stripeService;

        private readonly IAuthService _authService;


        public AuthController(clsStripeService stripeService, IAuthService AuthService)
        {
            _stripeService = stripeService;
            _authService = AuthService;
        }



















        [HttpPost("login", Name = "login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]


        public async Task<ActionResult> login([FromBody][Required] AuthRequest request)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //we gonna set these tokens to last for about 1 day 

            if (request.Mode == enMode.enStudentMode)
            {
                //user is a student

                var student = _authService.AuthenticateStudent(request);
                if (student == null)
                {

                    //student does not exist
                    //return anauthorized status code 
                    return Unauthorized("Not Valide credentiels");

                }



                //sign in async as student

                await _authService.SignInUser(new List<Claim>
                    {
                        new Claim("UserID",student.StudentID.ToString()),
                        new Claim("User_Role","student")

                    });

                //return the AuthResponse object
                return Ok(new AuthResponse { user = student.userDTO, ExpireAt = DateTime.Now.AddDays(1) });




            }
            else
            {

                var teacher = _authService.AuthenticateTeacher(request);
                if (teacher == null)
                {

                    //student does not exist
                    //return anauthorized status code 
                    return Unauthorized();

                }



                await _authService.SignInUser(new List<Claim>
                    {
                        new Claim("UserID",teacher.TeacherID.ToString()),
                        new Claim("User_Role","teacher")

                    });

                return Ok(new AuthResponse { user = teacher.userDTO, ExpireAt = DateTime.Now.AddDays(1) });



            }




        }









        [HttpPost("StudentSignUp", Name = "StudentSignUp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<UserDTO> StudentSignUp([FromBody][Required] AddStudentDTO request)
        {

            //Input Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }




            //fill clsStudent object

            clsStudent student = new clsStudent(request);
            if (student.PersonalInfo.Save())
            {
                if (student.Save())
                {

                    //here create the url where the user gonna post the image
                    //and send it with the user dto


                    return Ok(student.userDTO);

                }
                else
                {
                    return BadRequest("somthing wrong in the student info or in db connexion");
                }
            }
            else
            {
                return BadRequest("somthing wrong in the person info or in db connexion");
            }





        }

























        [HttpPost("TeacherSignUp", Name = "TeacherSignUp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<UserDTO>> TeacherSignUp([FromBody][Required] AddTeacherDTO request)
        {


            //Input validation 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }




            var UserStripeID = _stripeService.CreateConnectedAccount();

            //fill clsStudent object
            clsTeacher teacher = new clsTeacher(request);


            //wait for the user stripe id 
            teacher.TeacherStripeID = await UserStripeID;



            if (teacher.PersonalInfo.Save())
            {
                if (teacher.Save())
                {
                    //1)
                    //here create the url where the user gonna post the image
                    //and send it with the user dto


                    //here we gonna return the image upload url and the stripe creat connected account url + userDTO
                    return Ok(teacher.userDTO);

                }
                else
                {
                    return BadRequest("somthing wrong in the teacher info or in db connexion");
                }
            }
            else
            {
                return BadRequest("somthing wrong in the person info or in db connexion");
            }





        }



    }
}
