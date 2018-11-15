using AABC.Domain2.Cases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AABC.Web.App.Staffing.Models
{
    public class AddProviderContactLogVM
    {
        public int CaseId { get; set; }

        [Required(ErrorMessage = "Provider is required.")]
        public int? ProviderId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public int? StatusId { get; set; }

        [Required(ErrorMessage = "Contact Date is required.")]
        public DateTime ContactDate { get; set; }

        public string Notes { get; set; }

        public DateTime? FollowUpDate { get; set; }

        public List<StaffingLogProviderStatus> Statuses { get; set; }

        public List<CaseProviderVM> Providers { get; set; }
    }
}
