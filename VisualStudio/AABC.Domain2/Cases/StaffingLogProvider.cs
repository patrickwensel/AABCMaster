using System.Collections.Generic;

namespace AABC.Domain2.Cases
{
    public class StaffingLogProvider
    {
        public int ID { get; set; }
        public int StaffingLogID { get; set; }
        public int ProviderID { get; set; }

        public StaffingLog StaffingLog { get; set; }
        public virtual Providers.Provider Provider { get; set; }

        public virtual ICollection<StaffingLogProviderContactLog> StaffingLogProviderContactLog { get; set; }
    }
}
