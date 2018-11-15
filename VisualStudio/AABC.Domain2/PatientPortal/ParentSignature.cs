using System;

namespace AABC.Domain2.PatientPortal
{
    public class ParentSignature
    {

        public int ID { get; set; }
        public int LoginID { get; set; }

        public string Data { get; set; }
        public DateTime Date { get; set; }

        public virtual Login Login { get; set; }

    }
}
