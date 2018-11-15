using System;

namespace AABC.Web.Models.Cases
{
    public class TimeBillDefinalizeProvider
    {


        public Domain.Cases.CaseProvider dcpProvider { get; set; }
        public DateTime dcpSelectedDate { get; set; }
        public int CaseID { get; set; }

    }
}