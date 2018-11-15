using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.App.Account.Models
{
    public class UserVM : Domain.Admin.User
	{


		public IViewModelBase Base { get; set; }
		public WebViewHelper ViewHelper { get; set; }

		public UserVM() {
			this.ViewHelper = new WebViewHelper(this);
		}


		public class WebViewHelper : IWebViewHelper
		{


            public string TempPass { get; set; }

            public List<Domain.OfficeStaff.OfficeStaff> OfficeStaffList { get; set; }

			public bool HasValidationErrors { get; set; }
			public string ReturnErrorMessage { get; set; }

			public void BindModel() {
			}

			public bool Validate() {

				if (parent.UserName == null) {
					HasValidationErrors = true;
					return false;
				}

				if (parent.FirstName == null) {
					HasValidationErrors = true;
					return false;
				}

				if (parent.LastName == null) {
					HasValidationErrors = true;
					return false;
				}

				if (parent.Email == null) {
					HasValidationErrors = true;
					return false;
				}

				return true;
			}



			private UserVM parent;

			public WebViewHelper(UserVM parent) {
				this.parent = parent;
			}

			public bool IsAuthenticated() {
				return true;
			}

		}
	}

	public class ManageUsersVM
	{

		public UserVM User { get; set; }
		public UserListVM UserList { get; set; }
		public PermissionListVM UserPermissionList { get; set; }
		public OptionListVM UserOptionList { get; set; }

	}
}