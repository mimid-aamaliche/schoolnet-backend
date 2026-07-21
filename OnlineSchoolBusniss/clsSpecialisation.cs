using OnlineSchoolDataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    public class clsSpecialisation
    {
        enum enMode { enAddNewMode = 1, enUpdateMode = 2 }
        enMode _CurrentMode = enMode.enAddNewMode;


        public int? SpecialisationID { get; set; }
        public string SpecialisationName { get; set; }

        public string Description { get; set; }



        public clsSpecialisation()
        {
            SpecialisationID = null;
            SpecialisationName = "";
            Description = "";

            _CurrentMode = enMode.enAddNewMode;

        }
        private clsSpecialisation(int ? specialisationID, string specialisationName,string description)
        {
            SpecialisationID = specialisationID;
            SpecialisationName = specialisationName;
            Description = description;
            _CurrentMode= enMode.enUpdateMode;
        }


        private bool _AddNewSpecialisation()
        {

            this.SpecialisationID=clsSqlSpecialisationData.AddNewSpecialisation(this.SpecialisationName,this.Description);
            return this.SpecialisationID != null;


        }

        private bool _UpdateSpecialisation() {



            return clsSqlSpecialisationData.UpdateSpecialisation(SpecialisationID, SpecialisationName, Description);
        
        }

        public static clsSpecialisation Find(int? Specialisationid)
        {
            string Name = "";
            string description = "";
            if (clsSqlSpecialisationData.FindSpecialisationByID(Specialisationid,ref Name,ref description))
            {

                return new clsSpecialisation(Specialisationid, Name, description);
            }
            else
            {
                return null;
            }




        }

        public static clsSpecialisation Find(string Name)
        {
            int? Specialisationid = null;
            string description = "";
            if (clsSqlSpecialisationData.FindSpecialisationByName(Name, ref Specialisationid,ref description))
            {

                return new clsSpecialisation(Specialisationid, Name, description);
            }
            else
            {
                return null;
            }




        }

        public static DataTable GetSpecialisationList()
        {
            return clsSqlSpecialisationData.GetSpecialisationList();
        }


        public bool Save()
        {
            switch (_CurrentMode)
            {
                case enMode.enAddNewMode:
                    if (_AddNewSpecialisation())
                    {
                        this._CurrentMode = enMode.enUpdateMode;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.enUpdateMode:
                    return _UpdateSpecialisation();



            }


            return false;
        }



    }
}
