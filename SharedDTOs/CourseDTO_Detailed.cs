using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{   
    
    public class CourseDTO_Detailed
    {
        public CourseDTO courseinfo { get; set; }

        public  List<CourseHandlerDTO> teachers { get; set; }

        public CourseDTO_Detailed(CourseDTO courseinfo, List<CourseHandlerDTO> teachers)
        {
            this.courseinfo = courseinfo;
            this.teachers = teachers;
        }
    }
}
