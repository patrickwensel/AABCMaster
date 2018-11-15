using System.Collections.Generic;

namespace AABC.Domain2.Cases
{
    public class SpecialAttentionNeed
    {
        public int ID { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public ICollection<StaffingLog> StaffingLog { get; set; }
    }
}
