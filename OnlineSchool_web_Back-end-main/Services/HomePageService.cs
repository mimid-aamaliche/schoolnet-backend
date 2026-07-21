using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineSchoolBusniss;
using SharedDTOs;

namespace OnlineSchool_Back_end.Services
{
    public class HomePageService
    {
        




        public async Task<StudentHomeDto> StudentHomePageData(int id)
        {
            //return dto
            return await clsHome.GetStudentHomeData(id);
        }


         public async Task<TeacherHomeDto> TeacherHomePageData(int id)
        {
            //return dto
            return  await  clsHome.GetTeacherHomeData(id);
        }

    }
}