using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.App.Account.Models
{
    public class PermissionListVM
	{
		public IViewModelBase Base { get; set; }
		public IViewModelListBase ListBase { get; set; }
		public WebViewHelper ViewHelper;
		public List<PermissionListItemVM> PermissionList { get; set; }

		public PermissionListVM() {
			this.ViewHelper = new WebViewHelper(this);
			this.ListBase = new ViewModelListBase();
		}

		public class WebViewHelper : IWebViewHelper
		{
			private PermissionListVM parent;

			public WebViewHelper(PermissionListVM parent) {
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