using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.AppServices.Services;
using AABC.Mobile.SharedEntities.Interfaces;
using Unity;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Plugin.ExternalMaps;
using Plugin.ExternalMaps.Abstractions;
using Plugin.Messaging;
using Plugin.SecureStorage;
using Plugin.SecureStorage.Abstractions;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Xamarin.Forms;
using Prism.Ioc;
using Prism;
using System.Threading.Tasks;
using Prism.Navigation;
using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.Helpers;

namespace AABC.Mobile
{
	public partial class App : PrismApplication
	{
		public App(IPlatformInitializer initializer = null) : base(initializer)
		{
			// for initialization, put calls in OnInitialized() as this gets called before items here
		}

		protected override void OnStart()
		{
			base.OnStart();

			ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
			{
				try
				{
					// get the correct view model for this view. The view class must be called the same as the view, including the namespace, but with ViewModel appended.
					var viewName = viewType.FullName;
					var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
					var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}ViewModel, {1}", viewName, viewAssemblyName);
					return Type.GetType(viewModelName);
				}
				catch(Exception ex)
				{
					// report the error then just pass it to the next handler
					System.Diagnostics.Debug.WriteLine($"Exception {ex.GetType().Name} thrown. {ex.Message} {ex.StackTrace}");
					throw;
				}
			});
		}

		public INavigationService GetNavigationService()
		{
			return NavigationService;
		}

		protected async override void OnInitialized()
		{
			InitializeComponent();

			await NavigationService.NavigateAsync("file:///LoginPage");
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			var resourceProviderService = new ResourceProviderService(typeof(App).GetTypeInfo().Assembly, "AABC.Mobile.Resources.Resources.");
			containerRegistry.RegisterInstance<IResourceProviderService>(resourceProviderService);

			containerRegistry.RegisterInstance<ISecureStorage>(CrossSecureStorage.Current);
			containerRegistry.RegisterInstance<IConnectivity>(CrossConnectivity.Current);

			containerRegistry.RegisterSingleton<IPropertiesService, EmbeddedResourcePropertiesService>();
			containerRegistry.RegisterSingleton<ISQLite, SQLiteService>();

			containerRegistry.RegisterSingleton<IApplicationState, ApplicationState>();
			containerRegistry.RegisterSingleton<IAccountService, AccountService>();
			containerRegistry.RegisterSingleton<IDatabaseService, DatabaseService>();
			containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();

			containerRegistry.RegisterSingleton<ISecureAppStorage, SecureAppStorage>();
			containerRegistry.RegisterSingleton<IOfflineServices, OfflineServices>();

			containerRegistry.RegisterSingleton<IDataUpdateService, DataUpdateService>();
			containerRegistry.RegisterSingleton<ISessionUpdateService, SessionUpdateService>();
			containerRegistry.RegisterSingleton<IUpdateService, UpdateService>();

			containerRegistry.RegisterInstance<IPhoneCallTask>(CrossMessaging.Current.PhoneDialer);
			containerRegistry.RegisterInstance<IEmailTask>(CrossMessaging.Current.EmailMessenger);

			containerRegistry.RegisterInstance<IExternalMaps>(CrossExternalMaps.Current);

			RegisterAllPagesInThisAssembly(containerRegistry);
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override async void OnResume()
		{
			// Handle when your app resumes

			var applicationState = Container.Resolve<IApplicationState>();
			var databaseService = Container.Resolve<IDatabaseService>();

			// update the settings if we're logged in
			if (!String.IsNullOrEmpty(applicationState.CurrentUser?.Username))
			{
				await UpdateSettings();
			}

			// get the current session
			var currentlyActiveValidation = await databaseService.GetActiveCaseValidation();

			if (currentlyActiveValidation != null)
			{
				// we have an active session, but should we abandon it?
				var abandonedSession = await StartupHelper.CheckForAbandonedSession(currentlyActiveValidation);
				if (abandonedSession != null)
				{
					// we have abandoned the session, so inform the user
					var pageDialogService = Container.Resolve<Prism.Services.IPageDialogService>();
					await pageDialogService.DisplayAlertAsync("Session", $"Your currently active session with {abandonedSession.Case.Patient.PatientFirstName} {abandonedSession.Case.Patient.PatientLastName} has been abandoned", "OK");
				}

				// redirect if we've abandoned the session and we're logged in 
				if (abandonedSession != null)
				{
					// we've abandoned the session, so we can check for an update
					if (await StartupHelper.IsAnUpdateRequired())
					{
						// we require an update, so redirect to the correct page
						await NavigationService.NavigateAsync("file:///UpdatePage");
					}
					else if (!String.IsNullOrEmpty(applicationState.CurrentUser.Username))
					{
						// we've abandoned the session and we're logged in so we can need to go to the TabbedNavigationPage
						await NavigationService.NavigateAsync("file:///TabbedNavigationPage", animated: false);
					}
				}
			}
			else
			{
				// we don't have an active session, so do we need to do an update?
				if (await StartupHelper.IsAnUpdateRequired())
				{
					// we require an update, so redirect to the correct page
					await NavigationService.NavigateAsync("file:///UpdatePage");
				}
			}
		}

		/// <summary>
		/// Updates the settings.
		/// </summary>
		/// <returns>Task.</returns>
		async Task UpdateSettings()
		{
			try
			{
				IConnectivity _connectivity = Container.Resolve<IConnectivity>();
				if (_connectivity.IsConnected)
				{
					// update the current settings
					IDataUpdateService dataUpdateService = Container.Resolve<IDataUpdateService>();
					ISettingsService settingsService = Container.Resolve<ISettingsService>();

					var initialData = await dataUpdateService.GetInitialData();
					await settingsService.UpdateSettings(initialData.Settings);
				}
			}
			catch
			{
				// ignore any errors
			}
		}



		/// <summary>
		/// Registers all pages in this assembly.
		/// </summary>
		void RegisterAllPagesInThisAssembly(IContainerRegistry containerRegistry)
		{
			var assembly = typeof(App).GetTypeInfo().Assembly;

			// get all the pages from the assembly
			var pageTypes = assembly.DefinedTypes
								.Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(Page)))
								.Select(x => x.AsType());

			// register all the pages in this assembly
			foreach (var pageType in pageTypes)
			{
				containerRegistry.RegisterForNavigation(pageType, pageType.Name);
			}
		}

		/// <summary>
		/// Determines whether the android back button is allowed.
		/// </summary>
		/// <returns><c>true</c> if the back button is allowed; otherwise, <c>false</c>.</returns>
		public bool IsBackAllowed()
		{
			var mainPage = App.Current.MainPage as TabbedPage;

			if (mainPage == null)
			{
				return false;
			}
			else
			{
				var navigationPage = mainPage.CurrentPage as NavigationPage;
				if (navigationPage == null)
				{
					return false;
				}
				else
				{
					return (navigationPage.StackDepth > 1);
				}
			}
		}
	}
}
