using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedDTOs;
using OnlineSchoolDataAccess;


namespace OnlineSchoolBusniss
{
    public class clsHome
    {

        //fields

        public static async Task<StudentHomeDto> GetStudentHomeData(int? id)
        {

            StudentHomeDto dto = new StudentHomeDto();

            if (await clsSqlHomeData.StudentHomeData(id.Value,dto))
            {

                return dto;
            }
            else
            {
                return null;
            }




        }



        public static async Task<TeacherHomeDto> GetTeacherHomeData(int? id)
        {

            TeacherHomeDto dto = new TeacherHomeDto();

            if (await clsSqlHomeData.TeacherHomeData(id.Value, dto))
            {

                return dto;
            }
            else
            {
                return null;
            }




        }


    }
}