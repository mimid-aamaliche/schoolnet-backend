using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class AddContentDTO
    {
        [Required(ErrorMessage ="Content type field is required ")]
        [RegularExpression("^(Video)$",ErrorMessage =" Content Type must be either Video (Pdf Would be supported in the futur )")]
        public string ContentType { get; set; }

        [Required(ErrorMessage ="Content type field is required ")]
        [Range(0,int.MaxValue,ErrorMessage ="CourseId Can t be a negative Numbre ")]
        public int? CourseID { get; set; }

        [Required(ErrorMessage ="Content Must have a title ")]
        public string ContentTitle { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }

        public AddContentDTO(string contentType, int? courseID, string contentTitle,string description,  string notes)
        {
            ContentType = contentType;
            CourseID = courseID;
            ContentTitle = contentTitle;
            Description = description;
            Notes = notes;
        }
    }
}
