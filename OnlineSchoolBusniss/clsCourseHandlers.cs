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
    public class clsCourseHandlers
    {
        enum enMode { enAddNewMode = 1, enUpdateMode = 2 }
        enMode _CurrentMode = enMode.enAddNewMode;


        public int? RecordID { get; set; }
        public int? TeacherID { get; set; }
        public int? CourseID { get; set; }
        public string Permission {  get; set; }

        public clsCourseHandlers()
        {
            RecordID = null;
            TeacherID = null;
            Permission = "";
            CourseID = null;
            _CurrentMode = enMode.enAddNewMode;

        }
        private clsCourseHandlers(int? recordID, int? teacherID, string permission,int? courseid)
        {
            RecordID = recordID;
            TeacherID = teacherID;
            Permission = permission;
            CourseID = courseid;

            _CurrentMode = enMode.enUpdateMode;
        }


        private bool _AddNewCourseHandler()
        {

           this.RecordID=clsSqlCourseHandlersData.AddNewCourseHandler(Permission,CourseID,TeacherID);
            return this.RecordID != null;


        }

        private bool _UpdateCourseHandler()
        {



             return clsSqlCourseHandlersData.UpdateCourseHandler(RecordID,Permission,CourseID,TeacherID);
        }

        public static clsCourseHandlers Find(int? TeacherID,int?CourseID)
        {
            string permission = "";
            int? recordID = null;
            if (clsSqlCourseHandlersData.FindCourseHandlerRecordByTeacherandCourseID(TeacherID,CourseID,ref recordID,ref permission))
            {

                return new clsCourseHandlers(recordID,TeacherID, permission, CourseID);
            }
            else
            {
                return null;
            }




        }

         public static List< CourseDTO> GetCourseOfTeacherX(int? TeacherID)
         {
            return clsSqlCourseHandlersData.GetCoursesOfTeacherX(TeacherID);
         }

        public static List<CourseHandlerDTO> GetTeachersOfCourse(int? CourseID)
        {
            return clsSqlCourseHandlersData.GetTeachersOfCourse(CourseID) ;
        }





        public bool Save()
        {
            switch (_CurrentMode)
            {
                case enMode.enAddNewMode:
                    if (_AddNewCourseHandler())
                    {
                        this._CurrentMode = enMode.enUpdateMode;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.enUpdateMode:
                    return _UpdateCourseHandler();



            }


            return false;
        }


    }
}
