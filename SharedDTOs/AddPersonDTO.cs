using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class AddPersonDTO
    {

        [Required]
        public string NationalNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string SecondName { get; set; }


        public string ThirdName { get; set; }


        public string ForthName { get; set; }


        public string Image { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Must Be a Valid email Format 'example@esomeProvider.somthing")]
        public string Email { get; set; }

        [Required]
        [Phone(ErrorMessage ="Must be a valide phone numbre")]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public AddPersonDTO(string nationalNumber, string firstName, string secondName, string thirdName, string forthName, string image, DateTime? dateOfBirth, string email, string phoneNumber, string address)
        {
            NationalNumber = nationalNumber;
            FirstName = firstName;
            SecondName = secondName;
            ThirdName = thirdName;
            ForthName = forthName;
            Image = image;
            DateOfBirth = dateOfBirth;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }
    }
}
