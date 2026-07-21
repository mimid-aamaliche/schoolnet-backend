using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class AddStudentDTO
    {


        [Required]
        public string UserName { get; set; }

        [Required]  
        public string Password { get; set; }

        public int? StudentLevelID { get; set; }


        [Required]  
        public AddPersonDTO Person { get; set; }

        public AddStudentDTO(string userName, string password, int? studentLevelID, AddPersonDTO person)
        {
            UserName = userName;
            Password = password;
            StudentLevelID = studentLevelID;
            Person = person;
        }
    }
}
