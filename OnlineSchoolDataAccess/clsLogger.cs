using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolDataAccess
{   

  
    internal class clsLogger
    {
        public delegate void LogAction (string message, DateTime Dateofmessage);
        private LogAction _logaction;




        public clsLogger(LogAction logaction)
        {
            _logaction = logaction;
        }



        public void Log(string message,DateTime date)
        {
            if (_logaction != null)
            {
                _logaction?.Invoke(message,date);
            }
        }
       




    }
}
