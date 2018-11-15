using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.Models.OfficeStaff
{
    public class OfficeStaffListModel
    {
        

        public IViewModelBase Base { get; set; }
        public IViewModelListBase ListBase { get; set; }
        public List<Domain.OfficeStaff.OfficeStaff> DetailList { get; set; }

        public OfficeStaffListModel() {
            this.ListBase = new ViewModelListBase();
        }
        
        
        

                    
        

    }
}