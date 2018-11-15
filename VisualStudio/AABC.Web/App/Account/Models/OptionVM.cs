using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.App.Account.Models
{
    public class OptionVM : Domain.Admin.UserOption
	{

		public IViewModelBase Base { get; set; }
		public WebViewHelper ViewHelper { get; set; }

		public OptionVM() {
			this.ViewHelper = new WebViewHelper(this);
		}

		public class WebViewHelper : IWebViewHelper
		{

			public List<Domain.Admin.UserOption> UserOptionList { get; set; }

			public bool HasValidationErrors { get; set; }
			public string ReturnErrorMessage { get; set; }

			public void BindModel() {

			}

			public bool Validate() {

				if (parent.UserOptionID == null) {
					HasValidationErrors = true;
					return false;
				}

				return true;
			}

			private OptionVM parent;

			public WebViewHelper(OptionVM parent) {
				this.parent = parent;
			}

		}

	}
}