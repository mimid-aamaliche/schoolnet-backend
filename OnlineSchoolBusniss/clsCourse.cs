using OnlineSchoolDataAccess;
using SharedDTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    public class clsCourse
    {
        enum enMode { enAddNewMode = 1, enUpdateMode }
        enMode CurrentMode;
        public int? CourseID { get; set; }

        [StringPropertyLength(2, 100)]
        public string CourseTitle { get; set; }

        [StringPropertyLength(2, 500)]
        public string CourseDescription { get; set; }

        public decimal CourseFeePerMonth { get; set; }

        public int? LevelRequiredID { get; set; }


        public CourseDTO CDTO { get { 
                return new CourseDTO( 
                    CourseID.HasValue? CourseID.Value:-1,
                    CourseTitle,
                    CourseDescription,
                    CourseFeePerMonth,
                    LevelRequiredID,
                    ImagePath);                   

            } }


        public CourseDTO_Detailed CDTO_Detailed { get
            {
                //here we get the teachers of the course and there permission
                List<CourseHandlerDTO> List = clsCourseHandlers.GetTeachersOfCourse(CourseID);
                

                //return the dto
                return new CourseDTO_Detailed(this.CDTO, List);


            } }



        public string ImagePath { get; set; }


        public clsCourse()
        {
            CourseID = null;
            CourseTitle = "";
            CourseDescription = "";
            CourseFeePerMonth = 0;
            LevelRequiredID = null;
            ImagePath = "";

            CurrentMode = enMode.enAddNewMode;



        }

        public clsCourse(AddCourseDTO courseInfo)
        {
            CourseID = null;
            CourseTitle = courseInfo.CourseTitle;
            CourseDescription = courseInfo.CourseDescription;
            CourseFeePerMonth = courseInfo.CourseFeePerMonth;
            LevelRequiredID = courseInfo.LevelRequiredID;
            ImagePath = courseInfo.ImagePath;

            CurrentMode = enMode.enAddNewMode;

        }


        private clsCourse(int? courseid, string coursetitle, string courseDescription,
            decimal feepermonth, int? level, string image)
        {
            CourseID = courseid;
            CourseTitle = coursetitle;
            CourseFeePerMonth = feepermonth;
            LevelRequiredID = level;
            ImagePath = image;
            CourseDescription = courseDescription; 
            CurrentMode=enMode.enUpdateMode;
        }

        private bool _AddNewCourse()
        {
            this.CourseID=clsSqlCourseData.AddNewCourse(this.CourseTitle, this.CourseDescription,this.CourseFeePerMonth,this.ImagePath,this.LevelRequiredID);
            return this.CourseID != null && this.CourseID != -1 ;
        }

        private bool _UpdateCourse()
        {
            return clsSqlCourseData.UpDateCourse(this.CourseID,this.CourseTitle, this.CourseDescription, this.CourseFeePerMonth, this.ImagePath, this.LevelRequiredID);
        }
        public static clsCourse Find(int? id)
        {
            string title="", description="",image="";
            int? Levelid = null;
            decimal Coursefees = 0;

            if(clsSqlCourseData.FindCourseByID(id,ref title,ref description,ref Coursefees,ref image,ref Levelid))
            {

                return new clsCourse(id, title, description, Coursefees, Levelid, image);
            }
            else
            {
                return null;
            }
            


            
        }
        public static List<CourseDTO> GetCoursesList()
        {

            return clsSqlCourseData.CoursesList();



        }

        public bool Save()
        {

            switch (this.CurrentMode)
            {

                case enMode.enAddNewMode:
                    if (_AddNewCourse())
                    {
                        this.CurrentMode = enMode.enUpdateMode;
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                case enMode.enUpdateMode:
                    return _UpdateCourse();


            }


            return false;
        }



    }
}
