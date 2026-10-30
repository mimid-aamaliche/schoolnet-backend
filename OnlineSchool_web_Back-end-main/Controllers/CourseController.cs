using Microsoft.AspNetCore.Mvc;
using OnlineSchoolBusniss;
using SharedDTOs;
using OnlineSchool_Back_end.Services;

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/Course")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(IAuthService AuthService,
                                ILogger<CourseController> logger)
        {
            _authService = AuthService;
            _logger      = logger;
        }


        // ─── GET api/Course/All ───────────────────────────────────────────────

        [HttpGet("All", Name = "GetAllCourses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<IEnumerable<CourseDTO>> GetAllCourses()
        {
            _logger.LogInformation("GetAllCourses called");

            List<CourseDTO> CourseList = clsCourse.GetCoursesList();

            if (CourseList.Count == 0)
            {
                _logger.LogInformation("GetAllCourses — no courses found (204)");
                return NoContent();
            }

            _logger.LogInformation("GetAllCourses — returned {Count} courses", CourseList.Count);
            return Ok(CourseList);
        }


        // ─── GET api/Course/{id} ──────────────────────────────────────────────

        [HttpGet("{id}", Name = "GetCourseByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CourseDTO_Detailed> GetCourseByID(int id)
        {
            _logger.LogInformation("GetCourseByID | CourseID={CourseID}", id);

            if (id < 0)
            {
                _logger.LogWarning("GetCourseByID — invalid ID {CourseID}", id);
                return BadRequest("course id must be positive");
            }

            clsCourse Course = clsCourse.Find(id);

            if (Course == null)
            {
                _logger.LogWarning("GetCourseByID — course not found | CourseID={CourseID}", id);
                return NoContent();
            }

            _logger.LogInformation("GetCourseByID — found course | CourseID={CourseID}", id);
            return Ok(Course.CDTO_Detailed);
        }


        // ─── GET api/Course/My ────────────────────────────────────────────────

        [HttpGet("My", Name = "GetCoursesOfUserByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetCoursesOfUserByID()
        {
            if (!_authService.UserIsAuthenticated())
            {
                _logger.LogWarning("GetCoursesOfUserByID — unauthenticated request");
                return Unauthorized("User is not logged in this moment");
            }

            var UserMode = _authService.getRole();
            if (UserMode == null)
            {
                _logger.LogWarning("GetCoursesOfUserByID — missing role claim");
                return Unauthorized("User Role Claim Not Found");
            }

            var UserId = _authService.GetUserID();
            if (UserId == null)
            {
                _logger.LogWarning("GetCoursesOfUserByID — missing user ID claim");
                return Unauthorized("User Id Claim Not Found");
            }

            if (!int.TryParse(UserId, out int id))
            {
                _logger.LogWarning("GetCoursesOfUserByID — invalid user ID format: {UserId}", UserId);
                return Unauthorized("User id  Format not valid");
            }

            if (id < 0)
            {
                _logger.LogWarning("GetCoursesOfUserByID — negative user ID | UserID={UserID}", id);
                return BadRequest("user id must be positive");
            }

            int mode = UserMode == "student" ? 1 : 2;
            _logger.LogInformation("GetCoursesOfUserByID | UserID={UserID} Role={Role}", id, UserMode);

            if ((enMode)mode == enMode.enTeacherMode)
            {
                List<CourseDTO> courseList = clsCourseHandlers.GetCourseOfTeacherX(id);
                if (courseList == null)
                {
                    _logger.LogInformation("GetCoursesOfUserByID — no courses found for teacher | TeacherID={TeacherID}", id);
                    return NoContent();
                }

                _logger.LogInformation("GetCoursesOfUserByID — returned {Count} courses for teacher | TeacherID={TeacherID}", courseList.Count, id);
                return Ok(courseList);
            }
            else if ((enMode)mode == enMode.enStudentMode)
            {
                List<CourseDTO> courseList = await clsSubscribtion.GetStudentCoursesAsync(id);
                if (courseList == null)
                {
                    _logger.LogInformation("GetCoursesOfUserByID — no subscribed courses for student | StudentID={StudentID}", id);
                    return NoContent();
                }

                _logger.LogInformation("GetCoursesOfUserByID — returned {Count} courses for student | StudentID={StudentID}", courseList.Count, id);
                return Ok(courseList);
            }

            _logger.LogError("GetCoursesOfUserByID — invalid mode value {Mode}", mode);
            return BadRequest("Invalid Mode Value");
        }


        // ─── POST api/Course/Add ──────────────────────────────────────────────

        [HttpPost("Add", Name = "AddNewCourse")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CourseDTO> AddNewCourse(AddCourseDTO request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("AddNewCourse — invalid model state");
                return BadRequest(ModelState);
            }

            if (!_authService.UserIsAuthenticated())
            {
                _logger.LogWarning("AddNewCourse — unauthenticated request");
                return Unauthorized("User is not logged in this moment");
            }

            var UserMode = _authService.getRole();
            if (UserMode == null)
            {
                _logger.LogWarning("AddNewCourse — missing role claim");
                return Unauthorized("User Role Claim Not Found");
            }

            int mode = UserMode == "student" ? 1 : 2;
            if (mode == 1)
            {
                _logger.LogWarning("AddNewCourse — student tried to add a course (forbidden)");
                return Unauthorized("Students can t add courses");
            }

            var UserId = _authService.GetUserID();
            if (UserId == null || !int.TryParse(UserId, out int id))
            {
                _logger.LogWarning("AddNewCourse — missing or invalid user ID claim");
                return Unauthorized("User id  Format not valid");
            }

            _logger.LogInformation("AddNewCourse | TeacherID={TeacherID} Title={Title}", id, request.CourseTitle);

            clsCourse new_Course = new clsCourse(request);
            if (new_Course.Save())
            {
                clsCourseHandlers CH = new clsCourseHandlers
                {
                    CourseID   = new_Course.CourseID,
                    Permission = "admin",
                    TeacherID  = id
                };

                if (!CH.Save())
                {
                    _logger.LogError("AddNewCourse — course saved but CourseHandler record failed | CourseID={CourseID} TeacherID={TeacherID}",
                        new_Course.CourseID, id);
                }

                _logger.LogInformation("AddNewCourse succeeded | CourseID={CourseID} TeacherID={TeacherID}", new_Course.CourseID, id);
                return CreatedAtRoute("GetCourseByID", new { id = new_Course.CourseID }, new_Course.CDTO);
            }

            _logger.LogError("AddNewCourse failed — db save error | TeacherID={TeacherID}", id);
            return BadRequest("somthing went wrong in the db connection part");
        }
    }
}
