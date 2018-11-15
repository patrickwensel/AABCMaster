using AABC.Mobile.SharedEntities.Entities;
using AABC.Mobile.SharedEntities.Messages;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Entities
{
	[Table("CaseValidations")]
	public class CaseValidation : ValidatedSession
	{
		public CaseValidation()
		{
		}

		public CaseValidation(ValidatedSession validatedSession)
		{
			ServerValidatedSessionID = validatedSession.ServerValidatedSessionID;
			CaseID = validatedSession.CaseID;
			DateOfService = validatedSession.DateOfService;
			StartTime = validatedSession.StartTime;
			Duration = validatedSession.Duration;
			ServiceID = validatedSession.ServiceID;
			LocationID = validatedSession.LocationID;
			LocationDescription = validatedSession.LocationDescription;
			ServiceDescription = validatedSession.ServiceDescription;
		}


		[PrimaryKey]
		[AutoIncrement]
		public int CaseValidationID { get; set; }


		[Ignore]
		public DateTime StartDateTime
		{
			get
			{
				return DateOfService + StartTime;
			}
		}

		[Ignore]
		public Case Case {  get; set; }
	}
}
