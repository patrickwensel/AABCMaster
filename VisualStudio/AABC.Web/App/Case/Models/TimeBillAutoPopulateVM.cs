using System;

namespace AABC.Web.Models.Cases
{
    public class TimeBillAutoPopulateVM
    {

        // prefix with hap as this needs to coexist with
        // other dx controls, and dx binding is a nightmare if
        // field and control names don't match

        public Domain.Cases.CaseProvider hapProvider { get; set; }
        public DateTime hapStartDate { get; set; }
        public DateTime hapEndDate { get; set; }
        public bool hapMonday { get; set; }
        public bool hapTuesday { get; set; }
        public bool hapWednesday { get; set; }
        public bool hapThursday { get; set; }
        public bool hapFriday { get; set; }
        public bool hapSaturday { get; set; }
        public bool hapSunday { get; set; }
        public DateTime hapTimeIn { get; set; }
        public DateTime hapTimeOut { get; set; }
        public Domain.Cases.Service hapService { get; set; }
        public string hapNotes { get; set; }


    }
}