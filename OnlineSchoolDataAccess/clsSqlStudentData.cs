using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace OnlineSchoolDataAccess
{
    public class clsSqlStudentData
    {


        public static int? AddNewStudent(int? PersonID,string UserName, string Password,int? LevelID)
        {
            int? StudentID = null;
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_AddNew_Student", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //here Person id must have value
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        



                        command.Parameters.AddWithValue("@UserName", UserName.ToString());
                        command.Parameters.AddWithValue("@PassWord", Password.ToString());

                        if(LevelID.HasValue)
                        command.Parameters.AddWithValue("@CurrentLevelID", LevelID.ToString());
                        else
                            command.Parameters.AddWithValue("@CurrentLevelID",DBNull.Value);


                        SqlParameter InsertedID = new SqlParameter("@InsertedID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        command.Parameters.Add(InsertedID);
                        connection.Open();
                        command.ExecuteNonQuery();
                        StudentID = (int)command.Parameters["@InsertedID"].Value;


                        connection.Close();
                    }




                }




            }
            catch (Exception ex) {

                clsGlobal.Logger.Log(ex.Message, DateTime.Now);

            }

            return StudentID;
        }

        public static bool UpDateStudent(int? StudentID,int? PersonID, string UserName, string Password, int? LevelID)
        {
            bool UpDated = false;
            
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_UpDate_Student", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //here Person id must have value
                        command.Parameters.AddWithValue("@PersonID", PersonID);



                        command.Parameters.AddWithValue("@StudentID", StudentID);

                        command.Parameters.AddWithValue("@UserName", UserName.ToString());
                        command.Parameters.AddWithValue("@PassWord", Password.ToString());

                        if (LevelID.HasValue)
                            command.Parameters.AddWithValue("@CurrentLevelID", LevelID.ToString());
                        else
                            command.Parameters.AddWithValue("@CurrentLevelID", DBNull.Value);


                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        command.Parameters.Add(returnParameter);



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

        public static bool FindStudentById(int? StudentID,ref int? PersonID,ref string UserName,ref int? LevelID)
        {
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByID_Student", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@StudentID", StudentID);

                        command.Parameters.Add(new SqlParameter("@UserName", SqlDbType.NVarChar,50)
                        {
                            Direction = ParameterDirection.Output,
                        });
                       
                        command.Parameters.Add(new SqlParameter("@PersonID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@CurrentLevelID", SqlDbType.Int)
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
                            PersonID =(int?)command.Parameters["@PersonID"].Value;
                            LevelID = command.Parameters["@CurrentLevelID"].Value == DBNull.Value ? null : (int?)command.Parameters["@CurrentLevelID"].Value;
                            UserName =(string)command.Parameters["@UserName"].Value;

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


        public static DataTable GetStudent_PersonListInFormat()
        {
            DataTable StudentList= new DataTable();
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_GetListInFormat_Student", connection))
                    {

                        //this procedure goona get the student list with all the personal info 
                        //bisacly return the join of student levels person 




                        

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {


                            if (reader.HasRows)
                            {
                                StudentList.Load(reader);


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




            return StudentList;

        }
        public static DataTable GetStudent_PersonList()
        {
            DataTable StudentList = new DataTable();
            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_GetNormalList_Student", connection))
                    {

                        //this procedure goona get the student list with all the personal info 
                        //bisacly return the join of student levels person 






                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {


                            if (reader.HasRows)
                            {
                                StudentList.Load(reader);


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




            return StudentList;

        }

        public static bool FindStudentByUserNameAnsPassword(string UserName, string Password, ref int? PersonID, ref int? StudentID, ref int? LevelID)
        {
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByUserNameAndPassword_Student", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@UserName", UserName.ToString());
                        command.Parameters.AddWithValue("@PassWord", Password.ToString());


                       
                        command.Parameters.Add(new SqlParameter("@PersonID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@CurrentLevelID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        });
                        command.Parameters.Add(new SqlParameter("@StudentID", SqlDbType.Int)
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
                            PersonID = (int)command.Parameters["@PersonID"].Value;
                            LevelID = command.Parameters["@CurrentLevelID"].Value == DBNull.Value ? null : (int?)command.Parameters["@CurrentLevelID"].Value;
                            UserName = (string)command.Parameters["@UserName"].Value;
                            StudentID=(int)command.Parameters["@StudentID"].Value;







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

        public static bool StudentExistByID( int? StudentID)
        {
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Student_Exist", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@StudentID", StudentID);






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

        public static bool StudentExistByUserNameAndPassword(string UserName, string Password)
        {
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Student_Exist_ByUserNameAndPass", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@UserName", UserName.ToString());
                        command.Parameters.AddWithValue("@PassWord", Password.ToString());



                        


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

        public static bool StudentExistByUserName(string UserName)
        {
            bool IsFound = false;

            try
            {


                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Student_Exist_ByUserName", connection))
                    {

                        command.CommandType = System.Data.CommandType.StoredProcedure;


                        command.Parameters.AddWithValue("@UserName", UserName.ToString());






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

    }
}
