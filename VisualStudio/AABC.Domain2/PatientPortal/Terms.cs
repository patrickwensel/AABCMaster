using System;
using System.Collections.Generic;

namespace AABC.Domain2.PatientPortal
{
    public class Terms
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<AcceptedTerms> AcceptedTerms { get; set; }

        public Terms()
        {
            Created = DateTime.UtcNow;
        }
    }
}
