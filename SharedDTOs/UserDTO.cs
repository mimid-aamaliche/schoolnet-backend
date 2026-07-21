using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public enum enMode { enStudentMode=1,enTeacherMode=2}
    public class UserDTO
    {
        public enMode Mode {  get; set; }

        public int Userid { get; set; }

        public string FullName { get; set; }

        

        public UserDTO(enMode mode, int userid, string fullname)
        {
            Mode = mode;
            Userid = userid;
            FullName = fullname;
        }
    }
}
