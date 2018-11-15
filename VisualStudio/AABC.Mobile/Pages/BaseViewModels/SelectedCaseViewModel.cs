using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.SharedEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.Pages.BaseViewModels
{
	/// <summary>
	/// Class SelectedCaseViewModel.
	/// </summary>
	/// <remarks>
	///		This class exposes the currently selected case
	/// </remarks>
	public class SelectedCaseViewModel
	{
		readonly IApplicationState _applicationState;

		public Case SelectedCase
		{
			get
			{
				return _applicationState.SelectedCase;
			}
		}

		public CaseValidation SessionInProgress
		{
			get
			{
				return _applicationState.SessionInProgress;
			}
		}


		public SelectedCaseViewModel(IApplicationState applicationState)
		{
			_applicationState = applicationState;
		}
	}
}
