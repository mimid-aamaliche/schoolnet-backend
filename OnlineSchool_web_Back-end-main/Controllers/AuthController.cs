using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineSchoolBusniss;
using SharedDTOs;
using OnlineSchool_Back_end.Services;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(clsStripeService stripeService,
                              IAuthService AuthService,
                              ILogger<AuthController> logger)
        {
            _stripeService = stripeService;
            _authService   = AuthService;
            _logger        = logger;
        }


        // ─── POST api/Auth/login ───────────────────────────────────────────────

        [HttpPost("login", Name = "login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> login([FromBody][Required] AuthRequest request)
        {
            _logger.LogInformation("Login attempt | Mode={Mode}", request.Mode);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login failed — invalid model state");
                return BadRequest(ModelState);
            }

            if (request.Mode == enMode.enStudentMode)
            {
                var student = _authService.AuthenticateStudent(request);
                if (student == null)
                {
                    _logger.LogWarning("Student login failed — invalid credentials");
                    return Unauthorized("Not Valide credentiels");
                }

                await _authService.SignInUser(new List<Claim>
                {
                    new Claim("UserID",    student.StudentID.ToString()),
                    new Claim("User_Role", "student")
                });

                _logger.LogInformation("Student logged in | StudentID={StudentID}", student.StudentID);
                return Ok(new AuthResponse { user = student.userDTO, ExpireAt = DateTime.Now.AddDays(1) });
            }
            else
            {
                var teacher = _authService.AuthenticateTeacher(request);
                if (teacher == null)
                {
                    _logger.LogWarning("Teacher login failed — invalid credentials");
                    return Unauthorized();
                }

                await _authService.SignInUser(new List<Claim>
                {
                    new Claim("UserID",    teacher.TeacherID.ToString()),
                    new Claim("User_Role", "teacher")
                });

                _logger.LogInformation("Teacher logged in | TeacherID={TeacherID}", teacher.TeacherID);
                return Ok(new AuthResponse { user = teacher.userDTO, ExpireAt = DateTime.Now.AddDays(1) });
            }
        }


        // ─── POST api/Auth/StudentSignUp ──────────────────────────────────────

        [HttpPost("StudentSignUp", Name = "StudentSignUp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserDTO> StudentSignUp([FromBody][Required] AddStudentDTO request)
        {
            _logger.LogInformation("StudentSignUp attempt");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("StudentSignUp failed — invalid model state");
                return BadRequest(ModelState);
            }

            clsStudent student = new clsStudent(request);

            if (student.PersonalInfo.Save())
            {
                if (student.Save())
                {
                    _logger.LogInformation("StudentSignUp succeeded | StudentID={StudentID}", student.StudentID);
                    return Ok(student.userDTO);
                }

                _logger.LogError("StudentSignUp failed — could not save student record | PersonID={PersonID}", student.PersonalInfo.PersonID);
                return BadRequest("somthing wrong in the student info or in db connexion");
            }

            _logger.LogError("StudentSignUp failed — could not save person record");
            return BadRequest("somthing wrong in the person info or in db connexion");
        }


        // ─── POST api/Auth/TeacherSignUp ──────────────────────────────────────

        [HttpPost("TeacherSignUp", Name = "TeacherSignUp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> TeacherSignUp([FromBody][Required] AddTeacherDTO request)
        {
            _logger.LogInformation("TeacherSignUp attempt");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("TeacherSignUp failed — invalid model state");
                return BadRequest(ModelState);
            }

            var UserStripeID = _stripeService.CreateConnectedAccount();

            clsTeacher teacher = new clsTeacher(request);
            teacher.TeacherStripeID = await UserStripeID;
            _logger.LogInformation("Stripe connected account created | StripeID={StripeID}", teacher.TeacherStripeID);

            if (teacher.PersonalInfo.Save())
            {
                if (teacher.Save())
                {
                    _logger.LogInformation("TeacherSignUp succeeded | TeacherID={TeacherID}", teacher.TeacherID);
                    return Ok(teacher.userDTO);
                }

                _logger.LogError("TeacherSignUp failed — could not save teacher record | PersonID={PersonID}", teacher.PersonalInfo.PersonID);
                return BadRequest("somthing wrong in the teacher info or in db connexion");
            }

            _logger.LogError("TeacherSignUp failed — could not save person record");
            return BadRequest("somthing wrong in the person info or in db connexion");
        }
    }
}
