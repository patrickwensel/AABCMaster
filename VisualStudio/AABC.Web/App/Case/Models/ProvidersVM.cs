using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.Models.Cases
{
    public class ProvidersVM : Domain.Cases.Case
    {

        public IViewModelBase Base { get; set; }
        public WebViewHelper ViewHelper { get; set; }

        public ProvidersVM() {
            ViewHelper = new WebViewHelper(this);
        }




        public class WebViewHelper : IWebViewHelper
        {

            ProvidersVM parent;

            public List<Providers.ProvidersListItemVM> Providers { get; set; }

            public bool HasValidationErrors { get; set; }
            public string ReturnErrorMessage { get; set; }

            public WebViewHelper(ProvidersVM parent) {
                this.parent = parent;
            }

            public void BindModel() {
                
            }

            public bool Validate() {
                return true;
            }
        }

    }
}