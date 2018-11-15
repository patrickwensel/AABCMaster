using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using Prism.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AABC.Mobile.Pages.Update
{
	[AddINotifyPropertyChangedInterface]
	class UpdatePageViewModel
	{
		readonly IUpdateService _updateService;


		public ICommand Update { get; set; }

		public UpdatePageViewModel(IUpdateService updateService)
		{
			_updateService = updateService;

			Update = new DelegateCommand(() => OnUpdate().IgnoreResult());
		}

		async Task OnUpdate()
		{
			await _updateService.RedirectToUpdate();
		}
	}
}
