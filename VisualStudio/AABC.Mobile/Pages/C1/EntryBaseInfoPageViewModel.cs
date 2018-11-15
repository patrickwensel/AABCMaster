using AABC.Mobile.AppServices.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using PropertyChanged;
using AABC.Mobile.Pages.BaseViewModels;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.SharedEntities.Entities;
using AABC.Mobile.AppServices.Entities;
using System.Globalization;
using AABC.Mobile.SharedEntities.Messages;
using AABC.Mobile.Entities;
using AABC.Mobile.Interfaces;

namespace AABC.Mobile.Pages.C1
{
	[AddINotifyPropertyChangedInterface]
	class EntryBaseInfoPageViewModel : INavigatingAware
	{
		readonly INavigationService _navigationService;

		readonly IDataUpdateService _dataUpdateService;

		readonly IDatabaseService _databaseService;

		bool _initialized;
		int _pageNumber;
		IWizardController _wizardController;

		public EntryBaseInfoPageModel EntryBaseInfoPageModel { get; set; }

		public List<Service> Services { get; set; }

		public ICommand ContinueCommand { get; set; }

		public ICommand CancelCommand { get; set; }

		public ICommand DateSelectedCommand { get; set; }

		public ICommand ServiceChangedCommand { get; set; }

		public ICommand LocationChangedCommand { get; set; }

		public DateTime MinDateOfService { get; set; }

		public bool Busy { get; set; }

		public bool DisplayServiceNotEntered { get; set; }

		public bool DisplayLocationNotEntered { get; set; }

		public string ErrorMessage { get; set; }

		public bool DisplayErrorMessage { get; set; }

		public bool CanContinue { get; set; } = true;

		public EntryBaseInfoPageViewModel(INavigationService navigationService, IApplicationState applicationState, IDataUpdateService dataUpdateService, IDatabaseService databaseService)
		{
			_navigationService = navigationService;
			_dataUpdateService = dataUpdateService;
			_databaseService = databaseService;

			ContinueCommand = new DelegateCommand(() => OnContinueCommand().IgnoreResult()).ObservesCanExecute(() => CanContinue);
			CancelCommand = new DelegateCommand(() => OnCancelCommand().IgnoreResult());
			DateSelectedCommand = new DelegateCommand(() => OnDateSelectedCommand().IgnoreResult());

			ServiceChangedCommand = new DelegateCommand(() => { if (EntryBaseInfoPageModel.SelectedService != null) { DisplayServiceNotEntered = false; } } );
			LocationChangedCommand = new DelegateCommand(() => { if (EntryBaseInfoPageModel.SelectedLocation != null) { DisplayLocationNotEntered = false; } } );

			var now = DateTime.Now;
			MinDateOfService = now.Date;
		}

		async Task OnDateSelectedCommand()
		{
			await UpdateServices();
		}

		async Task UpdateServices()
		{
			try
			{
				Busy = true;

				// call the server with this case ID and the date to get the services and respective locations
				var response = await _dataUpdateService.GetLocationsAndServices(EntryBaseInfoPageModel.Case.ID, EntryBaseInfoPageModel.DateOfService);

				Services = response.Services;
			}
			catch (Exception ex)
			{
                // something has gone wrong updating the services.
                // report this error and don't mov onto the next page
#if DEBUG
				ErrorMessage = "Unable to get locations and services : " + ex.GetType().Name + " " + ex.Message;
#else
                ErrorMessage = "Unable to get locations and services.";
#endif
				DisplayErrorMessage = true;
				CanContinue = false;

			}
			finally
			{
				Busy = false;
			}
		}

		async Task OnContinueCommand()
		{
			if (!ValidateEntry())
			{
				// it's not valid, so don't bother sending it to the server.
				return;
			}

			await _wizardController.NextPage(_pageNumber);

		}

		async Task OnCancelCommand()
		{
			await _navigationService.GoBackAsync();
		}
		

		public async void OnNavigatingTo(NavigationParameters parameters)
		{
			if (!_initialized)
			{
				_initialized = true;
				_wizardController = parameters.GetValue<IWizardController>("IWizardController");
				_pageNumber = parameters.GetValue<int>("PageNumber");

				EntryBaseInfoPageModel = parameters.GetValue<EntryBaseInfoPageModel>("EntryBaseInfoPageModel");

				if (Services == null)
				{
					await UpdateServices();
				}
			}

		}

		bool ValidateEntry()
		{
			DisplayServiceNotEntered = (EntryBaseInfoPageModel.SelectedService == null);
			DisplayLocationNotEntered = (EntryBaseInfoPageModel.SelectedLocation == null);

			return (EntryBaseInfoPageModel.SelectedService != null && EntryBaseInfoPageModel.SelectedLocation != null);
		}

	}
}
