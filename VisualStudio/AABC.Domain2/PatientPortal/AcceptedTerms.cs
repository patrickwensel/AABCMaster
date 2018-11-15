using System;

namespace AABC.Domain2.PatientPortal
{
    public class AcceptedTerms
    {
        public int LoginId { get; set; }
        public int TermsId { get; set; }
        public DateTime Created { get; set; }

        public AcceptedTerms()
        {
            Created = DateTime.UtcNow;
        }
    }
}
