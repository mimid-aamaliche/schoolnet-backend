using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class StudentCourseDto
{
    public int CourseID { get; set; }
    public string Title { get; set; }
    public string? Image { get; set; }
    public decimal FeesPerMonth { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsPaid { get; set; }
    public int TotalContents { get; set; }
}
}