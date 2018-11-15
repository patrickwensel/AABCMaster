using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.PatientPortal.Models
{
    public class ExistingLoginListItem
    {

        public int ID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }

        public string PatientsConcat {
            get
            {
                string[] patients = Patients.OrderBy(x => x.FirstName).Select(x => x.FirstName + " " + x.LastName).ToArray();
                return string.Join(", ", patients);
            }
        }

        public List<ExistingLoginPatientListItem> Patients { get; set; }


    }
}