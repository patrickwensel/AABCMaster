using System;

namespace AABC.Domain2.Hours
{
    public class SessionSignature
    {
        public int ID { get; set; }
        public string ParentSignature { get; set; }
        public string ParentName { get; set; }
        public string ParentSignatureType { get; set; }
        public string ProviderSignature { get; set; }
        public string ProviderName { get; set; }
        public string ProviderSignatureType { get; set; }
        public DateTime Created { get; private set; }

        public SessionSignature()
        {
            Created = DateTime.UtcNow;
        }
    }
}
