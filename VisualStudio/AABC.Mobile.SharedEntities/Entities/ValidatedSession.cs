using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Entities
{

	public enum CaseValidationState
	{
		None = 0,
		Valid,
		Active,
		CompletedAwaitingSendToServer,
		CompletedSentToServer,
		AbandonedAwaitingSendToServer,
		AbandonedSentToServer,
	}


	public class ValidatedSession
	{
		public int ServerValidatedSessionID { get; set; }

		public int CaseID { get; set; }

		public string UserName { get; set; }

		public DateTime DateOfService { get; set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan Duration { get; set; }

		public int ServiceID { get; set; }

		public int LocationID { get; set; }

		public string LocationDescription { get; set; }

		public string ServiceDescription { get; set; }

		public string SsgCaseIds { get; set; }

		public CaseValidationState State { get; set; }
	}
}
