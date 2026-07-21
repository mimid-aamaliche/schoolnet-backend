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
    public class clsSqlTeacherData
    {

        public static int? AddNewTeacher(int? PersonID,string TeacherStripeID, string UserName, string Password, int? SpecialisationID,string CvPath, string Notes)
        {
            int? TeacherID = null;
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_AddNew_Teacher", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //here Person id must have value
                        command.Parameters.AddWithValue("@PersonID", PersonID);


                        if (!string.IsNullOrEmpty(TeacherStripeID))
                            command.Parameters.AddWithValue("@TeacherStripeID", TeacherStripeID);
                        else
                            command.Parameters.AddWithValue("@TeacherStripeID", DBNull.Value);




                        command.Parameters.AddWithValue("@UserName", UserName.ToString());
                        command.Parameters.AddWithValue("@PassWord", Password.ToString());
                        if(!string.IsNullOrEmpty(CvPath))
                            command.Parameters.AddWithValue("@CV", CvPath);
                        else
                            command.Parameters.AddWithValue("@CV", DBNull.Value);

                        if(!string.IsNullOrEmpty(Notes))
                             command.Parameters.AddWithValue("@Notes", Notes.ToString());
                        else
                        {
                            command.Parameters.AddWithValue("@Notes", DBNull.Value);

                        }

                        if (SpecialisationID.HasValue)
                            command.Parameters.AddWithValue("@SpecialisationID", SpecialisationID.ToString());
                        else
                            command.Parameters.AddWithValue("@SpecialisationID", DBNull.Value);


                        SqlParameter InsertedID = new SqlParameter("@InsertedID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        command.Parameters.Add(InsertedID);
                        connection.Open();
                        command.ExecuteNonQuery();
                        TeacherID = (int)command.Parameters["@InsertedID"].Value;


                        connection.Close();
                    }




                }




            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }

            return TeacherID;
        }

        public static bool UpdateTeacher(int? TeacherID,  int? PersonID,string TeacherStripeID, string UserName, string Password, int? SpecialisationID, string CvPath, string Notes)
        {
            if (TeacherID == null) { return false; }
            bool IsUpdated = false;
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Update_Teacher", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //here Person id must have value
                        command.Parameters.AddWithValue("@TeacherID", TeacherID);


                        if (!string.IsNullOrEmpty(TeacherStripeID))
                            command.Parameters.AddWithValue("@TeacherStripeID", TeacherStripeID);
                        else
                            command.Parameters.AddWithValue("@TeacherStripeID", DBNull.Value);


                        command.Parameters.AddWithValue("@PersonID", PersonID);

                        command.Parameters.AddWithValue("@UserName", UserName.ToString());
                        command.Parameters.AddWithValue("@PassWord", Password.ToString());

                        if (CvPath != "")
                            command.Parameters.AddWithValue("@CV", CvPath);
                        else
                            command.Parameters.AddWithValue("@CV", DBNull.Value);


                        if (!string.IsNullOrEmpty(Notes))
                            command.Parameters.AddWithValue("@Notes", Notes.ToString());
                        else
                        {
                            command.Parameters.AddWithValue("@Notes", DBNull.Value);

                        }




                        if (SpecialisationID.HasValue)
                            command.Parameters.AddWithValue("@SpecialisationID", SpecialisationID.ToString());
                        else
                            command.Parameters.AddWithValue("@SpecialisationID", DBNull.Value);


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

        public static bool FindTeacherByID(int? TeacherID,ref string TeacherStripeID , ref int? PersonID, ref string UserName, ref string Password, ref int? SpecialisationID, ref string CvPath, ref string Notes)
        {
            if (TeacherID == null) { return false; }
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByID_Teacher", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //Input parametres      =======================================================================+
                        command.Parameters.AddWithValue("@TeacherID", TeacherID.Value);



                        //OutPut parametres     =======================================================================
                        command.Parameters.Add(new SqlParameter("@TeacherStripeID", SqlDbType.NVarChar, 100)
                        {
                            Direction = ParameterDirection.Output,
                        });

                        command.Parameters.Add(new SqlParameter("@UserName", SqlDbType.NVarChar, 50)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@PassWord", SqlDbType.NVarChar, 500)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@PersonID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@SpecialisationID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@CV", SqlDbType.NVarChar, 300)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 500)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);




                        //Start the connection and execute the query
                        connection.Open();
                        command.ExecuteNonQuery();

                        //note : the procedure "SP_FindByID_Teacher" returns a 1 if the user is found and 0 if not found
                        IsFound = (int)returnParameter.Value == 1;

                        if (IsFound)
                        {

                            if (command.Parameters["@TeacherStripeID"].Value != DBNull.Value)
                                TeacherStripeID = (string)command.Parameters["@TeacherStripeID"].Value;

                            Password = (string)command.Parameters["@PassWord"].Value;
                            PersonID = (int?)command.Parameters["@PersonID"].Value;
                            SpecialisationID = command.Parameters["@SpecialisationID"].Value == DBNull.Value ? null : (int?)command.Parameters["@SpecialisationID"].Value;
                            UserName = (string)command.Parameters["@UserName"].Value;


                            if (command.Parameters["@CV"].Value != DBNull.Value)
                                CvPath = (string)command.Parameters["@CV"].Value;

                            if(command.Parameters["@Notes"].Value != DBNull.Value)
                                Notes=(string)command.Parameters["@Notes"].Value;








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

        public static bool FindTeacherByUserNameAndPassword(string UserName,string Password,ref string TeacherStripeID,ref int? TeacherID, ref int? PersonID,  ref int? SpecialisationID, ref string CvPath, ref string Notes)
        {
            
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByUserNameAndPassword_Teacher", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //Input parametres      =======================================================================+
                        command.Parameters.AddWithValue("@UserName", UserName);
                        command.Parameters.AddWithValue("@PassWord", Password);


                        //OutPut parametres     =======================================================================
                        command.Parameters.Add(new SqlParameter("@TeacherStripeID", SqlDbType.NVarChar, 100)
                        {
                            Direction = ParameterDirection.Output,
                        });

                        command.Parameters.Add(new SqlParameter("@TeacherID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        
                        command.Parameters.Add(new SqlParameter("@PersonID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@SpecialisationID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@CV", SqlDbType.NVarChar, 300)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@Notes", SqlDbType.NVarChar, 500)
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
                            if (command.Parameters["@TeacherStripeID"].Value != DBNull.Value)
                                TeacherStripeID = (string)command.Parameters["@TeacherStripeID"].Value;

                            TeacherID = (int?)command.Parameters["@TeacherID"].Value;
                            PersonID = (int?)command.Parameters["@PersonID"].Value;
                            SpecialisationID = command.Parameters["@SpecialisationID"].Value == DBNull.Value ? null : (int?)command.Parameters["@SpecialisationID"].Value;
                            UserName = (string)command.Parameters["@UserName"].Value;

                            if (command.Parameters["@CV"].Value != DBNull.Value)
                                CvPath = (string)command.Parameters["@CV"].Value;

                            if (command.Parameters["@Notes"].Value != DBNull.Value)
                                Notes = (string)command.Parameters["@Notes"].Value;








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


        public static bool TeacherExistByUserNameAndPassword(string UserName, string Password)
        {

            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Exist_ByUserNameAndPassword_Teacher", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@UserName", UserName);
                        command.Parameters.AddWithValue("@PassWord", Password);

                        

                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);



                        connection.Open();
                        command.ExecuteNonQuery();
                        IsFound = (int)returnParameter.Value == 1;

                       



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

        public static bool TeacherExistByUserName(string UserName)
        {

            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Exist_ByUserName_Teacher", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@UserName", UserName);



                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);



                        connection.Open();
                        command.ExecuteNonQuery();
                        IsFound = (int)returnParameter.Value == 1;





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






        public static bool TeacherExistByID(int? TeacherID)
        {
            if(TeacherID == null) return false;

            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Teacher_Exist", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@TeacherID", TeacherID);



                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);



                        connection.Open();
                        command.ExecuteNonQuery();
                        IsFound = (int)returnParameter.Value == 1;





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

        //must change from datatable to dtos
        public static DataTable GetTeacher_PersonListInFormat()
        {
            DataTable TeacherList = new DataTable();
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_GetListInFormat_Teacher", connection))
                    {

                        //this procedure goona get the Teacher list with all the personal info 
                        //bisacly return the join of Teacher Specialisation person 






                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {


                            if (reader.HasRows)
                            {
                                TeacherList.Load(reader);


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
        public static DataTable GetTeacher_PersonList()
        {
            DataTable TeacherList = new DataTable();
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_GetNormalList_Teacher", connection))
                    {

                        //this procedure goona get the student list with the id of the rest of info






                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {


                            if (reader.HasRows)
                            {
                                TeacherList.Load(reader);


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

