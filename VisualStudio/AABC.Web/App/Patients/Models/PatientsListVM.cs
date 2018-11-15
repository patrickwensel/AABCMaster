using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.Models.Patients
{

    public enum ListViewType
    {
        Active,
        Discharged
    }


    public class PatientsListVM<TItemType>
    {

        public IViewModelBase Base { get; set; }
        public IViewModelListBase ListBase { get; set; }
        public WebViewHelper ViewHelper { get; set; }
        public IEnumerable<TItemType> DetailList { get; set; }

        public ListViewType ListViewType { get; private set; }

        public PatientsListVM(ListViewType listViewType)
        {
            ListViewType = listViewType;
            ListBase = new ViewModelListBase();
            ViewHelper = new WebViewHelper(this);
        }



        public class WebViewHelper : IWebViewHelper
        {
            private readonly PatientsListVM<TItemType> parent;

            public WebViewHelper(PatientsListVM<TItemType> parent)
            {
                this.parent = parent;
            }

            public bool HasValidationErrors { get; set; }

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