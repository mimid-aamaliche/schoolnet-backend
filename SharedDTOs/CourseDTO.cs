using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class CourseDTO
    {
        public CourseDTO(int courseID, string courseTitle, string? courseDescription, decimal courseFeePerMonth, int? levelRequiredID, string? imagePath)
        {
            CourseID = courseID;
            CourseTitle = courseTitle;
            CourseDescription = courseDescription;
            CourseFeePerMonth = courseFeePerMonth;
            LevelRequiredID = levelRequiredID;
            ImagePath = imagePath;
        }

        public int CourseID { get; set; }
        public string CourseTitle { get; set; }
        public string? CourseDescription { get; set; }
        public decimal CourseFeePerMonth { get; set; }
        public int? LevelRequiredID { get; set; }
        public string? ImagePath { get; set; }




    }
}
