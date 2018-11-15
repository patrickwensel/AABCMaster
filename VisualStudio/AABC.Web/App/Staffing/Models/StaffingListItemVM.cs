using System;

namespace AABC.Web.App.Staffing.Models
{
    public class StaffingListItemVM
    {
        public int ID { get; set; }
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CaseManager { get; set; }
        public DateTime? DateWentToRestaff { get; set; }

        public string CommonName { get { return FirstName + " " + LastName; } }
    }
}