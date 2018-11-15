using System;
using System.Collections.Generic;

namespace AABC.Web.App.Referrals.Models
{
    public class ReferralListItemModel
    {
        public int ID { get; set; }
        public string AssignedStaff { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string InsuranceName { get; set; }
        public string BenefitCheck { get; set; }
        public IEnumerable<StatusListItem> Status { get; set; }
        public string StatusNote { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }

    public class StatusListItem
    {
        public string Label { get; set; }
        public string ColorCode { get; set; }
    }
}