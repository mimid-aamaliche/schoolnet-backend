using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolDataAccess
{
    public class DataAccessConfiguration
    {

        internal static string _connectionString;

        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;
        }


    }
}
