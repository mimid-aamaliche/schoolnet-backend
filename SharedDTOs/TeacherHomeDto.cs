using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedDTOs
{
   public class TeacherHomeDto
{
    public ProfileDto Profile { get; set; }
    public List<TeacherCourseDto> Courses { get; set; }
    public TeacherStatsDto Stats { get; set; }
}
}