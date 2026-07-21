using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class ContentDTO
    {
        public int? CourseContentID { get; set; }
        public string ContentType { get; set; }
        
        public string ContentTitle { get; set; }
        public string Description { get; set; }
        public DateTime DateOfRealease { get; set; }

        public ContentDTO(int? courseContentID, string contentType, string contentTitle, string description, DateTime dateOfRealease)
        {
            CourseContentID = courseContentID;
            ContentType = contentType;
            ContentTitle = contentTitle;
            Description = description;
            DateOfRealease = dateOfRealease;
        }
    }
}
