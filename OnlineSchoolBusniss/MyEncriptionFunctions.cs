using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    public class MyEncriptionFunctions
    {







        public static string hmachashing(string input, string Key, HMAC algorithme)
        {



            //get the key in bytes and give it to the algorithm we are using
            byte[] KeyInBytes = Encoding.UTF8.GetBytes(Key);
            algorithme.Key = KeyInBytes;

            //compute the hashing of our data
            byte[] dataHashed = algorithme.ComputeHash(Encoding.UTF8.GetBytes(input));


            return BitConverter.ToString(dataHashed).Replace("-", "").ToLower();
        }








    }
}
