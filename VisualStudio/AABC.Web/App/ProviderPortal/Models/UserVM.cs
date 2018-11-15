using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.Models.ProviderPortal
{
    public class UserVM
    {
        public IEnumerable<UserItem> Items { get; set; }

        public IViewModelBase Base { get; set; }
        public WebViewHelper ViewHelper { get; set; }

        public UserVM()
        {
            this.ViewHelper = new WebViewHelper(this);
        }

        public class WebViewHelper : IWebViewHelper
        {

            public bool HasValidationErrors { get; set; }
            public string ReturnErrorMessage { get; set; }

            public void BindModel()
            {
            }

            public bool Validate()
            {
                return true;
            }

            private readonly UserVM parent;

            public WebViewHelper(UserVM parent)
            {
                this.parent = parent;
            }


            public bool IsAuthenticated()
            {
                return true;
            }

        }
    }

    public class UserItem
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ProviderNumber { get; set; }
        public bool IsActive { get; set; }
        public bool HasAppAccess { get; set; }
    }

}