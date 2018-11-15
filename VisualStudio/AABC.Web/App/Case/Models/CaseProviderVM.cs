using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.Models.Cases
{
    public class CaseProviderVM
    {
        public int ID { get; set; } // case ID, previous razor model expects "ID" so that's why it's not named CaseID
        public IEnumerable<CaseProviderListItem> Items { get; set; }
        public string PatientName { get; set; }
        public string PatientGender { get; set; }
        public IViewModelBase Base { get; set; }
        public WebViewHelper ViewHelper { get; set; }

        public CaseProviderVM()
        {
            ViewHelper = new WebViewHelper(this);
        }

        public class WebViewHelper : IWebViewHelper
        {
            readonly CaseProviderVM parent;
            public IEnumerable<ProviderDropdownListItem> Providers { get; set; }
            public bool HasValidationErrors { get; set; }
            public string ReturnErrorMessage { get; set; }

            public WebViewHelper(CaseProviderVM parent)
            {
                this.parent = parent;
            }

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