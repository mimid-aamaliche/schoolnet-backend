using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class TeacherCourseDto
    {
        public int CourseID { get; set; }
    public string Title { get; set; }
    public string? Image { get; set; }
    public decimal FeesPerMonth { get; set; }
    public string Permission { get; set; }
    public int TotalContents { get; set; }
    public int TotalStudents { get; set; }
    }
}