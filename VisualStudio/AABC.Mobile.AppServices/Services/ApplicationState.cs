using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.SharedEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Services
{
	/// <summary>
	/// Class ApplicationState.
	/// </summary>
	/// <seealso cref="AABC.Mobile.AppServices.Interfaces.IApplicationState" />
	public class ApplicationState : IApplicationState
	{
		readonly INotificationService _notificationService;

		/// <summary>
		/// Gets or sets the current user.
		/// </summary>
		/// <value>The current user.</value>
		public User CurrentUser { get; set; }

		/// <summary>
		/// Gets or sets the selected case.
		/// </summary>
		/// <value>The selected case.</value>
		public Case SelectedCase { get; set; }

		/// <summary>
		/// Gets or sets the session in progress.
		/// </summary>
		/// <value>The session in progress.</value>
		public CaseValidation SessionInProgress { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationState"/> class.
		/// </summary>
		/// <param name="notificationService">The notification service.</param>
		public ApplicationState(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}


		/// <summary>
		/// Sets the session in progress.
		/// </summary>
		/// <param name="sessionInProgress">The session in progress.</param>
		public void SetSessionInProgress(CaseValidation sessionInProgress)
		{
			SessionInProgress = sessionInProgress;

			if (sessionInProgress == null)
			{
				_notificationService.RemoveNotification();
			}
			else
			{
				_notificationService.Notify("Applied ABC Provider", $"A session with {SessionInProgress.Case.Patient.PatientFirstName} {SessionInProgress.Case.Patient.PatientLastName.Substring(0, 1)} is in progress");
			}
		}
	}
}
