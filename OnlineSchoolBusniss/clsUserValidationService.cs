using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedDTOs;

namespace OnlineSchoolBusniss
{
    public class clsUserValidationService
    {
        


        public enum enOperation
        {
            /*Teacher ops*/
            Add_Course=1,
            Delete_Course,
            AddContent_OnCourse,
            DeleteContent_OfCourse,


            /*Student  ops*/

            WatchContent_ofCourse,

            



        }




        public bool UserIsValide
            (
                UserDTO currentuser , 
                enOperation CurrentOperation ,
                int ? CourseID = null
            )
        {
            

            





            return true;
        }




        private bool UserCanAddCourse(UserDTO currentuser)
        {
            
            //every teacher can add new courses

            return currentuser.Mode==enMode.enTeacherMode;
        }

        
    




    }
}