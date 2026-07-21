using Azure.Core;
using OnlineSchoolDataAccess;
using SharedDTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    public class clsStudent
    {
        enum enMode { enAddNewMode=1,enUpdateMode=2}
        enMode _CurrentMode = enMode.enAddNewMode;
        public int? StudentID { get; set; }


        public string UserName { get; set; }


        public string Password { get; set; }

        public int? StudentLevelID { get; set; }

        public clsPerson PersonalInfo { get; set; }


        public UserDTO userDTO { get
            {
                return new UserDTO(SharedDTOs.enMode.enStudentMode, this.StudentID.Value, this.PersonalInfo.FullName());
            } }


        public clsStudent()
        {
            StudentID = null;
            UserName = "";
            Password ="";
            StudentLevelID = null;
            PersonalInfo = new clsPerson();

            _CurrentMode=enMode.enAddNewMode;
        }


        public clsStudent(AddStudentDTO StudentInfo)
        {
            StudentID = null;
            UserName = StudentInfo.UserName;
            Password = StudentInfo.Password;
            StudentLevelID = StudentInfo.StudentLevelID;
            PersonalInfo = new clsPerson(StudentInfo.Person);

            _CurrentMode = enMode.enAddNewMode;
        }


        public clsStudent(int? id,string username, string password,int? level,clsPerson personalinfo)
        {

            StudentID =   id;
            UserName =   username;
            Password =   password;
            StudentLevelID = level;
            PersonalInfo = personalinfo;
            _CurrentMode=enMode.enUpdateMode;

        }

        private bool _AddNewStudent()
        {

            this.StudentID=clsSqlStudentData.AddNewStudent(this.PersonalInfo.PersonID,this.UserName,this.Password,this.StudentLevelID);
            return this.StudentID != null;
        }


        private bool _UpdateStudent()
        {


            return  PersonalInfo.Save() &&
                clsSqlStudentData.UpDateStudent(this.StudentID,this.PersonalInfo.PersonID,this.UserName,this.Password,this.StudentLevelID);

        }


        public static clsStudent FindStudent(int st_id)
        {
            int? personid = null;
            string username = "";
            int? level = null;

            if(clsSqlStudentData.FindStudentById(st_id,ref personid,ref username,ref level))
            {
                return new clsStudent(st_id,username,"",level,clsPerson.Find(personid));

            }
            else
            {
                return null;
            }




        }

        public static DataTable GetStudentListInFormat()
        {
            return clsSqlStudentData.GetStudent_PersonListInFormat();



        }

        public static DataTable GetStudentList()
        {
            return clsSqlStudentData.GetStudent_PersonList();



        }

        public static clsStudent FindStudent(string username, string password)
        {
            int? personid = null;
            int? st_id = null;
            int? level = null;

            if (clsSqlStudentData.FindStudentByUserNameAnsPassword(username, password, ref personid,ref st_id, ref level))
            {
                return new clsStudent(st_id, username, password, level, clsPerson.Find(personid));

            }
            else
            {
                return null;
            }




        }

        public static bool StudentExist(int student_id)
        {
            return clsSqlStudentData.StudentExistByID(student_id);


        }
        public static bool StudentExist(string username, string password)
        {
            return clsSqlStudentData.StudentExistByUserNameAndPassword(username,password);


        }
        public static bool StudentExist(string username)
        {
            return clsSqlStudentData.StudentExistByUserName(username);


        }
        public bool Save()
        {
            switch (_CurrentMode)
            {
                case enMode.enAddNewMode:
                    if (_AddNewStudent())
                    {
                        this._CurrentMode = enMode.enUpdateMode;
                        return true;
                    }
                    else
                    {
                        return false;   
                    }

                case enMode.enUpdateMode:
                    return _UpdateStudent();
                    


            }


            return false;
        }

    }
}
