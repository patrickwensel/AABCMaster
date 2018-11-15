using AABC.Domain2.Cases;
using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.Models.Cases
{

    public class CaseStatusDescription
    {
        public int ID { get; set; }
        public string Description { get; set; }
    }

    public class SummaryVM : Domain.Cases.Case
    {

        public IViewModelBase Base { get; set; }
        public WebViewHelper ViewHelper { get; set; }
        public bool HasAuthorizations { get; set; }
        //public bool ShowHistory { get; set; }

        public SummaryVM()
        {
            ViewHelper = new WebViewHelper(this);
        }







        public class WebViewHelper : IWebViewHelper
        {

            private SummaryVM parent;
            public bool HasValidationErrors { get; set; }
            public string ReturnErrorMessage { get; set; }

            public List<CaseStatusDescription> StatusDescriptionList { get; set; }
            public List<Domain.OfficeStaff.OfficeStaff> OfficeStaffList { get; set; }
            public List<Domain.Services.ServiceLocation> ServiceLocationList { get; set; }
            public IEnumerable<FunctioningLevel> FunctioningLevelList { get; set; }


            public void BindModel()
            {
                if (parent.FunctioningLevelID.HasValue)
                {
                    parent.FunctioningLevel = AppService.Current.DataContextV2.FunctioningLevels.Where(x => x.ID == parent.FunctioningLevelID).Single();
                }
                else
                {
                    parent.FunctioningLevel = null;
                }

            }

            public bool Validate()
            {
                return true;
            }

            public WebViewHelper(SummaryVM parent)
            {
                this.parent = parent;
            }
        }

    }
}