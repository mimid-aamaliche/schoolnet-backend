using OnlineSchoolDataAccess;
using SharedDTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    public class clsCourseContent
    {

        enum enMode { enAddNewMode = 1, enUpdateMode }
        private enMode CurrentMode;
        public int? CourseContentID { get; set; }
        public string ContentType { get; set; }
        public string ContentPath { get; set; }


        public int? CourseID { get; set; }
        public string ContentTitle { get; set; }
        public string Description { get; set; }
        public DateTime DateOfRealease { get; set; }
        public string Notes { get; set; }



        //fields for mux service
        public string PlayBackID { get; set; }
        public string AssetID { get; set; }
        public string UploadID { get; set; }   // MUX direct-upload ID — stored when upload URL is generated
        public string AssetStatus { get; set; }




        public ContentDTO Content_DTO { get
            {



                return new ContentDTO(CourseContentID, ContentType, ContentTitle, Description, DateOfRealease);



            }
        }


        public ContentDTO_Detailed Content_DetailedDTO
        {
            get
            {

                return new ContentDTO_Detailed(CourseContentID, ContentType, ContentPath, CourseID, ContentTitle,
                    Description, DateOfRealease, Notes, PlayBackID, AssetID, AssetStatus);

            }



        }




        //Constructors used to add content
        public clsCourseContent()
        {
            CourseContentID = null;
            ContentType = "";
            ContentPath = "";
            ContentTitle = "";
            Description = "";
            Notes = "";
            DateOfRealease = DateTime.Now;
            CourseID = null;
            PlayBackID = "";
            AssetID = "";
            UploadID = "";
            AssetStatus = "";


            CurrentMode = enMode.enAddNewMode;
          

        }

       

        public clsCourseContent(AddContentDTO contentInfo)
        {
            CourseContentID = null;
            ContentType = contentInfo.ContentType;
            ContentTitle = contentInfo.ContentTitle;
            Description = contentInfo.Description;
            Notes = contentInfo.Notes;
            DateOfRealease = DateTime.Now;
            CourseID = contentInfo.CourseID;
            PlayBackID = "";
            AssetID = "";
            UploadID = "";
            AssetStatus = "";


            CurrentMode = enMode.enAddNewMode;


        }




        //Constructors used to update content

        public clsCourseContent(int? courseContentID, string contentType, string contentPath, int? courseID, string contentTitle, string description, DateTime dateOfRealease, string notes, string playBackID, string assetID, string uploadID, string assetStatus)
        {
            CourseContentID = courseContentID;
            ContentType = contentType;
            ContentPath = contentPath;
            CourseID = courseID;
            ContentTitle = contentTitle;
            Description = description;
            DateOfRealease = dateOfRealease;
            Notes = notes;
            PlayBackID = playBackID;
            AssetID = assetID;
            UploadID = uploadID;
            AssetStatus = assetStatus;
            CurrentMode = enMode.enUpdateMode;

        }

        private bool _AddNewCourseContent()
        {
            this.CourseContentID = clsSqlCourseContentData.AddNewCourseContent(ContentTitle, Description, ContentType,
                                                                ContentPath, CourseID, Notes, DateOfRealease,
                                                                PlayBackID, AssetID, UploadID, AssetStatus);

            return CourseContentID != null;

        }

        private bool _UpDateCourseContent()
        {
            return clsSqlCourseContentData.UpDateCourseContent(CourseContentID, ContentTitle,
                                  Description, ContentType,
                                  ContentPath, CourseID, Notes, DateOfRealease,
                                  PlayBackID, AssetID, UploadID, AssetStatus);


        }













        public static clsCourseContent Find(int? contentID)
        {
            string desc = "", notes = "", contentpath = "", type = "", title = "", playbackid = "", assetid = "", uploadid = "", status = "";
            DateTime releasedate = DateTime.MinValue;
            int? courseid = null;
            if (clsSqlCourseContentData.FindCourseContentByID(contentID, ref title, ref desc, ref type, ref contentpath,
                ref courseid, ref notes, ref releasedate, ref playbackid, ref assetid, ref uploadid, ref status))
            {
                return new clsCourseContent(contentID, type, contentpath, courseid, title, desc, releasedate, notes, playbackid, assetid, uploadid, status);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Look up a content record by its MUX UploadID.
        /// Used by webhook handlers to find the record without relying on MUX metadata.
        /// </summary>
        public static clsCourseContent FindByUploadID(string uploadID)
        {
            if (string.IsNullOrEmpty(uploadID)) return null;

            int? contentid = null;
            string desc = "", notes = "", contentpath = "", type = "", title = "", playbackid = "", assetid = "", status = "";
            DateTime releasedate = DateTime.MinValue;
            int? courseid = null;

            if (clsSqlCourseContentData.FindByUploadID(uploadID, ref contentid, ref title, ref desc, ref type,
                ref contentpath, ref courseid, ref notes, ref releasedate, ref playbackid, ref assetid, ref status))
            {
                // Note: UploadID is passed as-is since we searched by it
                return new clsCourseContent(contentid, type, contentpath, courseid, title, desc, releasedate, notes, playbackid, assetid, uploadID, status);
            }
            else
            {
                return null;
            }
        }







        public static List<ContentDTO> GetContentOfCourseX(int? courseid)
        {

            return clsSqlCourseContentData.CoursesXContentList(courseid);



        }








        public bool Save()
        {

            switch (this.CurrentMode)
            {

                case enMode.enAddNewMode:
                    if (_AddNewCourseContent())
                    {
                        this.CurrentMode = enMode.enUpdateMode;
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                case enMode.enUpdateMode:
                    return _UpDateCourseContent();


            }


            return false;
        }



    }
}
