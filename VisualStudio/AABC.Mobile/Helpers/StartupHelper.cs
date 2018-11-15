using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Interfaces;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Plugin.Connectivity.Abstractions;
using AABC.Mobile.SharedEntities.Entities;

namespace AABC.Mobile.Helpers
{
	class StartupHelper
	{
		public static async Task<CaseValidation> CheckForAbandonedSession(CaseValidation currentlyActiveValidation)
		{
			var container = ((PrismApplication)(App.Current)).Container;

			var databaseService = container.Resolve<IDatabaseService>();
			var settingsService = container.Resolve<ISettingsService>();

			if (settingsService.Setting<bool>("ActiveSession.Abandon.Enabled"))
			{
				// have we gone over Abandon.TimeoutMinutes
				var currentDuration = DateTime.Now - currentlyActiveValidation.StartDateTime;
				int timeoutMinutes = settingsService.Setting<int>("ActiveSession.Abandon.TimeoutMinutes");
				if (currentDuration > TimeSpan.FromMinutes(timeoutMinutes))
				{
					// abandon this session
					currentlyActiveValidation.Duration = currentDuration;
					currentlyActiveValidation.State = CaseValidationState.AbandonedAwaitingSendToServer;

					// we don't have a current session anymore
					var applicationState = container.Resolve<IApplicationState>();
					applicationState.SetSessionInProgress(null);

					await databaseService.WriteCaseValidation(currentlyActiveValidation);
					return currentlyActiveValidation;
				}
			}

			return null;
		}
	
		/// <summary>
		/// Determines whether an update is required.
		/// </summary>
		/// <returns>True if an update is required</returns>
		public static async Task<bool> IsAnUpdateRequired()
		{
			var container = ((PrismApplication)(App.Current)).Container;

			var connectivity = container.Resolve<IConnectivity>();
			
			try
			{
				// are we connected?
				if (connectivity.IsConnected)
				{
					var updateService = container.Resolve<IUpdateService>();
					var currentVersionResponse = await updateService.UpdateRequired();

					// return whether an update is required
					return currentVersionResponse.UpdateRequired;
				}
			}
			catch
			{
				// ignore any errors
			}

			// no update is required
			return false;
		}
		
	}
}
