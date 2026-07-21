using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SharedDTOs;

namespace OnlineSchoolDataAccess
{
    public class clsSqlCourseHandlersData
    {
        public static int? AddNewCourseHandler(string Permission, int? CourseID,int? TeacherID)
        {
            int? newRecordID = null;
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_AddNew_CourseHandler", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //the the values of the new record in the specialisation table 
                        command.Parameters.AddWithValue("@Permission", Permission);

                       
                        command.Parameters.AddWithValue("@CourseID", CourseID);
                      
                        
                        command.Parameters.AddWithValue("@TeacherID", TeacherID);
                       


                        SqlParameter InsertedID = new SqlParameter("@InsertedID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        command.Parameters.Add(InsertedID);
                        connection.Open();
                        command.ExecuteNonQuery();
                        newRecordID = (int)command.Parameters["@InsertedID"].Value;


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }

            return newRecordID;
        }

        public static async Task<int?> AddNewCourseHandlerAsync(string Permission, int? CourseID,int? TeacherID)
        {
            int? newRecordID = null;
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_AddNew_CourseHandler", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //the the values of the new record in the specialisation table 
                        command.Parameters.AddWithValue("@Permission", Permission);

                       
                        command.Parameters.AddWithValue("@CourseID", CourseID);
                      
                        
                        command.Parameters.AddWithValue("@TeacherID", TeacherID);
                       


                        SqlParameter InsertedID = new SqlParameter("@InsertedID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        command.Parameters.Add(InsertedID);
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        newRecordID = (int)command.Parameters["@InsertedID"].Value;


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }

            return newRecordID;
        }









        public static bool UpdateCourseHandler(int? ID, string Permission, int? CourseID, int? TeacherID)
        {
            if (!ID.HasValue)
            {
                return false;
            }

            bool IsUpdated = false;
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Update_CourseHandler", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //the the new values of the record in the specialisation table 

                        command.Parameters.AddWithValue("@RecordID", ID);

                        command.Parameters.AddWithValue("@Permission", Permission);

                       
                        command.Parameters.AddWithValue("@CourseID", CourseID);

                        command.Parameters.AddWithValue("@TeacherID", TeacherID);

                        SqlParameter InsertedID = new SqlParameter("@Updated", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };

                        command.Parameters.Add(InsertedID);
                        connection.Open();
                        command.ExecuteNonQuery();
                        IsUpdated = (int)command.Parameters["@Updated"].Value == 1;


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }

            return IsUpdated;
        }

        public static async Task<bool> UpdateCourseHandlerAsync(int? ID, string Permission, int? CourseID, int? TeacherID)
        {
            if (!ID.HasValue)
            {
                return false;
            }

            bool IsUpdated = false;
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Update_CourseHandler", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //the the new values of the record in the specialisation table 

                        command.Parameters.AddWithValue("@RecordID", ID);

                        command.Parameters.AddWithValue("@Permission", Permission);

                       
                        command.Parameters.AddWithValue("@CourseID", CourseID);

                        command.Parameters.AddWithValue("@TeacherID", TeacherID);

                        SqlParameter InsertedID = new SqlParameter("@Updated", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };

                        command.Parameters.Add(InsertedID);
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        IsUpdated = (int)command.Parameters["@Updated"].Value == 1;


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }

            return IsUpdated;
        }







        //this gonne be used to serch for the permission of the teacher on the specific course
        //that Permission determines what he can do in a course (delete content , add content , ban student,inban student,add other teachers to handel course
        public static bool FindCourseHandlerRecordByTeacherandCourseID(int? TeacherID ,int? CoursID, ref int? ID, ref string Permission)
        {
            if (!TeacherID.HasValue) { return false; }
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByTeacherAndCourseIDs_CourseHandlers", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@TeacherID", TeacherID);
                        command.Parameters.AddWithValue("@CourseID", CoursID);


                        command.Parameters.Add(new SqlParameter("@RecordID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@Permission", SqlDbType.NVarChar, 500)
                        {
                            Direction = ParameterDirection.Output,
                        });



                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);



                        connection.Open();
                        command.ExecuteNonQuery();
                        IsFound = (int)returnParameter.Value == 1;

                        if (IsFound)
                        {

                            Permission = command.Parameters["@Permission"].Value == DBNull.Value ? "" : (string)command.Parameters["@Permission"].Value;
                            ID = command.Parameters["@RecordID"].Value == DBNull.Value ? null : (int?)command.Parameters["@RecordID"].Value;



                        }



                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }

            return IsFound;
        }








        public static List<CourseDTO> GetCoursesOfTeacherX(int? TeacherID)
        {
           

            List<CourseDTO> CoursesList = new List<CourseDTO>();
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_TeacherCourses_CourseHandler", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@TeacherID", TeacherID);




                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            int idIndex=reader.GetOrdinal("id");
                            int TitleIndex=reader.GetOrdinal("Title");
                            int DescriptionIndex=reader.GetOrdinal("Description");
                            int FeesPerMonthIndex=reader.GetOrdinal("FeesPerMonth");
                            int LevelRequiredIDIndex=reader.GetOrdinal("LevelRequiredID");
                            int ImageIndex=reader.GetOrdinal("Image");


                            while (reader.Read())
                            {


                                CoursesList.Add(new CourseDTO(
                                    reader.GetInt32(idIndex),
                                    reader.GetString(TitleIndex),

                                    reader.IsDBNull(DescriptionIndex)?null : reader.GetString(DescriptionIndex),
                                    reader.GetDecimal(FeesPerMonthIndex),
                                    reader.IsDBNull(LevelRequiredIDIndex)? null : reader.GetInt32(LevelRequiredIDIndex),
                                    reader.IsDBNull(ImageIndex)?  null : reader.GetString(ImageIndex)
                                    )
                                );


                            }





                            reader.Close();
                        }


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }




            return CoursesList;




        }

        public static async Task<List<CourseDTO>> GetCoursesOfTeacherXAsync(int? TeacherID)
        {
           

            List<CourseDTO> CoursesList = new List<CourseDTO>();
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_TeacherCourses_CourseHandler", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@TeacherID", TeacherID);




                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            int idIndex=reader.GetOrdinal("id");
                            int TitleIndex=reader.GetOrdinal("Title");
                            int DescriptionIndex=reader.GetOrdinal("Description");
                            int FeesPerMonthIndex=reader.GetOrdinal("FeesPerMonth");
                            int LevelRequiredIDIndex=reader.GetOrdinal("LevelRequiredID");
                            int ImageIndex=reader.GetOrdinal("Image");


                            while (await reader.ReadAsync())
                            {


                                CoursesList.Add(new CourseDTO(
                                    reader.GetInt32(idIndex),
                                    reader.GetString(TitleIndex),

                                    reader.IsDBNull(DescriptionIndex)?null : reader.GetString(DescriptionIndex),
                                    reader.GetDecimal(FeesPerMonthIndex),
                                    reader.IsDBNull(LevelRequiredIDIndex)? null : reader.GetInt32(LevelRequiredIDIndex),
                                    reader.IsDBNull(ImageIndex)?  null : reader.GetString(ImageIndex)
                                    )
                                );


                            }



                            reader.Close();
                        }


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }




            return CoursesList;




        }








        public static List<CourseHandlerDTO> GetTeachersOfCourse(int? CourseID)
        {



            List<CourseHandlerDTO> TeacherList = new List<CourseHandlerDTO>();

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_CourseTeachers_CourseHandler", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@CourseID", CourseID);




                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {


                            int TeacherIDIndex=reader.GetOrdinal("TeacherID");
                            int TeacherNameIndex=reader.GetOrdinal("TeacherName");
                            int PermissionIndex=reader.GetOrdinal("Permission");


                            while (reader.Read()) {

                                TeacherList.Add(new CourseHandlerDTO(
                                    reader.GetInt32(TeacherIDIndex),
                                    reader.GetString(TeacherNameIndex),
                                    CourseID,
                                    reader.GetString(PermissionIndex)

                                    ));


                            }


                           
                            reader.Close();
                        }


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }




            return TeacherList;





        }


        public static async Task<List<CourseHandlerDTO>> GetTeachersOfCourseAsync(int? CourseID)
        {



            List<CourseHandlerDTO> TeacherList = new List<CourseHandlerDTO>();

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_CourseTeachers_CourseHandler", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@CourseID", CourseID);




                        await connection.OpenAsync();
                        using (SqlDataReader reader =await  command.ExecuteReaderAsync())
                        {

                            int TeacherIDIndex=reader.GetOrdinal("TeacherID");
                            int TeacherNameIndex=reader.GetOrdinal("TeacherName");
                            int PermissionIndex=reader.GetOrdinal("Permission");


                            while (await reader.ReadAsync()) {

                                TeacherList.Add(new CourseHandlerDTO(
                                    reader.GetInt32(TeacherIDIndex),
                                    reader.GetString(TeacherNameIndex),
                                    CourseID,
                                    reader.GetString(PermissionIndex)

                                    ));


                            }


                           
                            reader.Close();
                        }


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }




            return TeacherList;





        }








    }
}
