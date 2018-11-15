using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Pages.BaseViewModels;
using AABC.Mobile.SharedEntities.Entities;
using Plugin.ExternalMaps.Abstractions;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AABC.Mobile.Pages.S2
{
	enum DisplayInformationType
	{
		Contact = 0,
		Location,
	}

	class DisplayInformation
	{
		public DisplayInformationType DisplayInformationType { get; set; }

		public string MainText { get; set; }

		public string Subtext { get; set; }

		public ImageSource ImageSource { get; set; }
	}

	class DisplayCaseDetails
	{
		public string Name { get; set; }

		public string Location { get; set; }

	}

	[AddINotifyPropertyChangedInterface]
	class CaseDetailsPageViewModel : SelectedCaseViewModel, INavigatingAware
	{
		readonly INavigationService _navigationService;
		readonly IPhoneCallTask _phoneCallTask;
		readonly IPageDialogService _pageDialogService;
		readonly IExternalMaps _externalMaps;


		public DisplayCaseDetails DisplayCaseDetails { get; set; }

		public ICommand ViewInsuranceAndAuthorizations { get; set; }

		public ICommand InformationListCommand { get; set; }

		public bool PhoneAvailable { get; set; }

		public List<DisplayInformation> InformationList { get; set; }

		public CaseDetailsPageViewModel(INavigationService navigationService, IApplicationState applicationState, IPhoneCallTask phoneCallTask, IPageDialogService pageDialogService, IExternalMaps externalMaps) : base(applicationState)
		{
			_navigationService = navigationService;
			_phoneCallTask = phoneCallTask;
			_pageDialogService = pageDialogService;
			_externalMaps = externalMaps;

			ViewInsuranceAndAuthorizations = new DelegateCommand(() => OnViewInsuranceAndAuthorizations().IgnoreResult());
			InformationListCommand = new DelegateCommand<DisplayInformation>((displayInformation) => OnInformationListCommand(displayInformation).IgnoreResult());
		}

		async Task OnViewInsuranceAndAuthorizations()
		{
			await _navigationService.NavigateAsync("InsuranceAndAuthorizationsPage");
		}

		async Task OnInformationListCommand(DisplayInformation displayInformation)
		{
			switch (displayInformation.DisplayInformationType)
			{
				case DisplayInformationType.Contact:
					if (await _pageDialogService.DisplayAlertAsync("Call", $"Call {displayInformation.MainText} on {displayInformation.Subtext}", "Call", "Cancel"))
					{
						_phoneCallTask.MakePhoneCall(displayInformation.Subtext, displayInformation.MainText);
					}
					break;

				case DisplayInformationType.Location:
					await _externalMaps.NavigateTo(DisplayCaseDetails.Name, SelectedCase.Patient.PatientAddress1, SelectedCase.Patient.PatientCity, SelectedCase.Patient.PatientState, SelectedCase.Patient.PatientZip, "USA", "USA");
					break;
			}
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			PhoneAvailable = _phoneCallTask.CanMakePhoneCall;

			var patient = SelectedCase.Patient;

			var displayCaseDetails = new DisplayCaseDetails
				{
					Name = patient.PatientFirstName + " " + patient.PatientLastName.Substring(0, 1),
					Location = patient.PatientCity + ", " + patient.PatientState,
				};

			InformationList = new List<DisplayInformation>();

			if (!String.IsNullOrEmpty(patient.PatientGuardianFirstName))
			{
				InformationList.Add(new DisplayInformation 
					{
						DisplayInformationType = DisplayInformationType.Contact,
						MainText = patient.PatientGuardianFirstName + " " + patient.PatientGuardianLastName + " (" + patient.PatientGuardianRelationship + ")", 
						Subtext = patient.PatientGuardianPhone,
						ImageSource = ImageSource.FromResource("AABC.Mobile.Resources.ButtonImages.Phone.png")
					});
			}

			if (!String.IsNullOrEmpty(patient.PatientGuardian2FirstName))
			{
				InformationList.Add(new DisplayInformation
					{
						DisplayInformationType = DisplayInformationType.Contact,
						MainText = patient.PatientGuardian2FirstName + " " + patient.PatientGuardian2LastName + " (" + patient.PatientGuardian2Relationship + ")", 
						Subtext = patient.PatientGuardian2Phone,
						ImageSource = ImageSource.FromResource("AABC.Mobile.Resources.ButtonImages.Phone.png")
				});
			}

			string address2 = String.IsNullOrEmpty(patient.PatientAddress2) ? String.Empty : patient.PatientAddress2 + ", ";
			InformationList.Add(new DisplayInformation
			{
				DisplayInformationType = DisplayInformationType.Location,
				MainText = patient.PatientAddress1,
				Subtext = address2 + patient.PatientCity + ", " + patient.PatientState + " " + patient.PatientZip,
				ImageSource = ImageSource.FromResource("AABC.Mobile.Resources.ButtonImages.Location.png")
			});


			DisplayCaseDetails = displayCaseDetails;
		}
	}
}
