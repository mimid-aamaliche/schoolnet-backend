using OnlineSchoolDataAccess;
using SharedDTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    public class clsSubscribtion
    {
        enum enMode { enAddNew=1,enUpdateMode=2}
        private enMode _CurrentMode;
        public int? ID { get; set; }
        public DateTime _StartDate {  get; set; }
        public DateTime? _EndDate { get; set; }
        public int _StudentID { get; set; }

        public int _CourseID { get; set; }
        public bool? _IsPaid { get; set; }
        public int? _PaymentID { get; set; }



        public clsSubscribtion() {

            
            _EndDate = null;
            _IsPaid = null;
            _PaymentID = null;

            _CurrentMode =enMode.enAddNew;
        }
        public clsSubscribtion(DateTime StartDate, DateTime EndDate, int StudentID,
        int CourseID, bool IsPaid, int? PaymentID)
        {
            _StartDate = StartDate;
            _EndDate = EndDate;
            _StudentID = StudentID;
            _CourseID = CourseID;
            _IsPaid = IsPaid;
            _PaymentID = PaymentID;

            _CurrentMode = enMode.enUpdateMode;
        }


        private bool _AddNewSubscribtion()
        {

            this.ID=clsSqlSubscribtionsData.AddNewSubscriptionRecord(this._StartDate, this._EndDate,this._StudentID
                ,this._CourseID,this._IsPaid,this._PaymentID);

            return  ID!=null;

        }
        private async Task<bool> _AddNewSubscribtionAsync()
        {

            this.ID=await clsSqlSubscribtionsData.AddNewSubscriptionRecordAsync(this._StartDate, this._EndDate,this._StudentID
                ,this._CourseID,this._IsPaid,this._PaymentID);

            return  ID!=null;

        }




        private bool _UpdateSubscribtion()
        {

            int? RawEffected= clsSqlSubscribtionsData.UpdateSubscriptionRecord(ID, _IsPaid, _PaymentID); 

            return RawEffected!=null&& RawEffected!=0;
        }

        

        public static async Task<bool> StudentHasAccessAsync(int? StudentID, int CourseID)
        {
         

            Task<bool> resultTask = clsSqlSubscribtionsData.StudentHasAccess(StudentID, CourseID);
           
            return await resultTask;
           


        }







        public static  async Task<List<CourseDTO>> GetStudentCoursesAsync(int? StudentID)
        {

            List<CourseDTO> StudentList  =  await clsSqlSubscribtionsData.GetCoursesOfStudentXAsync(StudentID);


            return StudentList;

        }



        public bool Save()
        {
            switch (this._CurrentMode)
            {
                case enMode.enAddNew:
                    if (_AddNewSubscribtion())
                    {
                        this._CurrentMode = enMode.enUpdateMode;
                        return true;
                    }
                    break;
                case enMode.enUpdateMode:
                    return _UpdateSubscribtion();
                    

            }

            return false;
        }


        public async Task<bool> SaveAsync()
        {
            switch (this._CurrentMode)
            {
                case enMode.enAddNew:
                    if (await _AddNewSubscribtionAsync())
                    {
                        this._CurrentMode = enMode.enUpdateMode;
                        return true;
                    }
                    break;
                case enMode.enUpdateMode:
                    return _UpdateSubscribtion();
                    //!!!!!!!!!!!!!!!!! change this to an async update !!!!!!!!!!!!!!!!!!!!!!!

            }

            return false;
        }



    }
}
