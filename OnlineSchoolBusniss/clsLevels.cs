using OnlineSchoolDataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    public class clsLevels
    {
        enum enMode { enAddNewMode = 1, enUpdateMode = 2 }
        enMode _CurrentMode = enMode.enAddNewMode;


        public int? LevelID { get; set; }
        public string LevelName { get; set; }

        public string Description { get; set; }



        public clsLevels()
        {
            LevelID = null;
            LevelName = "";
            Description = "";

            _CurrentMode = enMode.enAddNewMode;

        }
        private clsLevels(int? Levelid, string levelName, string description)
        {
            LevelID = Levelid;
            LevelName = levelName;
            Description = description;
            _CurrentMode = enMode.enUpdateMode;
        }


        private bool _AddNewLevel()
        {

            this.LevelID = clsSqlLevelsData.AddNewLevel(LevelName, Description);
            return this.LevelID != null;


        }

        private bool _UpdateLevel()
        {


         return clsSqlLevelsData.UpdateLevel(LevelID, LevelName, Description);

        }

        public static clsLevels Find(int? levelid)
        {
            string Name = "";
            string description = "";
            if (clsSqlLevelsData.FindLevelByID(levelid, ref Name, ref description))
            {

                return new clsLevels(levelid, Name, description);
            }
            else
            {
                return null;
            }




        }

        public static clsLevels Find(string Name)
        {
            int? levelid = null;
            string description = "";
            if (clsSqlLevelsData.FindLevelByName(Name, ref levelid, ref description))
            {

                return new clsLevels(levelid, Name, description);
            }
            else
            {
                return null;
            }




        }

        public static DataTable GetLevelsList()
        {
            return clsSqlLevelsData.GetLevelList();
        }


        public bool Save()
        {
            switch (_CurrentMode)
            {
                case enMode.enAddNewMode:
                    if (_AddNewLevel())
                    {
                        this._CurrentMode = enMode.enUpdateMode;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.enUpdateMode:
                    return _UpdateLevel();



            }


            return false;
        }








    }
}
