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
    public class clsSqlSpecialisationData
    {
        public static int? AddNewSpecialisation(string SpecialisationName,string Description)
        {
            int? SpecialisationID = null;
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_AddNew_Specialisation", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //the the values of the new record in the specialisation table 
                        command.Parameters.AddWithValue("@SpecialisationName", SpecialisationName);

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
                        SpecialisationID = (int)command.Parameters["@InsertedID"].Value;


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }

            return SpecialisationID;
        }

        public static bool UpdateSpecialisation(int?ID, string SpecialisationName, string Description)
        {
            if (!ID.HasValue)
            {
                return false;
            }

           bool  IsUpdated = false;
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Update_Specialisation", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //the the new values of the record in the specialisation table 

                        command.Parameters.AddWithValue("@RecordID", ID);

                        command.Parameters.AddWithValue("@SpecialisationName", SpecialisationName);

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
                        IsUpdated = (int)command.Parameters["@Updated"].Value==1;


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



        public static bool FindSpecialisationByID(int? ID,ref string SpecialisationName,ref string Description)
        {
            if (ID == null) { return false; }
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByID_Specialisation", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@RecordID", ID);

                        command.Parameters.Add(new SqlParameter("@SpecialisationName", SqlDbType.NVarChar, 300)
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
                            SpecialisationName = (string)command.Parameters["@SpecialisationName"].Value;
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


        public static bool FindSpecialisationByName(string SpecialisationName , ref int? ID , ref string Description)
        {
            if (SpecialisationName == "") { return false; }
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByName_Specialisation", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@SpecialisationName", SpecialisationName);

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


        public static DataTable GetSpecialisationList()
        {
            DataTable SpecialisationList = new DataTable();
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_GetNormalList_Specialisation", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;





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
