using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SharedDTOs;

namespace OnlineSchoolDataAccess
{
    public class clsSqlHomeData
    {


        public static async Task<bool> StudentHomeData(int studentId, StudentHomeDto dto)
        {
            await using var conn = new SqlConnection(DataAccessConfiguration._connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("GetStudentHomePage", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@StudentID", studentId);

            await using var reader = await cmd.ExecuteReaderAsync();

            // Result set 1 — profile
            if (!await reader.ReadAsync()) return false;
            var profile = new ProfileDto
            {
                Id = reader.GetInt32("StudentID"),
                FullName = reader.GetString("FullName"),
                Image = reader.IsDBNull("Image") ? null : reader.GetString("Image"),
                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                UserName = reader.GetString("UserName"),
            };



            List<StudentCourseDto> courses = null;
            // Result set 2 — courses
            if (await reader.NextResultAsync())
            {
                courses = new List<StudentCourseDto>();
                while (await reader.ReadAsync())
                {
                    courses.Add(new StudentCourseDto
                    {
                        CourseID = reader.GetInt32("CourseID"),
                        Title = reader.GetString("Title"),
                        Image = reader.IsDBNull("Image") ? null : reader.GetString("Image"),
                        FeesPerMonth = reader.GetDecimal("FeesPerMonth"),
                        StartDate = reader.GetDateTime("StartDate"),
                        EndDate = reader.IsDBNull("EndDate") ? null : reader.GetDateTime("EndDate"),
                        IsPaid = reader.IsDBNull("IsPaid") ? null : reader.GetBoolean("IsPaid"),
                        TotalContents = reader.GetInt32("TotalContents"),
                    });
                }
            }


            StudentStatsDto stats = null;
            // Result set 3 — stats
            if (await reader.NextResultAsync())
            {
                await reader.ReadAsync();
                stats = new StudentStatsDto
                {
                    EnrolledCourses = reader.GetInt32("EnrolledCourses"),
                    PaidSubscriptions = reader.GetInt32("PaidSubscriptions"),
                    ActiveSubscriptions = reader.GetInt32("ActiveSubscriptions"),
                };
            }





            dto.Courses = courses;
            dto.Profile = profile;
            dto.Stats = stats;


            return true;
        }


        public static async Task<bool> TeacherHomeData(int teacherId, TeacherHomeDto dto)
        {

            await using var conn = new SqlConnection(DataAccessConfiguration._connectionString);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("GetTeacherHomePage", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TeacherID", teacherId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return false;
            var profile = new ProfileDto
            {
                Id = reader.GetInt32("TeacherID"),
                FullName = reader.GetString("FullName"),
                Image = reader.IsDBNull("Image") ? null : reader.GetString("Image"),
                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                UserName = reader.GetString("UserName"),
            };

            await reader.NextResultAsync();
            var courses = new List<TeacherCourseDto>();
            while (await reader.ReadAsync())
            {
                courses.Add(new TeacherCourseDto
                {
                    CourseID = reader.GetInt32("CourseID"),
                    Title = reader.GetString("Title"),
                    Image = reader.IsDBNull("Image") ? null : reader.GetString("Image"),
                    FeesPerMonth = reader.GetDecimal("FeesPerMonth"),
                    Permission = reader.GetString("Permission"),
                    TotalContents = reader.GetInt32("TotalContents"),
                    TotalStudents = reader.GetInt32("TotalStudents"),
                });
            }

            await reader.NextResultAsync();
            await reader.ReadAsync();
            var stats = new TeacherStatsDto
            {
                TotalCourses = reader.GetInt32("TotalCourses"),
                TotalStudents = reader.GetInt32("TotalStudents"),
                TotalContent = reader.GetInt32("TotalContent"),
            };

            dto.Courses = courses;
            dto.Profile = profile;
            dto.Stats = stats;

            
            return true;

        }


    }
}   
