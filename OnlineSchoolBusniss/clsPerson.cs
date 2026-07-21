using OnlineSchoolDataAccess;
using SharedDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OnlineSchoolBusniss
{
    public class clsPerson
    {
        enum enMode { enAddNewMode=1,enUpdateMode}
        enMode CurrentMode;

        public int? PersonID { get; set; }

        [StringPropertyLength(2, 50)]
        public string NationalNumber { get; set; }

        [StringPropertyLength(2,50)]
        public string FirstName  { get; set; }


        [StringPropertyLength(2,50)]
        public string SecondName { get; set; }

        
        public string ThirdName    { get; set; }

       
        public string ForthName  { get; set; }

        
        public string Image     { get; set; }


        public DateTime? DateOfBirth    { get; set; }


        [StringPropertyLength(0, 500)]
        public string Email      { get; set; }

        [StringPropertyLength(0, 20)]
        public string PhoneNumber { get; set; }

        [StringPropertyLength(0, 500)]
        public string Address      { get; set; }



        public clsPerson()
        {
            PersonID = null;
            DateOfBirth = null;

            CurrentMode = enMode.enAddNewMode;
        }

        public clsPerson(AddPersonDTO personInfo)
        {
            PersonID = null;
            DateOfBirth = personInfo.DateOfBirth;
            NationalNumber = personInfo.NationalNumber;
            FirstName = personInfo.FirstName;
            SecondName = personInfo.SecondName;
            ThirdName = personInfo.ThirdName;
            Image = personInfo.Image;
            Email = personInfo.Email;
            PhoneNumber = personInfo.PhoneNumber;
            Address = personInfo.Address;
            ForthName = personInfo.ForthName;


            CurrentMode = enMode.enAddNewMode;
        }

        public clsPerson(int? id, string nationalNumber, string firstName, string secondName, string thirdName
            , string forthName, string image, DateTime? dateOfBirth, string email, string phoneNumber
            , string address)
        {
            NationalNumber = nationalNumber;
            PersonID = id;
            FirstName = firstName;
            SecondName = secondName;
            ThirdName = thirdName;
            Image = image;
            DateOfBirth = dateOfBirth;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            ForthName = forthName;

            CurrentMode = enMode.enUpdateMode;

        }


        public string FullName()
        {
            return new StringBuilder(FirstName).Append(SecondName).ToString();

        }
         private bool _AddNewPerson()
        {

            this.PersonID=clsSqlPersonData.AddNewPerson(this.NationalNumber,this.FirstName,this.SecondName,this.ThirdName
                ,this.ForthName,this.Image,this.DateOfBirth,this.Email,this.PhoneNumber,this.Address);

            return PersonID!=null;
        }



        public static clsPerson Find(int? id)
        {
            string first = "";
            string second ="", third = "", forth = "", email = "", phone= "", address = "", image = "";
            DateTime? dateOfBirth=DateTime.Now;
            string nationalnum="";

            if(clsSqlPersonData.FindPerson(id ?? -1,ref nationalnum,ref first,ref second,ref third,ref forth,ref image,ref dateOfBirth,ref email,ref phone,ref address))
            {


                return new clsPerson(id,nationalnum,first,second,third,forth,image,dateOfBirth,email,phone,address);


            }
            else
            {

                return null;
            }



        }
        public static bool PersonExist(int? id)
        {
            return clsSqlPersonData.IsPersonExist(id);


        }

        private bool _UpdatePerson()
        {
            return clsSqlPersonData.UpDatePerson(this.PersonID,this.NationalNumber, this.FirstName, this.SecondName, this.ThirdName
                , this.ForthName, this.Image, this.DateOfBirth, this.Email, this.PhoneNumber, this.Address);

        }

        public bool Save()
        {

            switch (this.CurrentMode)
            {

                case enMode.enAddNewMode:
                    if (_AddNewPerson()) {
                    this.CurrentMode = enMode.enUpdateMode;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    

                case enMode.enUpdateMode:
                    return _UpdatePerson();
                    

            }


            return false;
        }



    }
}
