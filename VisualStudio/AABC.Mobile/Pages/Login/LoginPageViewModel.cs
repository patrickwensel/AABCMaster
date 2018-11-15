using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using Plugin.Connectivity.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AABC.Mobile.AppServices.Exceptions;
using Prism.Ioc;
using AABC.Mobile.Helpers;
using Prism.Services;

namespace AABC.Mobile.Pages.Login
{
	[AddINotifyPropertyChangedInterface]
	class LoginPageViewModel : INavigatingAware
	{
		readonly INavigationService _navigationService;

		readonly IAccountService _accountService;

		readonly IApplicationState _applicationState;

		readonly IConnectivity _connectivity;

		readonly IOfflineServices _offlineServices;

		readonly ISecureAppStorage _secureAppStorage;

		readonly IDataUpdateService _dataUpdateService;

		readonly IDatabaseService _databaseService;

		readonly ISessionUpdateService _sessionUpdateService;

		readonly ISettingsService _settingsService;

		readonly IUpdateService _updateService;

		readonly IPageDialogService _pageDialogService;

		public ICommand Login { get; set; }

		public ICommand Update { get; set; }
		

		public string Username { get; set; }

		public string Password { get; set; }

		public string Message { get; set; }
		

		public bool UsernameEntered { get; set; }

		public bool PasswordEntered { get; set; }

		public bool Busy { get; set; }

		public bool UpdateAvailable { get; set; }

		public LoginPageViewModel(INavigationService navigationService, IAccountService accountService, IApplicationState applicationState, IConnectivity connectivity, IOfflineServices offlineServices, ISecureAppStorage secureAppStorage, IDataUpdateService dataUpdateService, IDatabaseService databaseService, ISessionUpdateService sessionUpdateService, ISettingsService settingsService, IUpdateService updateService, IPageDialogService pageDialogService)
		{
			_navigationService = navigationService;
			_accountService = accountService;
			_applicationState = applicationState;
			_connectivity = connectivity;
			_offlineServices = offlineServices;
			_secureAppStorage = secureAppStorage;
			_dataUpdateService = dataUpdateService;
			_databaseService = databaseService;
			_sessionUpdateService = sessionUpdateService;
			_settingsService = settingsService;
			_updateService = updateService;
			_pageDialogService = pageDialogService;

			Login = new DelegateCommand(() => OnLoginCommand().IgnoreResult());
			Update = new DelegateCommand(() => OnUpdateCommand().IgnoreResult());
		}

		public async void OnNavigatingTo(NavigationParameters parameters)
		{
			// make sure the settings are initialized
			await _settingsService.Initialize();

			// get the current session
			var currentlyActiveValidation = await _databaseService.GetActiveCaseValidation();

			if (currentlyActiveValidation != null)
			{
				// we have an active session, but should we abandon it?
				var abandonedSession = await StartupHelper.CheckForAbandonedSession(currentlyActiveValidation);
				if (abandonedSession != null)
				{
					// we have abandoned the session, so inform the user
					await _pageDialogService.DisplayAlertAsync("Session", $"Your currently active session with {abandonedSession.Case.Patient.PatientFirstName} {abandonedSession.Case.Patient.PatientLastName.Substring(0, 1)} has been abandoned", "OK");

					// and set this to null as we no longer have a session
					currentlyActiveValidation = null;
				}
			}

			if (currentlyActiveValidation == null)
			{
				// we don't have an active session, so do we need to do an update?
				if (await StartupHelper.IsAnUpdateRequired())
				{
					// we require an update, so redirect to the correct page
					await _navigationService.NavigateAsync("file:///UpdatePage");
				}
			}

			// if we have a stored username, use this
			var username = _secureAppStorage.Username;

			if (username != null)
			{
				Username = username;
			}
		}

		async Task OnLoginCommand()
		{
			Message = String.Empty;

			if (UsernameEntered && PasswordEntered)
			{
				bool attemptOfflineLogin = false;

				// are we online
				if (_connectivity.IsConnected)
				{
					attemptOfflineLogin = await OnlineLogin();
				}
				else
				{
					attemptOfflineLogin = true;
				}


				if (attemptOfflineLogin)
				{
					await OfflineLogin();
				}
			}
		}

		async Task OfflineLogin()
		{
			// we're offline, so use the stored credentials
			if (_offlineServices.ValidateCredentials(Username, Password))
			{
				_accountService.SetCachedCredentials(Username, Password);

				// we're logged in 
				await SetCurrentlyActiveSession();

				// start the SessionUpdateService 
				_sessionUpdateService.Start();

				// redirect to the start page
				await _navigationService.NavigateAsync("file:///TabbedNavigationPage");
			}
			else
			{
				// we're not logged in 
				Message = "The user name or password is incorrect";
			}
		}

		/// <summary>
		/// Called when logging in online.
		/// </summary>
		/// <returns>A bool indicating whether we should attempt offline login or not</returns>
		async Task<bool> OnlineLogin()
		{
			try
			{
				Busy = true;

				// await _accountService.Login("18774", "yakov123");
				await _accountService.Login(Username, Password);

				if (_applicationState.CurrentUser.Username == Username)
				{
					// we're logged in 
					_offlineServices.SaveCredentials(Username, Password);
					_secureAppStorage.Username = Username;

					var initialData = await _dataUpdateService.GetInitialData();

					await _databaseService.WriteCurrentCases(initialData.Cases, Username);

					await _databaseService.WriteValidatedSessions(initialData.ValidatedSessions, Username);

					await _settingsService.UpdateSettings(initialData.Settings);

					await SetCurrentlyActiveSession();

					// start the SessionUpdateService 
					_sessionUpdateService.Start();


					// and redirect to the start page
					await _navigationService.NavigateAsync("file:///TabbedNavigationPage?SelectedPage=Cases");
				}
				else
				{
					// we're not logged in 
					Message = "There was a problem logging in";
				}
			}
			catch (CommunicationException communicationException)
			{
				Message = communicationException.ErrorMessage;

				// we got a valid response back, so don't continue
				return false;
			}
			catch (Exception ex)
			{
#if DEBUG
				// if in debug, show the message then login offline
				var pageDialogService = ((Prism.Unity.PrismApplication)(App.Current)).Container.Resolve<Prism.Services.IPageDialogService>();
				await pageDialogService.DisplayAlertAsync("Debug", "Unable to login : " + ex.GetType().Name + " " + ex.Message, "Continue");
#endif
				return true;
			}
			finally
			{
				Busy = false;
			}

			return false;
		}

		async Task OnUpdateCommand()
		{
			await _updateService.RedirectToUpdate();
		}

		async Task SetCurrentlyActiveSession()
		{
			// get the active session if we have one
			var currentlyActiveValidation = await _databaseService.GetActiveCaseValidation();
			if (currentlyActiveValidation != null)
			{
				_applicationState.SelectedCase = currentlyActiveValidation.Case;
				_applicationState.SetSessionInProgress(currentlyActiveValidation);
			}
			else
			{
				_applicationState.SelectedCase = null;
				_applicationState.SetSessionInProgress(null);
			}
		}
	}
}
