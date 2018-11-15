using AABC.Mobile.SharedEntities.Entities;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AABC.Mobile.Controls
{
	[AddINotifyPropertyChangedInterface]
	public class DisplayCase
	{
		public Case Case { get; set; }

		public bool Required { get; set; }

		public bool Selected { get; set; }

		public ImageSource ImageSource { get; set; }
	}

	public class SelectedUsersChangedEventArgs : EventArgs
	{
		public List<int> SelectedCaseIds { get; set; }
	}


	public partial class SsgSelectionControl : ContentView
	{
		public static readonly BindableProperty CasesProperty = BindableProperty.Create("Cases", typeof(List<Case>), typeof(SsgSelectionControl), null, propertyChanged: (ssgSelectionControl, o, n) => ((SsgSelectionControl)ssgSelectionControl).OnCasesChanged((List<Case>)n));

		public static readonly BindableProperty SelectedCaseIdsProperty = BindableProperty.Create("SelectedCaseIds", typeof(List<int>), typeof(SsgSelectionControl), new List<int>(), defaultBindingMode: BindingMode.TwoWay, propertyChanged: (ssgSelectionControl, o, n) => ((SsgSelectionControl)ssgSelectionControl).OnSelectedCasesChanged());

		public event EventHandler<SelectedUsersChangedEventArgs> SelectedUsersChanged;

		public ObservableCollection<DisplayCase> DisplayCases { get; set; }

		public ICommand CaseTappedCommand { get; set; }

		public List<Case> Cases
		{
			get { return (List<Case>)this.GetValue(CasesProperty); }
			set { this.SetValue(CasesProperty, value); }
		}

		public List<int> SelectedCaseIds
		{
			get { return (List<int>)this.GetValue(SelectedCaseIdsProperty); }
			set { this.SetValue(SelectedCaseIdsProperty, value); }
		}
		

		public SsgSelectionControl()
		{
			CaseTappedCommand = new DelegateCommand<DisplayCase>((displayCase) => OnCaseTappedCommandCommand(displayCase));

			InitializeComponent();
		}

		void OnCaseTappedCommandCommand(DisplayCase displayCase)
		{
			if (!displayCase.Required)
			{
				displayCase.Selected = !displayCase.Selected;
				displayCase.ImageSource = GetSelectedImageSource(displayCase.Selected, displayCase.Required);

				// add or remove from the list
				if (displayCase.Selected)
				{
					SelectedCaseIds.Add(displayCase.Case.ID);
				}
				else
				{
					SelectedCaseIds.Remove(displayCase.Case.ID);
				}

				SelectedUsersChanged?.Invoke(this, new SelectedUsersChangedEventArgs { SelectedCaseIds = SelectedCaseIds });
			}
		}


		void OnCasesChanged(List<Case> cases)
		{
			var selectedCases = this.SelectedCaseIds;
			DisplayCases = new ObservableCollection<DisplayCase>(
														cases.Select((c, index) =>
															{
																var selected = selectedCases?.Contains(c.ID) ?? false;
																return new DisplayCase
																{
																	Case = c,
																	Selected = selected,
																	Required = (index == 0),
																	ImageSource = GetSelectedImageSource(selected, (index == 0))
																};
															}));

			CasesList.HeightRequest = CasesList.RowHeight * DisplayCases.Count;
		}

		void OnSelectedCasesChanged()
		{
			var selectedCaseIds = this.SelectedCaseIds;

			if (DisplayCases != null)
			{ 
				int index = 0;
				foreach (var displayCase in DisplayCases)
				{
					var selected = selectedCaseIds?.Contains(displayCase.Case.ID) ?? false;
					if (displayCase.Selected != selected)
					{
						displayCase.Selected = selected;
						displayCase.ImageSource = GetSelectedImageSource(selected, (index == 0));
					}
					index++;
				}
			}
		}

		ImageSource GetSelectedImageSource(bool selected, bool required)
		{
			if (required)
			{
				return Xamarin.Forms.ImageSource.FromResource("AABC.Mobile.Resources." + "ButtonImages.RequiredUser.png");
			}
			else if (selected)
			{
				return Xamarin.Forms.ImageSource.FromResource("AABC.Mobile.Resources." + "ButtonImages.SelectedUser.png");
			}
			else
			{
				return null;
				// return Xamarin.Forms.ImageSource.FromResource("AABC.Mobile.Resources." + "ButtonImages.UnselectedUser.png");
			}

		}

	}
}