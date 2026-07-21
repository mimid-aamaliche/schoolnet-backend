using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OnlineSchoolBusniss;
using SharedDTOs;
using OnlineSchool_Back_end.Services;
using System.Data;
using System.Security.Cryptography;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/Course")]
    [ApiController]
    public class CourseController : ControllerBase
    {

        private readonly IAuthService _authService;


        public CourseController(IAuthService AuthService)
        {
            _authService = AuthService;
        }




        [HttpGet("All", Name = "GetAllCourses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public ActionResult<IEnumerable<CourseDTO>> GetAllCourses()
        {

            List<CourseDTO> CourseList = clsCourse.GetCoursesList();

            if (CourseList.Count == 0)
            {
                return NoContent();

            }

            return Ok(CourseList);//return the list of courses

        }





        [HttpGet("{id}", Name = "GetCourseByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CourseDTO_Detailed> GetCourseByID(int id)
        {


            //we need to return the course usual info+Teachers(name,permission,id) of the course+

            if (id < 0)
            {
                return BadRequest("course id must be positive");
            }


            clsCourse Course = clsCourse.Find(id);

            if (Course == null)
            {

                return NoContent();
            }

            //now get the course teachers and store it in the course detailed dto
            //Course.CDTODetailed instead of Course.CDTO


            return Ok(Course.CDTO_Detailed);



        }




        [HttpGet("My", Name = "GetCoursesOfUserByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetCoursesOfUserByID()
        {

            if (!_authService.UserIsAuthenticated())
            {
                return Unauthorized("User is not logged in this moment");
            }


            var UserMode=_authService.getRole();
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


            if (id < 0)
            {
                return BadRequest("user id must be positive");
            }



            if ((enMode)mode == enMode.enTeacherMode)
            {

                List<CourseDTO> CourstList = clsCourseHandlers.GetCourseOfTeacherX(id);

                if (CourstList == null)
                {
                    return NoContent();
                }

                return Ok(CourstList);

            }
            else if ((enMode)mode == enMode.enStudentMode)
            {

                List<CourseDTO> CourstList = await clsSubscribtion.GetStudentCoursesAsync(id);

                if (CourstList == null)
                {
                    return NoContent();
                }

                return Ok(CourstList);


            }



            return BadRequest("Invalid Mode Value");



        }



        [HttpPost("Add", Name = "AddNewCourse")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<CourseDTO> AddNewCourse(AddCourseDTO request)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



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

            int mode = UserMode == "student" ? 1 : 2;

            if(mode==1)
                return Unauthorized("Students can t add courses");



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

           
            //create course object

            clsCourse new_Course = new clsCourse(request);
            if (new_Course.Save())
            {
                //we need to create a course handler record 
                //teacher id from authentication header
                //course id we just got from db in our course object
                //Permission = admin , because he is the one who created it 

                clsCourseHandlers CH = new clsCourseHandlers();
                CH.CourseID = new_Course.CourseID;
                CH.Permission = "admin";
                CH.TeacherID = id;// for now 

                if(!CH.Save()){
                    //throw an error because this course will be with no teachers here
                }

                return CreatedAtRoute("GetCourseByID", new { id = new_Course.CourseID }, new_Course.CDTO);

            }
            else
            {
                return BadRequest("somthing went wrong in the db connection part");
            }





        }

















    }
}
