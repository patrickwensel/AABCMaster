using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.App.Staffing.Models
{
    public class ProvidersListVM<TItem>
    {
        public IViewModelListBase ListBase { get; set; }
        public int CaseID { get; set; }
        public string PreferredGender { get; set; }
        public IEnumerable<TItem> Providers { get; set; }

        public ProvidersListVM()
        {
            ListBase = new ViewModelListBase();
        }
    }
}