using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class AddCourseDTO
    {

        public AddCourseDTO( string courseTitle, string? courseDescription, decimal courseFeePerMonth, int? levelRequiredID, string? imagePath)
        {
            CourseTitle = courseTitle;
            CourseDescription = courseDescription;
            CourseFeePerMonth = courseFeePerMonth;
            LevelRequiredID = levelRequiredID;
            ImagePath = imagePath;
        }


        [Required(ErrorMessage =" Course must have a title ")]
        public string CourseTitle { get; set; }


        public string? CourseDescription { get; set; }

        [Required(ErrorMessage =" Course must have a fee ")]
        public decimal CourseFeePerMonth { get; set; }
        public int? LevelRequiredID { get; set; }
        public string? ImagePath { get; set; }




    }
}
