
using Dymeng.Framework.Web.Mvc.Views;

namespace AABC.Web.Models.Providers
{
    public class ProviderTypeVM : Domain.Providers.ProviderType
    {

        public IViewModelBase Base { get; set; }
        public WebViewHelper ViewHelper { get; }
        
        public ProviderTypeVM() {
            this.ViewHelper = new WebViewHelper(this);
        }

        


        public class WebViewHelper : Dymeng.Framework.Web.Mvc.Views.IWebViewHelper
        {
            public string ReturnErrorMessage { get; set; }

            private ProviderTypeVM parent;

            public bool HasValidationErrors { get; set; }

            public WebViewHelper(ProviderTypeVM parent) {
                this.parent = parent;
            }

            public void BindModel() {

            }

            public bool Validate() {
                
                if (parent.Code == null || parent.Code == "") {
                    return false;
                }

                if (parent.Name == null || parent.Name == "") {
                    return false;
                }

                return true;

            }
            

        }

    }

    

}