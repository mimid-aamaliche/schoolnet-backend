using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SharedDTOs;
namespace OnlineSchoolDataAccess
{
    public class clsSqlCourseContentData
    {


        public static int? AddNewCourseContent(string ContentTitle, string Description, string ContentType, string ContentPath,
            int? CourseID, string Notes, DateTime DateOfRealease, string PlayBackID, string AssetID, string UploadID, string Status)
        {

            int? Insertedid = null;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {



                    using (SqlCommand command = new SqlCommand("SP_AddNew_CourseContent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        //the values of our new record _____________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________

                        command.Parameters.AddWithValue("@Title", ContentTitle);
                        command.Parameters.AddWithValue("@ContentType", ContentType);
                        command.Parameters.AddWithValue("@DateOfRealease", DateOfRealease);

                        if (Description != "")
                            command.Parameters.AddWithValue("@Description", Description);
                        else
                            command.Parameters.AddWithValue("@Description", DBNull.Value);
                        if (Notes != "")
                            command.Parameters.AddWithValue("@Notes", Notes);
                        else
                            command.Parameters.AddWithValue("@Notes", DBNull.Value);


                        if (CourseID.HasValue)
                            command.Parameters.AddWithValue("@CourseID", CourseID.Value);
                        else
                            return null;

                        if (!string.IsNullOrEmpty(ContentPath))
                            command.Parameters.AddWithValue("@ContentPath", ContentPath);
                        else
                            command.Parameters.AddWithValue("@ContentPath", DBNull.Value);




                        //Mux Service fields -*****************************************************

                        if (PlayBackID != "")
                            command.Parameters.AddWithValue("@PlayBackID", PlayBackID);
                        else
                            command.Parameters.AddWithValue("@PlayBackID", DBNull.Value);


                        if (AssetID != "")
                            command.Parameters.AddWithValue("@AssetID", AssetID);
                        else
                            command.Parameters.AddWithValue("@AssetID", DBNull.Value);


                        if (UploadID != "")
                            command.Parameters.AddWithValue("@UploadID", UploadID);
                        else
                            command.Parameters.AddWithValue("@UploadID", DBNull.Value);


                        if (Status != "")
                            command.Parameters.AddWithValue("@Status", Status);
                        else
                            command.Parameters.AddWithValue("@Status", DBNull.Value);

                        //-*************************************************************************











                        SqlParameter outputIdParam = new SqlParameter("@InsertedID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputIdParam);




                        connection.Open();
                        command.ExecuteNonQuery();




                        Insertedid = command.Parameters["@InsertedID"].Value!=DBNull.Value?  (int)command.Parameters["@InsertedID"].Value : null;

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

        

        public static async Task<int?> AddNewCourseContentAsync(string ContentTitle, string Description, string ContentType, string ContentPath,
            int? CourseID, string Notes, DateTime DateOfRealease, string PlayBackID, string AssetID, string UploadID, string Status)
        {

            int? Insertedid = null;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {



                    using (SqlCommand command = new SqlCommand("SP_AddNew_CourseContent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        //the values of our new record _____________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________

                        command.Parameters.AddWithValue("@Title", ContentTitle);
                        command.Parameters.AddWithValue("@ContentType", ContentType);
                        command.Parameters.AddWithValue("@DateOfRealease", DateOfRealease);

                        if (Description != "")
                            command.Parameters.AddWithValue("@Description", Description);
                        else
                            command.Parameters.AddWithValue("@Description", DBNull.Value);
                        if (Notes != "")
                            command.Parameters.AddWithValue("@Notes", Notes);
                        else
                            command.Parameters.AddWithValue("@Notes", DBNull.Value);


                        if (CourseID.HasValue)
                            command.Parameters.AddWithValue("@CourseID", CourseID.Value);
                        else
                            return null;

                        if (!string.IsNullOrEmpty(ContentPath))
                            command.Parameters.AddWithValue("@ContentPath", ContentPath);
                        else
                            command.Parameters.AddWithValue("@ContentPath", DBNull.Value);




                        //Mux Service fields -*****************************************************

                        if (PlayBackID != "")
                            command.Parameters.AddWithValue("@PlayBackID", PlayBackID);
                        else
                            command.Parameters.AddWithValue("@PlayBackID", DBNull.Value);


                        if (AssetID != "")
                            command.Parameters.AddWithValue("@AssetID", AssetID);
                        else
                            command.Parameters.AddWithValue("@AssetID", DBNull.Value);


                        if (UploadID != "")
                            command.Parameters.AddWithValue("@UploadID", UploadID);
                        else
                            command.Parameters.AddWithValue("@UploadID", DBNull.Value);


                        if (Status != "")
                            command.Parameters.AddWithValue("@Status", Status);
                        else
                            command.Parameters.AddWithValue("@Status", DBNull.Value);

                        //-*************************************************************************










                        SqlParameter outputIdParam = new SqlParameter("@InsertedID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputIdParam);




                        connection.Open();
                        await command.ExecuteNonQueryAsync();




                        Insertedid = command.Parameters["@InsertedID"].Value!=DBNull.Value?  (int)command.Parameters["@InsertedID"].Value : null;

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







        public static bool UpDateCourseContent(int? CourseContentID, string ContentTitle, string Description,
                                     string ContentType, string ContentPath,
                                 int? CourseID, string Notes, DateTime DateOfRealease
                                 , string PlayBackID, string AssetID, string UploadID, string Status)
        {
            if (!CourseContentID.HasValue) { return false; }

            bool UpDated = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Update_CourseContent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //the new value of our record _____________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        command.Parameters.AddWithValue("@Title", ContentTitle);
                        command.Parameters.AddWithValue("@DateOfRealease", DateOfRealease);

                        if (Description != "")
                            command.Parameters.AddWithValue("@Description", Description);
                        else
                            command.Parameters.AddWithValue("@Description", DBNull.Value);

                        if (Notes != "")
                            command.Parameters.AddWithValue("@Notes", Notes);
                        else
                            command.Parameters.AddWithValue("@Notes", DBNull.Value);



                        //Mux Service fields -*****************************************************

                        if (PlayBackID != "")
                            command.Parameters.AddWithValue("@PlayBackID", PlayBackID);
                        else
                            command.Parameters.AddWithValue("@PlayBackID", DBNull.Value);


                        if (AssetID != "")
                            command.Parameters.AddWithValue("@AssetID", AssetID);
                        else
                            command.Parameters.AddWithValue("@AssetID", DBNull.Value);


                        if (Status != "")
                            command.Parameters.AddWithValue("@Status", Status);
                        else
                            command.Parameters.AddWithValue("@Status", DBNull.Value);

                        //-*************************************************************************





                        command.Parameters.AddWithValue("@ContentType", ContentType);

                        if (ContentPath != "")
                            command.Parameters.AddWithValue("@ContentPath", ContentPath);
                        else
                            command.Parameters.AddWithValue("@ContentPath", DBNull.Value);


                        if (CourseID.HasValue)
                            command.Parameters.AddWithValue("@CourseID", CourseID.Value);
                        else
                            command.Parameters.AddWithValue("@CourseID", DBNull.Value);
                        //_________________________________________________________









                        //the id of the record we wana change________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        command.Parameters.AddWithValue("@ContentID", CourseContentID.Value);
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

                        //if the record does not exist it will return -1
                        //if an exception /an error was thrown , it will return the error number 



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




        public static async Task<bool> UpDateCourseContentAsync(int? CourseContentID, string ContentTitle, string Description,
                                     string ContentType, string ContentPath,
                                 int? CourseID, string Notes, DateTime DateOfRealease
                                 , string PlayBackID, string AssetID, string UploadID, string Status)
        {
            if (!CourseContentID.HasValue) { return false; }

            bool UpDated = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Update_CourseContent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        //the new value of our record _____________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        command.Parameters.AddWithValue("@Title", ContentTitle);
                        command.Parameters.AddWithValue("@DateOfRealease", DateOfRealease);

                        if (Description != "")
                            command.Parameters.AddWithValue("@Description", Description);
                        else
                            command.Parameters.AddWithValue("@Description", DBNull.Value);

                        if (Notes != "")
                            command.Parameters.AddWithValue("@Notes", Notes);
                        else
                            command.Parameters.AddWithValue("@Notes", DBNull.Value);



                        //Mux Service fields -*****************************************************

                        if (PlayBackID != "")
                            command.Parameters.AddWithValue("@PlayBackID", PlayBackID);
                        else
                            command.Parameters.AddWithValue("@PlayBackID", DBNull.Value);


                        if (AssetID != "")
                            command.Parameters.AddWithValue("@AssetID", AssetID);
                        else
                            command.Parameters.AddWithValue("@AssetID", DBNull.Value);


                        if (Status != "")
                            command.Parameters.AddWithValue("@Status", Status);
                        else
                            command.Parameters.AddWithValue("@Status", DBNull.Value);

                        //-*************************************************************************





                        command.Parameters.AddWithValue("@ContentType", ContentType);

                        if (ContentPath != "")
                            command.Parameters.AddWithValue("@ContentPath", ContentPath);
                        else
                            command.Parameters.AddWithValue("@ContentPath", DBNull.Value);


                        if (CourseID.HasValue)
                            command.Parameters.AddWithValue("@CourseID", CourseID.Value);
                        else
                            command.Parameters.AddWithValue("@CourseID", DBNull.Value);
                        //_________________________________________________________









                        //the id of the record we wana change________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        //_________________________________________________________
                        command.Parameters.AddWithValue("@ContentID", CourseContentID.Value);
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





                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        UpDated = (int)returnParameter.Value == 1;

                        //if the record does not exist it will return -1
                        //if an exception /an error was thrown , it will return the error number 



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















        public static bool FindCourseContentByID(int? CourseContentID, ref string ContentTitle, ref string Description,
                                     ref string ContentType, ref string ContentPath,
                                ref int? CourseID, ref string Notes, ref DateTime DateOfRealease, ref string PlayBackID, ref string AssetID, ref string UploadID, ref string Status)
        {


            bool IsFound = false;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByID_CourseContent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;



                        command.Parameters.Add(new SqlParameter("@Title", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@DateOfRealease", SqlDbType.DateTime) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@ContentType", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@CourseID", SqlDbType.Int) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@ContentPath", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@PlayBackID", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@AssetID", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@UploadID", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@Status", SqlDbType.NVarChar, 30) { Direction = ParameterDirection.Output });




                        command.Parameters.AddWithValue("@ContentID", CourseContentID);

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



                            ContentTitle = (string)command.Parameters["@Title"].Value;
                            Description = command.Parameters["@Description"].Value == DBNull.Value ? "" : (string)command.Parameters["@Description"].Value;
                            DateOfRealease = (DateTime)command.Parameters["@DateOfRealease"].Value;
                            ContentType = command.Parameters["@ContentType"].Value == DBNull.Value ? "" : (string)command.Parameters["@ContentType"].Value;
                            Notes = command.Parameters["@Notes"].Value == DBNull.Value ? "" : (string)command.Parameters["@Notes"].Value;
                            ContentPath = command.Parameters["@ContentPath"].Value == DBNull.Value ? "" : (string)command.Parameters["@ContentPath"].Value;
                            CourseID = command.Parameters["@CourseID"].Value == DBNull.Value ? null : (int?)command.Parameters["@CourseID"].Value;

                            PlayBackID = command.Parameters["@PlayBackID"].Value == DBNull.Value ? "" : (string)command.Parameters["@PlayBackID"].Value;
                            AssetID = command.Parameters["@AssetID"].Value == DBNull.Value ? "" : (string)command.Parameters["@AssetID"].Value;
                            UploadID = command.Parameters["@UploadID"].Value == DBNull.Value ? "" : (string)command.Parameters["@UploadID"].Value;
                            Status = command.Parameters["@Status"].Value == DBNull.Value ? "" : (string)command.Parameters["@Status"].Value;



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


        











        public static List<ContentDTO> CoursesXContentList(int ?CourseID)
        {
            if (!CourseID.HasValue) { return null; }


            List<ContentDTO> List = new List<ContentDTO>();
            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_GetContentOfCourse_CourseContent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@CourseID", CourseID);



                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            int IdIndex=reader.GetOrdinal("id");
                            int ContentTypeIndex=reader.GetOrdinal("ContentType");
                            int ContentTitleIndex=reader.GetOrdinal("Title");
                            int DescriptionIndex=reader.GetOrdinal("Description");
                            int DateOfRealeseIndex=reader.GetOrdinal("DateOfRealease");



                            //load content into the list
                            while (reader.Read())
                            {
                                

                                List.Add(new ContentDTO(
                                    reader.GetInt32(IdIndex),
                                    reader.GetString(ContentTypeIndex),
                                    reader.GetString(ContentTitleIndex),
                                    reader.IsDBNull(DescriptionIndex) ? "" : reader.GetString(DescriptionIndex),
                                    reader.GetDateTime(DateOfRealeseIndex)                                    
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


            return List;


        }

        public static async Task<List<ContentDTO>> CoursesXContentListAsync(int ?CourseID)
        {
            if (!CourseID.HasValue) { return null; }


            List<ContentDTO> List = new List<ContentDTO>();
            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_GetContentOfCourse_CourseContent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@CourseID", CourseID);



                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            int IdIndex=reader.GetOrdinal("id");
                            int ContentTypeIndex=reader.GetOrdinal("ContentType");
                            int ContentTitleIndex=reader.GetOrdinal("Title");
                            int DescriptionIndex=reader.GetOrdinal("Description");
                            int DateOfRealeseIndex=reader.GetOrdinal("DateOfRealease");



                            //load content into the list
                            while (await reader.ReadAsync())
                            {
                                

                                List.Add(new ContentDTO(
                                    reader.GetInt32(IdIndex),
                                    reader.GetString(ContentTypeIndex),
                                    reader.GetString(ContentTitleIndex),
                                    await reader.IsDBNullAsync(DescriptionIndex) ? "" : reader.GetString(DescriptionIndex),
                                    reader.GetDateTime(DateOfRealeseIndex)                                    
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


            return List;


        }



































        // ─────────────────────────────────────────────────────────────────────────
        // Find content record by MUX UploadID  (used in webhook handlers)
        // ─────────────────────────────────────────────────────────────────────────

        public static bool FindByUploadID(string uploadID, ref int? ContentID, ref string ContentTitle,
            ref string Description, ref string ContentType, ref string ContentPath,
            ref int? CourseID, ref string Notes, ref DateTime DateOfRealease,
            ref string PlayBackID, ref string AssetID, ref string Status)
        {
            if (string.IsNullOrEmpty(uploadID)) return false;

            bool IsFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                using (SqlCommand command = new SqlCommand("SP_FindByUploadID_CourseContent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@UploadID", uploadID);

                    command.Parameters.Add(new SqlParameter("@ContentID",      SqlDbType.Int)          { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@Title",          SqlDbType.NVarChar, 300) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@Description",    SqlDbType.NVarChar, -1)  { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@DateOfRealease", SqlDbType.DateTime)      { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ContentType",    SqlDbType.NVarChar, 50)  { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@CourseID",       SqlDbType.Int)          { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@Notes",          SqlDbType.NVarChar, -1)  { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ContentPath",    SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@PlayBackID",     SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@AssetID",        SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@Status",         SqlDbType.NVarChar, 50)  { Direction = ParameterDirection.Output });

                    SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        { Direction = ParameterDirection.ReturnValue };
                    command.Parameters.Add(returnParameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                    IsFound = (int)returnParameter.Value == 1;

                    if (IsFound)
                    {
                        ContentID    = command.Parameters["@ContentID"].Value == DBNull.Value ? null : (int?)command.Parameters["@ContentID"].Value;
                        ContentTitle = (string)command.Parameters["@Title"].Value;
                        Description  = command.Parameters["@Description"].Value  == DBNull.Value ? "" : (string)command.Parameters["@Description"].Value;
                        DateOfRealease = (DateTime)command.Parameters["@DateOfRealease"].Value;
                        ContentType  = command.Parameters["@ContentType"].Value  == DBNull.Value ? "" : (string)command.Parameters["@ContentType"].Value;
                        Notes        = command.Parameters["@Notes"].Value        == DBNull.Value ? "" : (string)command.Parameters["@Notes"].Value;
                        ContentPath  = command.Parameters["@ContentPath"].Value  == DBNull.Value ? "" : (string)command.Parameters["@ContentPath"].Value;
                        CourseID     = command.Parameters["@CourseID"].Value     == DBNull.Value ? null : (int?)command.Parameters["@CourseID"].Value;
                        PlayBackID   = command.Parameters["@PlayBackID"].Value   == DBNull.Value ? "" : (string)command.Parameters["@PlayBackID"].Value;
                        AssetID      = command.Parameters["@AssetID"].Value      == DBNull.Value ? "" : (string)command.Parameters["@AssetID"].Value;
                        Status       = command.Parameters["@Status"].Value       == DBNull.Value ? "" : (string)command.Parameters["@Status"].Value;
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                clsGlobal.Logger.Log(ex.Message, DateTime.Now);
            }

            return IsFound;
        }


    }
}
