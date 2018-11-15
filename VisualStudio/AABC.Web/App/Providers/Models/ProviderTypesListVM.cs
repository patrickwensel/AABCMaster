using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.Models.Providers
{
    public class ProviderTypesListVM
    {

        public IViewModelBase Base { get; set; }
        public IViewModelListBase ListBase { get; set; }
        public WebViewHelper ViewHelper { get; }
        public List<ProviderTypesListItemVM> DetailList { get; set; }

        public ProviderTypesListVM() {
            this.ViewHelper = new WebViewHelper();
            ListBase = new ViewModelListBase();
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
        }

    }
}