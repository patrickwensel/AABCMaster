using System;
using System.Collections.Generic;

namespace AABC.Domain2.Providers
{

    public class ProviderSubType
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; private set; }
        public int ParentTypeID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ProviderType ParentType { get; set; }
        public virtual ICollection<Provider> Providers { get; set; }

        public ProviderSubType()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}
