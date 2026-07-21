using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using Microsoft.Data.SqlClient;


namespace OnlineSchoolDataAccess
{
    public class clsSqlPersonData
    {



        public static int? AddNewPerson(string nationalNumber, string firstName, string secondName, string thirdName
            , string forthName, string image, DateTime? dateOfBirth, string email, string phoneNumber
            , string address)
        {

            int? Insertedid=null;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {



                    using (SqlCommand command = new SqlCommand("SP_AddNew_Person", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@NationalNumber", nationalNumber);
                        command.Parameters.AddWithValue("@FirstName", firstName.ToString());
                        command.Parameters.AddWithValue("@SecondName", secondName.ToString());
                        if (thirdName != null)
                            command.Parameters.AddWithValue("@ThirdName", thirdName.ToString());
                        else
                            command.Parameters.AddWithValue("@ThirdName", DBNull.Value);

                        if (forthName != null)
                            command.Parameters.AddWithValue("@ForthName", forthName.ToString());
                        else
                            command.Parameters.AddWithValue("@ForthName", DBNull.Value);

                        if (image != null)
                            command.Parameters.AddWithValue("@Image", image.ToString());
                        else
                            command.Parameters.AddWithValue("@Image", DBNull.Value);


                        command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);

                        if (email != null)
                            command.Parameters.AddWithValue("@Email", email.ToString());
                        else
                            command.Parameters.AddWithValue("@Email", DBNull.Value);


                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber.ToString());

                        if(address != null)
                            command.Parameters.AddWithValue("@Addresse", address.ToString());
                        else
                            command.Parameters.AddWithValue("@Addresse", DBNull.Value);




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
            catch (Exception ex) {

                clsGlobal.Logger.Log(ex.Message,DateTime.Now);
                

            
            }

            return Insertedid;


        }

        public static bool UpDatePerson(int? id,string nationalNumber, string firstName, string secondName, string thirdName
           , string forthName, string image, DateTime? dateOfBirth, string email, string phoneNumber
           , string address)
        {

            bool UpDated = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Update_Person", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@NationalNumber", nationalNumber);
                        command.Parameters.AddWithValue("@FirstName", firstName.ToString());
                        command.Parameters.AddWithValue("@SecondName", secondName.ToString());



                        if (thirdName != null)
                            command.Parameters.AddWithValue("@ThirdName", thirdName.ToString());
                        else
                            command.Parameters.AddWithValue("@ThirdName", DBNull.Value);

                        if (forthName != null)
                            command.Parameters.AddWithValue("@ForthName", forthName.ToString());
                        else
                            command.Parameters.AddWithValue("@ForthName", DBNull.Value);

                        if (image != null)
                            command.Parameters.AddWithValue("@Image", image.ToString());
                        else
                            command.Parameters.AddWithValue("@Image", DBNull.Value);





                      
                        command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);



                        if (email != null)
                            command.Parameters.AddWithValue("@Email", email.ToString());
                        else
                            command.Parameters.AddWithValue("@Email", DBNull.Value);


                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber.ToString());

                        if (address != null)
                            command.Parameters.AddWithValue("@Addresse", address.ToString());
                        else
                            command.Parameters.AddWithValue("@Addresse", DBNull.Value);




                        command.Parameters.AddWithValue("@PersonID", id);

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


            return   UpDated;

        }


        public static bool FindPerson(int? id, ref string nationalNumber, ref string firstName, ref string secondName, ref string thirdName
         , ref string forthName, ref string image, ref DateTime? dateOfBirth, ref string email, ref string phoneNumber
         , ref string address)
        {
            bool IsFound = false;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_FindByID_Person", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;



                        command.Parameters.Add(new SqlParameter("@NationalNumber", SqlDbType.NVarChar, 10) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 15) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@SecondName", SqlDbType.NVarChar, 15) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@ThirdName", SqlDbType.NVarChar, 15) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@ForthName", SqlDbType.NVarChar, 15) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@Image", SqlDbType.NVarChar, 300) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 30) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@Addresse", SqlDbType.NVarChar, 300) { Direction = ParameterDirection.Output });

                        command.Parameters.AddWithValue("@PersonID", id);

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



                            nationalNumber = (string)command.Parameters["@NationalNumber"].Value;
                            firstName = (string)command.Parameters["@FirstName"].Value;
                            secondName = (string)command.Parameters["@SecondName"].Value;


                            if(command.Parameters["@ThirdName"].Value!=DBNull.Value)
                                thirdName = (string)command.Parameters["@ThirdName"].Value;

                            if (command.Parameters["@ForthName"].Value != DBNull.Value)
                                forthName = (string)command.Parameters["@ForthName"].Value;

                            if (command.Parameters["@Image"].Value != DBNull.Value)
                                image = (string)command.Parameters["@Image"].Value;

                            dateOfBirth = (DateTime)command.Parameters["@DateOfBirth"].Value;

                            if (command.Parameters["@Email"].Value != DBNull.Value)
                                email = (string)command.Parameters["@Email"].Value;

                            phoneNumber = (string)command.Parameters["@PhoneNumber"].Value;

                            if (command.Parameters["@Addresse"].Value != DBNull.Value)
                                address = (string)command.Parameters["@Addresse"].Value;



                        }

                        connection.Close();

                    }


                }
            }
            catch (Exception ex)
            {

                clsGlobal.Logger.Log(ex.Message , DateTime.Now);



            }


            return IsFound;

        }
     


        public static bool IsPersonExist(int? id)
        {
            bool IsFound = false;

            try
            {

                using (SqlConnection connection = new SqlConnection(DataAccessConfiguration._connectionString))
                {


                    using (SqlCommand command = new SqlCommand("SP_Person_Exist", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;



                        
                        command.Parameters.AddWithValue("@PersonID", id);

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

                clsGlobal.Logger.Log(ex.Message , DateTime.Now);



            }











            return IsFound;





        }




    }
}
