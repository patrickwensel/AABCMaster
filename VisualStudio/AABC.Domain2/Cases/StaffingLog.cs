using System;
using System.Collections.Generic;

namespace AABC.Domain2.Cases
{
    public class StaffingLog
    {
        public int ID { get; set; }
        public bool Active { get; set; } = true;
        public string ParentalRestaffRequest { get; set; }
        public decimal? HoursOfABATherapy { get; set; }
        public string AidesRespondingNo { get; set; }
        public string AidesRespondingMaybe { get; set; }
        public int ScheduleRequest { get; set; }
        public DateTime? DateWentToRestaff { get; set; }
        public string ProviderGenderPreference { get; set; }        
        public Case Case { get; set; }
        

        public virtual ICollection<StaffingLogParentContactLog> StaffingLogParentContactLog { get; set; }

        public virtual ICollection<SpecialAttentionNeed> SpecialAttentionNeeds { get; set; }
    }
}
