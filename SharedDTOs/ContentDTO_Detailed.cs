using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class ContentDTO_Detailed
    {

        public int? CourseContentID { get; set; }
        public string ContentType { get; set; }
        public string ContentPath { get; set; }
        public int? CourseID { get; set; }
        public string ContentTitle { get; set; }
        public string Description { get; set; }
        public DateTime DateOfRealease { get; set; }
        public string Notes { get; set; }


        //field for mux service
        public string PlayBackID { get; set; }
        public string AssetID { get; set; }
        public string AssetStatus { get; set; }

        public ContentDTO_Detailed(int? courseContentID, string contentType, string contentPath, int? courseID, string contentTitle, string description, DateTime dateOfRealease, string notes, string playBackID, string assetID, string assetStatus)
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
            AssetStatus = assetStatus;
        }
    }
}
