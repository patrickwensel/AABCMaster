using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.App.Staffing.Models
{

    public enum ListViewType
    {
        Active,
        Discharged
    }


    public class StaffingListVM
    {
        
        public IViewModelBase Base { get; set; }
        public IViewModelListBase ListBase { get; set; }
        public WebViewHelper ViewHelper { get; set; }
        public IEnumerable<StaffingListItemVM> DetailList { get; set; }
                    
        public ListViewType ListViewType { get;  private set;}

        public StaffingListVM(ListViewType listViewType) {
            ListViewType = listViewType;
            ListBase = new ViewModelListBase();
            ViewHelper = new WebViewHelper(this);
        }



        public class WebViewHelper : IWebViewHelper
        {

            StaffingListVM parent;

            public WebViewHelper(StaffingListVM parent) {
                this.parent = parent;
            }

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