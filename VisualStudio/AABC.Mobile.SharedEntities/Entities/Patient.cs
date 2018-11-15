using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Entities
{
	public enum Gender
	{
		Unknown,
		Male,
		Female,
	}

	public class Patient
	{

		public int ID { get; set; }

		public string PatientFirstName { get; set; }
		public string PatientLastName { get; set; }

		public string PatientAddress1 { get; set; }
		public string PatientAddress2 { get; set; }
		public string PatientCity { get; set; }
		public string PatientState { get; set; }
		public string PatientZip { get; set; }

		public Gender Gender { get; set; }

		public string PatientGuardianFirstName { get; set; }
		public string PatientGuardianLastName { get; set; }
		public string PatientGuardianRelationship { get; set; }
		public string PatientGuardianPhone { get; set; }

		public string PatientGuardian2FirstName { get; set; }
		public string PatientGuardian2LastName { get; set; }
		public string PatientGuardian2Relationship { get; set; }
		public string PatientGuardian2Phone { get; set; }

	}
}
