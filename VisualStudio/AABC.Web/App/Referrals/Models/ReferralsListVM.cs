using Dymeng.Framework.Web.Mvc.Views;
using System.Collections.Generic;

namespace AABC.Web.App.Referrals.Models
{
    public class ReferralsListVM
    {
        public IViewModelBase Base { get; set; }
        public IViewModelListBase ListBase { get; set; }
        public IEnumerable<ReferralListItemModel> DetailList { get; set; }
        public string Action { get; set; }
        public bool IsShowingFollowUpsOnly { get; set; }

        public ReferralsListVM()
        {
            ListBase = new ViewModelListBase();
        }
    }
}