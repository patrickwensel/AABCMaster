

using Dymeng.Framework.Web.Mvc.Views;

namespace AABC.Web.Models.OfficeStaff
{




    public class OfficeStaffViewModel
    {
        
        public IViewModelBase Base { get; set; }    
        public WebViewHelper ViewHelper { get; set; }
        public Domain.OfficeStaff.OfficeStaff Detail { get; set; }
        
        //CTORs
        public OfficeStaffViewModel() {
            ViewHelper = new WebViewHelper(this);
            Detail = new Domain.OfficeStaff.OfficeStaff();
        }
        
        
        



        public bool Validate() {
            // TODO: implement validation
            if (Detail.FirstName == null) {
                return false;
            }
            if (Detail.LastName == null) {
                return false;
            }

            return true;
        }


        public class WebViewHelper : IWebViewHelper
        {

            OfficeStaffViewModel parent;

            public bool HasValidationErrors { get; set; }

            public string ReturnErrorMessage { get; set; }

            public WebViewHelper(OfficeStaffViewModel parent) {
                this.parent = parent;
            }

            public void BindModel() {
                
            }

            public bool Validate() {

                if (parent.Detail.FirstName == null) {
                    HasValidationErrors = true;
                    return false;
                }
                if (parent.Detail.LastName == null) {
                    HasValidationErrors = true;
                    return false;
                }

                return true;

            }
        }

    }

    

}