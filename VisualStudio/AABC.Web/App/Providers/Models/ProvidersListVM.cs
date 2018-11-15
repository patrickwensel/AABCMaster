using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.Models.Providers
{
    public class ProvidersListVM
    {

        public IViewModelBase Base { get; set; }
        public IViewModelListBase ListBase { get; set; }

        public IEnumerable<ProvidersListItemVM> DetailList { get; set; }

        public WebViewHelper ViewHelper;

        public ProvidersListVM()
        {
            ViewHelper = new WebViewHelper(this);
            ListBase = new ViewModelListBase();
        }


        public class WebViewHelper : IWebViewHelper
        {

            ProvidersListVM parent;

            public bool HasValidationErrors { get; set; }

            public WebViewHelper(ProvidersListVM parent)
            {
                this.parent = parent;
            }

            public string ReturnErrorMessage { get; set; }

            public void BindModel()
            {

            }

            public bool Validate()
            {
                return true;
            }
        }


    }
}