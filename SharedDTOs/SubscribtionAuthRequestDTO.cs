using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class SubscribtionAuthRequestDTO
    {
        


        //token to verify the connexion is safe
        public string token {  get; set; }
        public DateTime expireAt { get; set; }
        public UserDTO user { get; set; }
        public int courseID { get; set; }

       
    }
}
