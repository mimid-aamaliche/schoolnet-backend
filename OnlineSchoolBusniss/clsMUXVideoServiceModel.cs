using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    public class clsMUXVideoServiceModel
    {

        public string muxTokenID {  get; set; }
        public string muxTokenSecret { get; set; }

        public string coreOrigine { get; set; }
        public string apiBaseURL { get; set; }

        public string WebHookKey { get; set; }
    }
}
