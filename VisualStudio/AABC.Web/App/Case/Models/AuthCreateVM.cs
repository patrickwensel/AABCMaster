using AABC.Domain.Cases;
using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.Models.Cases
{
    public class AuthCreateVM
    {
        
        // put this in a detail object so the naming for the auth edit
        // popup in the devex view doesn't collide with the underlying
        // controls in the parent view
        public CaseAuthorization Detail { get; set; }
                
        public WebViewHelper ViewHelper { get; set; }
        
        public AuthCreateVM() {
            ViewHelper = new WebViewHelper(this);
            
        }

        public Authorization NewAuth { get; set; }



        public class WebViewHelper : IWebViewHelper
        {
            
            public List<AuthorizationClass> AuthClasses { get; set; }
            public List<Authorization> Authorizations { get; set; }
            public int? CaseID { get; set; }

            

            // boilerplate stuff

            AuthCreateVM parent;

            public WebViewHelper(AuthCreateVM parent) {
                this.parent = parent;
            }
            
            public bool HasValidationErrors { get; set; }

            public string ReturnErrorMessage { get; set; }

            public void BindModel() {
                
            }

            public bool Validate() {
                if (parent.Detail.StartDate ==  null || parent.Detail.EndDate == null) {
                    return false;
                }
                if ((parent.Detail.EndDate - parent.Detail.StartDate).TotalDays < 7)
                {
                    return false;
                }
                return true;    
            }
        }

    }
}