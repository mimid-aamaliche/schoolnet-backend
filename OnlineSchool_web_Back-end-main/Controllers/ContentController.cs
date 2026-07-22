using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private readonly ILogger<ContentController> _logger;

        public ContentController(clsMUXVideoService MuxVideoService,
                                 clsMUXSigningService MuxSigningService,
                                 IHttpContextAccessor ctxAccessor,
                                 IAuthService AuthService,
                                 ILogger<ContentController> logger)
        {
            _muxVideoService      = MuxVideoService;
            _muxSigningService    = MuxSigningService;
            _httpContextAccessor  = ctxAccessor;
            _authService          = AuthService;
            _logger               = logger;
        }


        // ─── GET api/Content/{courseid} ───────────────────────────────────────

        [HttpGet("{courseid}", Name = "GetCourseContents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetCourseContents(int courseid)
        {
            _logger.LogInformation("GetCourseContents | CourseID={CourseID}", courseid);

            if (courseid < 0)
            {
                _logger.LogWarning("GetCourseContents — invalid CourseID={CourseID}", courseid);
                return BadRequest("Invalide Course ID");
            }

            List<ContentDTO> ContentList = clsCourseContent.GetContentOfCourseX(courseid);

            if (!_authService.UserIsAuthenticated())
            {
                _logger.LogInformation("GetCourseContents — guest request | CourseID={CourseID} ItemCount={Count}", courseid, ContentList.Count);
                return Ok(new { ContentList, UserRole = new { role = "guest" } });
            }

            var UserMode = _authService.getRole();
            if (UserMode == null)
                throw new Exception("ContentController: GetCourseContents — user is authenticated but has no role claim");

            var UserId = _authService.GetUserID();
            if (UserId == null || !int.TryParse(UserId, out int id))
                throw new Exception("ContentController: GetCourseContents — user is authenticated but has no valid UserID claim");

            _logger.LogInformation("GetCourseContents — authenticated | UserID={UserID} Role={Role} CourseID={CourseID} ItemCount={Count}",
                id, UserMode, courseid, ContentList.Count);

            return Ok(new
            {
                ContentList,
                UserRole = new
                {
                    role = UserMode == "student"
                        ? await clsSubscribtion.StudentHasAccessAsync(id, courseid) ? "subscribed_student" : "new_student"
                        : "teacher"
                }
            });
        }


        // ─── GET api/Content/ContentID/{ContentId} ────────────────────────────

        [HttpGet("ContentID/{ContentId}", Name = "GetContentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetContentByID(int ContentId)
        {
            _logger.LogInformation("GetContentByID | ContentID={ContentID}", ContentId);

            if (ContentId < 0)
            {
                _logger.LogWarning("GetContentByID — invalid ContentID={ContentID}", ContentId);
                return BadRequest("Content id must be positive");
            }

            clsCourseContent Content = clsCourseContent.Find(ContentId);

            if (Content == null || string.IsNullOrEmpty(Content.AssetID))
            {
                _logger.LogWarning("GetContentByID — content not found or no AssetID | ContentID={ContentID}", ContentId);
                return NotFound("No content was found");
            }

            if (Content.AssetStatus != "ready")
            {
                _logger.LogInformation("GetContentByID — content not yet ready | ContentID={ContentID} Status={Status}",
                    ContentId, Content.AssetStatus);
                return NotFound("This Content is not ready yet ");
            }

            if (string.IsNullOrEmpty(Content.PlayBackID))
            {
                _logger.LogInformation("GetContentByID — no PlaybackID cached, fetching from MUX | ContentID={ContentID} AssetID={AssetID}",
                    ContentId, Content.AssetID);

                Content.PlayBackID = await _muxVideoService.GetAssetPlayBackID(Content.AssetID);

                if (string.IsNullOrEmpty(Content.PlayBackID))
                {
                    _logger.LogInformation("GetContentByID — no existing PlaybackID, creating new one | ContentID={ContentID}", ContentId);
                    Content.PlayBackID = await _muxVideoService.CreateAssetPlayBackID(Content.AssetID);
                }

                Content.Save();
                _logger.LogInformation("GetContentByID — PlaybackID obtained and saved | ContentID={ContentID} PlaybackID={PlaybackID}",
                    ContentId, Content.PlayBackID);
            }

            _logger.LogInformation("GetContentByID — returning signed URL | ContentID={ContentID}", ContentId);
            return Ok(new { content = Content.Content_DetailedDTO, Url = _muxSigningService.CreateSignedURL(Content.PlayBackID) });
        }


        // ─── POST api/Content/Add ─────────────────────────────────────────────

        [HttpPost("Add", Name = "AddNewContent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContentDTO>> AddNewContent([FromBody][Required] AddContentDTO request)
        {
            _logger.LogInformation("AddNewContent | Title={Title} CourseID={CourseID}", request.ContentTitle, request.CourseID);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("AddNewContent — invalid model state");
                return BadRequest(ModelState);
            }

            clsCourseContent Content = new clsCourseContent(request);

            if (Content.Save())
            {
                _logger.LogInformation("AddNewContent succeeded | ContentID={ContentID} CourseID={CourseID}",
                    Content.CourseContentID, request.CourseID);
                return CreatedAtRoute("GetDirectUploadUrl", new { id = Content.CourseContentID }, Content.Content_DTO);
            }

            _logger.LogError("AddNewContent failed — DB save error | Title={Title} CourseID={CourseID}", request.ContentTitle, request.CourseID);
            return BadRequest();
        }


        // ─── POST api/Content/DirectUpload/{ContentID} ────────────────────────

        [HttpPost("DirectUpload/{ContentID}", Name = "GetDirectUploadUrl")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContentDTO>> GetDirectUploadUrl(int ContentID)
        {
            _logger.LogInformation("GetDirectUploadUrl | ContentID={ContentID}", ContentID);

            if (ContentID < 0)
            {
                _logger.LogWarning("GetDirectUploadUrl — invalid ContentID={ContentID}", ContentID);
                return BadRequest("Invalide Content Id");
            }

            try
            {
                clsCourseContent Content = clsCourseContent.Find(ContentID);
                if (Content == null)
                {
                    _logger.LogWarning("GetDirectUploadUrl — content record not found | ContentID={ContentID}", ContentID);
                    return BadRequest("No content with this id is available in the db");
                }

                _logger.LogInformation("GetDirectUploadUrl — creating MUX direct upload | ContentID={ContentID}", ContentID);
                DirectUploadRecord? DirectUploadRecord = await _muxVideoService.CreateAsset();

                if (DirectUploadRecord == null)
                    throw new Exception("MUX CreateAsset returned null");

                Content.UploadID = DirectUploadRecord.upload_id;  // real AssetID comes later via webhook
                Content.AssetID = "";                              // cleared — set by MUX webhook once asset is ready
                Content.AssetStatus = "Uploading";

                Content.Save();

                _logger.LogInformation("GetDirectUploadUrl — upload URL created | ContentID={ContentID} UploadID={UploadID}",
                    ContentID, DirectUploadRecord.upload_id);

                return Ok(new { Url = DirectUploadRecord.upload_url, Content.Content_DTO });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDirectUploadUrl — unexpected error | ContentID={ContentID}", ContentID);
                return StatusCode(500, "somthing went wrong please try later");
            }
        }
    }
}
