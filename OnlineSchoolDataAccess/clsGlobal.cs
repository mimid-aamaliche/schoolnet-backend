using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolDataAccess
{
    internal class clsGlobal
    {


        //here we gonna stor all the function that we gonna use with logger 
        //write to regitre func/to eventLog func/to file/...



        public static clsLogger Logger = new clsLogger((string message, DateTime date) => Console.WriteLine(message + date.ToString()));



        







    }
}
