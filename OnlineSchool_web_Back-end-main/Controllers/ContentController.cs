using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineSchool_Back_end.Services;
using OnlineSchoolBusniss;
using SharedDTOs;

namespace OnlineSchool_Back_end.Controllers
{
    [Route("api/Content")]
    [ApiController]
    public class ContentController : ControllerBase
    {



        private readonly clsMUXVideoService _muxVideoService;
        private readonly clsMUXSigningService _muxSigningService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;


        public ContentController(clsMUXVideoService MuxVideoService,
                                clsMUXSigningService MuxSigningService,
                                IHttpContextAccessor ctxAccessor,
                                IAuthService AuthService)
        {
            _muxVideoService = MuxVideoService;
            _muxSigningService = MuxSigningService;
            _httpContextAccessor = ctxAccessor;
            _authService = AuthService;

        }









        [HttpGet("{courseid}", Name = "GetCourseContents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetCourseContents(int courseid)
        {





            if (courseid < 0) { return BadRequest("Invalide Course ID"); }

            List<ContentDTO> ContentList = clsCourseContent.GetContentOfCourseX(courseid);

            if (!_authService.UserIsAuthenticated())
            {
                return Ok(new
                {
                    ContentList = ContentList,
                    UserRole = new { role = "guest" }
                });
            }

            var UserMode = _authService.getRole();

            if (UserMode == null)
                throw new Exception("Content Conteroller : GetCourseContent EndPoint : user is authenticated but has no role claim ");


            var UserId = _authService.GetUserID();
            if (UserId == null || !int.TryParse(UserId, out int id))
                throw new Exception("Content Conteroller : GetCourseContent EndPoint : user is authenticated but has no User id claim (or curropted one) ");



            return Ok(new
            {
                ContentList = ContentList,
                UserRole = new
                {
                    role = UserMode == "student" ?
                     await clsSubscribtion.StudentHasAccessAsync(id, courseid) ? "subscribed_student" : "new_student"
                     : "teacher"
                }
            });







        }







        [HttpGet("ContentID/{ContentId}", Name = "GetContentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult> GetContentByID(int ContentId)
        {


            if (ContentId < 0)
            {
                return BadRequest("Content id must be positive");
            }


            clsCourseContent Content = clsCourseContent.Find(ContentId);



            if (Content == null || string.IsNullOrEmpty(Content.AssetID))
            {
                return NotFound("No content was found");
            }

            //return only Ready to watch Content
            if (Content.AssetStatus != "ready")
            {
                return NotFound("This Content is not ready yet ");
            }


            //if we are here then the asset is ready and can be streamed

            if (string.IsNullOrEmpty(Content.PlayBackID))
            {
                //we  retrieve the asset playback id
                Content.PlayBackID = await _muxVideoService.GetAssetPlayBackID(Content.AssetID);


                // If none exists, create it and store it in  DB
                if (string.IsNullOrEmpty(Content.PlayBackID))
                {
                    Content.PlayBackID = await _muxVideoService.CreateAssetPlayBackID(Content.AssetID);
                }
                // Persist the new playback ID in  DB
                Content.Save();

            }


            //after retriving the playback id we will create the signed url
            //signed usel is just header.payload encrypted by mux secret key 
            //we gonna late the mux signing service do the work here 



            //we still need a last check here to see if the playback id exist



            return Ok(new { content = Content.Content_DetailedDTO, Url = _muxSigningService.CreateSignedURL(Content.PlayBackID) });



        }





        //add content

        [HttpPost("Add", Name = "AddNewContent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ContentDTO>> AddNewContent([FromBody][Required] AddContentDTO request)
        {

            //Input Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Note : WE MUST ADD TO DB DESIGN A CREATEDBY FIELD TO KEEP TRACK OF CONTENT CREATORS

            clsCourseContent Content = new clsCourseContent(request);

            
            
            if (Content.Save())
            {
                return CreatedAtRoute("GetDirectUploadUrl", new { id = Content.CourseContentID }, Content.Content_DTO);
            }
            else
            {
                return BadRequest();
            }


        }






        [HttpPost("DirectUpload/{ContentID}", Name = "GetDirectUploadUrl")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ContentDTO>> GetDirectUploadUrl(int ContentID)
        {
            if (ContentID < 0) { return BadRequest("Invalide Content Id"); }

            try
            {


                //a content with this content id must exist in db
                //check here

                clsCourseContent Content = clsCourseContent.Find(ContentID);
                if (Content == null)
                    return BadRequest("No content with this id is available in the db");


                //here we start the direct upload process
                DirectUploadRecord? DirectUploadRecord = await _muxVideoService.CreateAsset();


                //if the Upload url is still null meaning somthing went wrong again 
                //the user should try later
                if (DirectUploadRecord == null)
                    throw new Exception();


                Content.AssetID = DirectUploadRecord.upload_id;
                Content.AssetStatus = "Uploading";

                Content.Save();//TODO : MAKE AN ASYNC VERSION OF ALL THE SAVE FUNCTIONS

                return Ok(new { Url = DirectUploadRecord.upload_url, Content.Content_DTO });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "somthing went wrong please try later");
            }




        }









        //create content flow 
        //1) we create the content and restun the contentUrl(where the FR gonna upload the video)
        //2) FR upload the video to the url
        //3) FR alert us if somthing went wrong




        //getContentByID flow
        //we get the content info from db 
        //we get the content signed url from sprout video api by the contentId












    }
}

