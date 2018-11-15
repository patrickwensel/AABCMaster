using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.SharedEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Interfaces
{
	public interface IApplicationState
	{
		User CurrentUser { get; set; }

		Case SelectedCase { get; set; }

		CaseValidation SessionInProgress { get; }

		void SetSessionInProgress(CaseValidation sessionInProgress);
	}
}
