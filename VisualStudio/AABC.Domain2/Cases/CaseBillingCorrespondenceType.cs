using System.Collections.Generic;

namespace AABC.Domain2.Cases
{
    public class CaseBillingCorrespondenceType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CaseBillingCorrespondence> CaseBillingCorrespondences { get; set; }
    }
}
