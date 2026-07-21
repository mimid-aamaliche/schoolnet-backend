using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;


namespace OnlineSchoolDataAccess
{
    public class clsSqlLevelsData
    {

        public static int? AddNewLevel(string LevelName, string Description)
        {
            int? LevelID = null;
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_AddNew_Level", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //the the values of the new record in the specialisation table 
                        command.Parameters.AddWithValue("@LevelName", LevelName);

                        if (Description != "")
                            command.Parameters.AddWithValue("@Description", Description);
                        else
                            command.Parameters.AddWithValue("@Description", DBNull.Value);

                        SqlParameter InsertedID = new SqlParameter("@InsertedID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        command.Parameters.Add(InsertedID);
                        connection.Open();
                        command.ExecuteNonQuery();
                        LevelID = (int)command.Parameters["@InsertedID"].Value;


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }

            return LevelID;
        }

        public static bool UpdateLevel(int? ID, string LevelName, string Description)
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


                    using (SqlCommand command = new SqlCommand("SP_Update_Level", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //the the new values of the record in the specialisation table 

                        command.Parameters.AddWithValue("@RecordID", ID);

                        command.Parameters.AddWithValue("@LevelName", LevelName);

                        if (Description != "")
                            command.Parameters.AddWithValue("@Description", Description);
                        else
                            command.Parameters.AddWithValue("@Description", DBNull.Value);

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




        public static bool FindLevelByID(int? ID, ref string LevelName, ref string Description)
        {
            if (ID == null) { return false; }
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByID_Level", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@RecordID", ID);

                        command.Parameters.Add(new SqlParameter("@LevelName", SqlDbType.NVarChar, 300)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 500)
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
                            LevelName = (string)command.Parameters["@LevelName"].Value;
                            Description = command.Parameters["@Description"].Value == DBNull.Value ? "" : (string)command.Parameters["@Description"].Value;


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



        public static bool FindLevelByName(string LevelName, ref int? ID, ref string Description)
        {
            if (LevelName == "") { return false; }
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByName_Level", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@LevelName", LevelName);

                        command.Parameters.Add(new SqlParameter("@RecordID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 500)
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

                            Description = command.Parameters["@Description"].Value == DBNull.Value ? "" : (string)command.Parameters["@Description"].Value;
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

        public static DataTable GetLevelList()
        {
            DataTable SpecialisationList = new DataTable();
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_GetNormalList_Level", connection))
                    {






                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {


                            if (reader.HasRows)
                            {
                                SpecialisationList.Load(reader);


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




            return SpecialisationList;



        }




    }
}
