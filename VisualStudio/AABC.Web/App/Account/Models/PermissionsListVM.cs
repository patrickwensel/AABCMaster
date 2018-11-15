using AABC.Domain.Admin;
using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.Models.Permissions
{
    public class PermissionsListVM : Domain.Admin.Permission
	{

		public IViewModelBase Base { get; set; }
		public WebViewHelper ViewHelper;
		public List<Permission> PermissionList { get; set; }

		public PermissionsListVM() {
			ViewHelper = new WebViewHelper(this);
		}

		public class WebViewHelper : IWebViewHelper
		{
			PermissionsListVM parent;

			public WebViewHelper(PermissionsListVM parent) {
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