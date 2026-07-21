using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class CourseHandlerDTO
    {
        public CourseHandlerDTO(int? teacherID, string teacherName, int? courseID, string permission)
        {
            TeacherID = teacherID;
            TeacherName = teacherName;
            CourseID = courseID;
            Permission = permission;
        }



        public int? TeacherID { get; set; }
        public string TeacherName { get; set; }
        public int? CourseID { get; set; }
        public string Permission { get; set; }


    }
}
