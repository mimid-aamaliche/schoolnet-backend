using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SharedDTOs;


namespace OnlineSchoolDataAccess
{
    public class clsSqlSubscribtionsData
    {
        public static async Task<int?> AddNewSubscriptionRecordAsync(DateTime StartDate, DateTime? EndDate, int StudentID,
        int CourseID, bool? IsPaid, int? PaymentID)
        {

            int? Insertedid = null;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {



                    using (SqlCommand command = new SqlCommand("SP_AddNew_Subscription", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        //the values of our new record _____________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________

                        command.Parameters.AddWithValue("@StartDate", StartDate);
                        command.Parameters.AddWithValue("@StudentID", StudentID);

                        //this could be null
                        command.Parameters.AddWithValue("@EndDate", EndDate);

                        //this too
                        command.Parameters.AddWithValue("@IsPaid", IsPaid.Value);


                        //this could be null
                        if (PaymentID.HasValue)
                            command.Parameters.AddWithValue("@PaymentID", PaymentID.Value);
                        else
                            command.Parameters.AddWithValue("@PaymentID", DBNull.Value);



                        command.Parameters.AddWithValue("@CourseID", CourseID);




                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);




                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();




                        Insertedid = returnParameter.Value != DBNull.Value ? (int)returnParameter.Value : null;
                        connection.Close();

                    }


                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());

            }

            return Insertedid;


        }




        public static int? AddNewSubscriptionRecord(DateTime StartDate, DateTime? EndDate, int StudentID,
             int? CourseID, bool? IsPaid, int? PaymentID)
        {

            int? Insertedid = null;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {



                    using (SqlCommand command = new SqlCommand("SP_AddNew_Subscription", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        //the values of our new record _____________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________

                        command.Parameters.AddWithValue("@StartDate", StartDate);
                        command.Parameters.AddWithValue("@StudentID", StudentID);

                        //this could be null
                        if (EndDate.HasValue)
                            command.Parameters.AddWithValue("@EndDate", EndDate.Value);
                        else
                            command.Parameters.AddWithValue("@EndDate", DBNull.Value);

                        //this could be null
                        if (IsPaid.HasValue)
                            command.Parameters.AddWithValue("@IsPaid", IsPaid.Value);
                        else
                            command.Parameters.AddWithValue("@IsPaid", DBNull.Value);


                        //this could be null
                        if (PaymentID.HasValue)
                            command.Parameters.AddWithValue("@PaymentID", PaymentID.Value);
                        else
                            command.Parameters.AddWithValue("@PaymentID", DBNull.Value);



                        command.Parameters.AddWithValue("@CourseID", CourseID.Value);




                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);




                        connection.Open();
                        command.ExecuteNonQuery();




                        Insertedid = returnParameter.Value != DBNull.Value ? (int)returnParameter.Value : null;
                        connection.Close();

                    }


                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());

            }

            return Insertedid;


        }

        //remarque don t forget to use paging technique in the methods that return Lists




        //StudentHasAccess db done --------
        public static async Task<bool> StudentHasAccess(int? StudentID, int CourseID)
        {
            int studentHasAccess = -1;
            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {

                    using (SqlCommand command = new SqlCommand("SP_StudentHasAccessOnCourse_Subscription", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@StudentID", StudentID.Value);
                        command.Parameters.AddWithValue("@CourseID", CourseID);

                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);





                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();




                        studentHasAccess = (int)returnParameter.Value;



                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }


            if (studentHasAccess == -1)
            {
                //error login 


            }

            return studentHasAccess == 1;

        }






        //List of Courses Of Student X
        public static async Task<List<CourseDTO>> GetCoursesOfStudentXAsync(int? StudentID)
        {

            List<CourseDTO> CoursesList = new List<CourseDTO>();
            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {

                    using (SqlCommand command = new SqlCommand("SP_StudentEnrolledCourses_Subscription", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@StudentID", StudentID);

                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            //columns indeces !!!!!
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

                                    reader.IsDBNull(DescriptionIndex)? null : reader.GetString(DescriptionIndex),
                                    reader.GetDecimal(FeesPerMonthIndex),
                                    reader.IsDBNull(LevelRequiredIDIndex)? null : reader.GetInt32(LevelRequiredIDIndex),
                                    reader.IsDBNull(ImageIndex)?  null : reader.GetString(ImageIndex)
                                    )
                                );


                            }




                            reader.Close();




                        }


                        




                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }


            return CoursesList;

        }



        //Update Record db done

        public static async Task<int?> UpdateSubscriptionRecordAsync(int? id,
            bool? IsPaid, int? PaymentID)
        {

            int? rawseffected = null;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {



                    using (SqlCommand command = new SqlCommand("SP_UpdateSubscriptionRecord", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        //the new values of our record ____________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________

                        command.Parameters.AddWithValue("@id", id.Value);

                        if (PaymentID.HasValue)
                            command.Parameters.AddWithValue("@IsPaid", IsPaid.Value);
                        else
                            command.Parameters.AddWithValue("@IsPaid", DBNull.Value);

                        //this could be null
                        if (PaymentID.HasValue)
                            command.Parameters.AddWithValue("@PaymentID", PaymentID.Value);
                        else
                            command.Parameters.AddWithValue("@PaymentID", DBNull.Value);




                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);




                        connection.Open();
                        await command.ExecuteNonQueryAsync();




                        rawseffected = (int)returnParameter.Value;
                        connection.Close();

                    }


                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());


            }


            return rawseffected;


        }





        public static int? UpdateSubscriptionRecord(int? id,
           bool? IsPaid, int? PaymentID)
        {

            int? rawseffected = null;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {



                    using (SqlCommand command = new SqlCommand("SP_UpdateSubscriptionRecord", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        //the new values of our record ____________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________

                        command.Parameters.AddWithValue("@id", id.Value);

                        if (PaymentID.HasValue)
                            command.Parameters.AddWithValue("@IsPaid", IsPaid.Value);
                        else
                            command.Parameters.AddWithValue("@IsPaid", DBNull.Value);

                        //this could be null
                        if (PaymentID.HasValue)
                            command.Parameters.AddWithValue("@PaymentID", PaymentID.Value);
                        else
                            command.Parameters.AddWithValue("@PaymentID", DBNull.Value);




                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);




                        connection.Open();
                        command.ExecuteNonQuery();




                        rawseffected = (int)returnParameter.Value;
                        connection.Close();

                    }


                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());


            }


            return rawseffected;


        }
















        //not implimented yet 


        /* //Get History
            public static async Task<DataTable> GetStudentHistoryInCourseXAsync(int? StudentID, int? CourseID)
            {

                DataTable studentHistory = new DataTable();
                try
                {

                    using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                    {

                        using (SqlCommand command = new SqlCommand("SP_GetStudentHistoryInCourseX", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;


                            command.Parameters.AddWithValue("@StudentID", StudentID);
                            command.Parameters.AddWithValue("@CourseID", CourseID);

                            connection.Open();

                            Task<SqlDataReader> reader = command.ExecuteReaderAsync();




                            await reader;
                            if (reader.Result.HasRows)
                            {
                                studentHistory.Load(reader.Result);

                            }


                            reader.Result.Close();


                        }

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                }


                return studentHistory;

            }

    */









        /*
         //List of Students Of CourseX db done ----------
            public static async Task<DataTable> GetCourseXStudentsAsync(int? CourseID)
            {

                DataTable StudentList = new DataTable();
                try
                {

                    using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                    {

                        using (SqlCommand command = new SqlCommand("SP_GetStudentsOfCourseX", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;


                            command.Parameters.AddWithValue("@CourseID", CourseID);

                            connection.Open();

                            Task<SqlDataReader> reader = command.ExecuteReaderAsync();




                            await reader;
                            if (reader.Result.HasRows)
                            {
                                StudentList.Load(reader.Result);
                            }


                            reader.Result.Close();


                        }

                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                }


                return StudentList;

            }
    */




    }
}
