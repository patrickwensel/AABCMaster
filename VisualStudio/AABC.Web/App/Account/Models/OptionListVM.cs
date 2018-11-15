using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.App.Account.Models
{
    public class OptionListVM
	{

		public IViewModelBase Base { get; set; }
		public IViewModelListBase ListBase { get; set; }
		public WebViewHelper ViewHelper;
		public List<OptionListItemVM> OptionList { get; set; }

		public OptionListVM() {
			ViewHelper = new WebViewHelper(this);
			ListBase = new ViewModelListBase();
		}

		public class WebViewHelper : IWebViewHelper
		{
			private OptionListVM parent;

			public WebViewHelper(OptionListVM parent) {
				this.parent = parent;
			}

			public bool HasValidationErrors { get; set; }

			public string ReturnErrorMessage { get; set; }

			public void BindModel() {

			}

			public bool Validate() {
				return true;
			}
		}

	}
}