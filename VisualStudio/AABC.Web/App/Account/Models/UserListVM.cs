using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.App.Account.Models
{
    public class UserListVM
	{

		public IViewModelBase Base { get; set; }
		public IViewModelListBase ListBase { get; set; }
		public WebViewHelper ViewHelper;
		public List<UserVM> DetailList { get; set; }

		public UserListVM() {
			ViewHelper = new WebViewHelper(this);
		}

		public class WebViewHelper : IWebViewHelper
		{
			UserListVM parent;

			public WebViewHelper(UserListVM parent) {
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