
using Dymeng.Framework.Web.Mvc.Views;

namespace AABC.Web.Models.Permissions
{
    public class PermissionVM : Domain.Admin.Permission
	{

		public IViewModelBase Base { get; set; }
		public WebViewHelper ViewHelper { get; set; }

		public PermissionVM() {
			this.ViewHelper = new WebViewHelper(this);
		}


		public class WebViewHelper : IWebViewHelper
		{

			public bool HasValidationErrors { get; set; }
			public string ReturnErrorMessage { get; set; }

			public void BindModel() {
			}

			public bool Validate() {
				return true;
			}

			private PermissionVM parent;

			public WebViewHelper(PermissionVM parent) {
				this.parent = parent;
			}

			public bool IsAuthenticated() {
				return true;
			}
		}
	}


}