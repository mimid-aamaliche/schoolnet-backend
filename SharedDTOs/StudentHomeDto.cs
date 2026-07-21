using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedDTOs
{
   public class StudentHomeDto
{
    public ProfileDto Profile { get; set; }
    public List<StudentCourseDto> Courses { get; set; }
    public StudentStatsDto Stats { get; set; }
}
}