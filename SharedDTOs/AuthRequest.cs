using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public  class AuthRequest
    {

        [Required]  
        [Range(1,2,ErrorMessage =" Mode should be either 1 for student or 2 for teacher")]
        public enMode Mode {  get; set; }

        [Required]  
        public string Username { get; set; }


        [Required]  
        public string Password { get; set; }




    }
}
