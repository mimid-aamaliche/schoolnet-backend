using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Data.SqlClient;
using SharedDTOs;


namespace OnlineSchoolDataAccess
{
    public class clsSqlCourseData
    {
        public static int? AddNewCourse(string CourseTitle,string Description,decimal FeesPerMonth, string Image,int? LevelRequiredID)
        {

            int? Insertedid = null;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {



                    using (SqlCommand command = new SqlCommand("SP_AddNew_Course", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        //the values of our new record _____________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________

                        command.Parameters.AddWithValue("@Title", CourseTitle);
                        command.Parameters.AddWithValue("@FeesPerMonth", FeesPerMonth);

                        if (!string.IsNullOrEmpty(Description))
                            command.Parameters.AddWithValue("@Description", Description);
                        else
                            command.Parameters.AddWithValue("@Description", DBNull.Value);


                        if (LevelRequiredID.HasValue)
                            command.Parameters.AddWithValue("@LevelRequiredID", LevelRequiredID.Value);
                        else
                            command.Parameters.AddWithValue("@LevelRequiredID", DBNull.Value);


                        if (!string.IsNullOrEmpty(Image))
                            command.Parameters.AddWithValue("@Image", Image);
                        else
                            command.Parameters.AddWithValue("@Image", DBNull.Value);



                        SqlParameter outputIdParam = new SqlParameter("@InsertedID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputIdParam);




                        connection.Open();
                        command.ExecuteNonQuery();




                        Insertedid = (int)command.Parameters["@InsertedID"].Value;
                        connection.Close();

                    }


                }
            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);



            }

            return Insertedid;


        }
        public static bool UpDateCourse(int? CourseID,string CourseTitle, string Description, decimal FeesPerMonth, string Image, int? LevelRequiredID)
        {
            if (!CourseID.HasValue) { return false; }

            bool UpDated = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Update_Course", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //the new value of our record _____________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        command.Parameters.AddWithValue("@Title", CourseTitle);

                        if (!string.IsNullOrEmpty(Description))
                            command.Parameters.AddWithValue("@Description", Description);
                        else
                            command.Parameters.AddWithValue("@Description", DBNull.Value);

                        command.Parameters.AddWithValue("@FeesPerMonth", FeesPerMonth);

                        if (!string.IsNullOrEmpty(Image))
                            command.Parameters.AddWithValue("@Image", Image);
                        else
                            command.Parameters.AddWithValue("@Image", DBNull.Value);


                        if (LevelRequiredID.HasValue)
                        command.Parameters.AddWithValue("@LevelRequiredID", LevelRequiredID.Value);
                        else
                            command.Parameters.AddWithValue("@LevelRequiredID", DBNull.Value);
                        //_________________________________________________________









                        //the id of the record we wana change________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        command.Parameters.AddWithValue("@CourseID", CourseID.Value);
                        //_________________________________________________________




                        // the returne value 1 if the update is seccesfull 0 if not 
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________

                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);
                        //_________________________________________________________





                        connection.Open();
                        command.ExecuteNonQuery();
                        UpDated = (int)returnParameter.Value == 1;





                        connection.Close();

                    }


                }
            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);



            }


            return UpDated;

        }
        public static bool FindCourseByID(int? CourseID,ref string CourseTitle,ref string Description,ref decimal FeesPerMonth,ref string Image,ref int? LevelRequiredID)
        {
            bool IsFound = false;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByID_Course", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;



                        command.Parameters.Add(new SqlParameter("@Title", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@FeesPerMonth", SqlDbType.SmallMoney) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@Image", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@LevelRequiredID", SqlDbType.Int) { Direction = ParameterDirection.Output });
                       
                        command.Parameters.AddWithValue("@CourseID", CourseID);

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



                            CourseTitle = (string)command.Parameters["@Title"].Value;
                            Description = command.Parameters["@Description"].Value==DBNull.Value?"": (string)command.Parameters["@Description"].Value;
                            FeesPerMonth = (decimal)command.Parameters["@FeesPerMonth"].Value;
                            Image = command.Parameters["@Image"].Value == DBNull.Value ? "" : (string)command.Parameters["@Image"].Value;
                            LevelRequiredID= command.Parameters["@LevelRequiredID"].Value == DBNull.Value ? null : (int?)command.Parameters["@LevelRequiredID"].Value;

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
        public static List<CourseDTO> CoursesList()
        {
            List<CourseDTO> CourseList = new List<CourseDTO>();
            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_GetList_Course", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;



                       

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader()) {


                            //columns indeces !!!!!
                            int idIndex=reader.GetOrdinal("id");
                            int TitleIndex=reader.GetOrdinal("Title");
                            int DescriptionIndex=reader.GetOrdinal("Description");
                            int FeesPerMonthIndex=reader.GetOrdinal("FeesPerMonth");
                            int LevelRequiredIDIndex=reader.GetOrdinal("LevelRequiredID");
                            int ImageIndex=reader.GetOrdinal("Image");


                            while (reader.Read())
                            {


                                CourseList.Add(new CourseDTO(
                                    reader.GetInt32(idIndex),
                                    reader.GetString(TitleIndex),

                                    reader.IsDBNull(DescriptionIndex)? null : reader.GetString(DescriptionIndex),
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


            return CourseList;


        }


    }
}
