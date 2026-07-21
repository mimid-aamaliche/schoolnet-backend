using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class AddTeacherDTO
    {



        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required ")]
        public string Password { get; set; }

        public int? SpecialisationID { get; set; }
        public string TeacherCV { get; set; }
        public string Notes { get; set; }

        [Required(ErrorMessage = "Person information is required")]
        public AddPersonDTO Person { get; set; }

        public AddTeacherDTO(string userName, string password, int? specialisationID, string teacherCV, string notes, AddPersonDTO person)
        {
            UserName = userName;
            Password = password;
            SpecialisationID = specialisationID;
            TeacherCV = teacherCV;
            Notes = notes;
            Person = person;
        }

    }
}
