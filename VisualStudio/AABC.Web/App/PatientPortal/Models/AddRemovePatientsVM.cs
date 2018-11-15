using System.Collections.Generic;

namespace AABC.Web.App.PatientPortal.Models
{
    public class AddRemovePatientsVM
    {

        public int LoginID { get; set; }
        public List<CurrentPatientListItem> CurrentPatients { get; set; }
        public List<PatientListItem> PatientsList { get; set; }

    }


    public class PatientListItem
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class CurrentPatientListItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

}