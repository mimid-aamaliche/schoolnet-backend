using OnlineSchoolDataAccess;
using SharedDTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OnlineSchoolBusniss
{
    public class clsTeacher
    {
        enum enMode { enAddNewMode = 1, enUpdateMode = 2 }
        enMode _CurrentMode = enMode.enAddNewMode;

        public int? TeacherID { get; set; }

        public string TeacherStripeID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }    
        public clsPerson PersonalInfo { get; set; }
        public int? SpecialisationID { get; set; }
        public string TeacherCV { get; set; }
        public string Notes { get; set; }


        public UserDTO userDTO
        {
            get
            {

                return new UserDTO(SharedDTOs.enMode.enTeacherMode, this.TeacherID.Value, this.PersonalInfo.FullName());


            }
        }

        public clsTeacher()
        {
            TeacherID = null;
            UserName = "";
            Password = "";
            SpecialisationID = null;
            PersonalInfo = new clsPerson();
            TeacherCV = "";
            Notes = "";


            _CurrentMode = enMode.enAddNewMode;



        }

        public clsTeacher(AddTeacherDTO teacherInfo)
        {
            TeacherID = null;
            UserName = teacherInfo.UserName;
            Password = teacherInfo.Password;
            SpecialisationID = teacherInfo.SpecialisationID;
            PersonalInfo = new clsPerson(teacherInfo.Person);
            TeacherCV = teacherInfo.TeacherCV;
            Notes = teacherInfo.Notes;


            _CurrentMode = enMode.enAddNewMode;



        }

        private clsTeacher(int? id,string Th_StripeId, string username, string password, int? Specialisation, clsPerson personalinfo,string cv, string notes)
        {

            TeacherID = id;
            TeacherStripeID=Th_StripeId;
            UserName = username;
            Password = password;
            SpecialisationID = Specialisation;
            PersonalInfo = personalinfo;
            TeacherCV = cv;
            Notes = notes;


            _CurrentMode = enMode.enUpdateMode;

        }
        private bool _AddNewTeacher()
        {

            this.TeacherID = clsSqlTeacherData.AddNewTeacher(this.PersonalInfo.PersonID,this.TeacherStripeID, this.UserName, this.Password, this.SpecialisationID,this.TeacherCV,this.Notes);
            return this.TeacherID != null;
        }

        private bool _UpdateTeacher()
        {


            return PersonalInfo.Save() &&
                clsSqlTeacherData.UpdateTeacher(this.TeacherID, this.PersonalInfo.PersonID,this.TeacherStripeID, this.UserName, this.Password, this.SpecialisationID, this.TeacherCV, this.Notes);

        }

        public static clsTeacher FindTeacher(int Th_id)
        {
            int? personid = null;
            string username = "";
            string password = "";
            int? specialisationid = null;
            string cv = "";
            string notes ="";
            string Th_StripeID = "";

            if (clsSqlTeacherData.FindTeacherByID(Th_id,ref Th_StripeID, ref personid, ref username, ref password, ref specialisationid,ref cv,ref notes))
            {
                return new clsTeacher(Th_id, Th_StripeID, username, password, specialisationid, clsPerson.Find(personid),cv,notes);

            }
            else
            {
                return null;
            }




        }

        public static clsTeacher FindTeacher(string username, string password)
        {
            int? personid = null;
            int? Th_id=null;
            int? specialisationid = null;
            string cv = "";
            string notes = "";
            string Th_StripeID = "";

            if (clsSqlTeacherData.FindTeacherByUserNameAndPassword( username, password,ref Th_StripeID,ref Th_id, ref personid, ref specialisationid, ref cv, ref notes))
            {
                return new clsTeacher(Th_id, Th_StripeID, username, password, specialisationid, clsPerson.Find(personid), cv, notes);

            }
            else
            {
                return null;
            }




        }

        public static DataTable TeachersListInFormat()
        {


            return clsSqlTeacherData.GetTeacher_PersonListInFormat();



        }
        public static bool TecherExist(int TeacherId)
        {
            return clsSqlTeacherData.TeacherExistByID(TeacherId);
        }
        public static bool TecherExist(string username,string password)
        {
            return clsSqlTeacherData.TeacherExistByUserNameAndPassword(username, password);
        }
        public static bool TecherExist(string username)
        {
            return clsSqlTeacherData.TeacherExistByUserName(username);
        }
        public bool Save()
        {
            switch (_CurrentMode)
            {
                case enMode.enAddNewMode:
                    if (_AddNewTeacher())
                    {
                        this._CurrentMode = enMode.enUpdateMode;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.enUpdateMode:
                    return _UpdateTeacher();



            }


            return false;
        }


    }
}
