using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class AuthResponse
    {
        public DateTime ExpireAt { get; set; }
        public  UserDTO user { get; set; }



    }
}
