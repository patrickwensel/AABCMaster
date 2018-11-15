using AABC.Domain2.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace AABC.Domain2.Providers
{
    public class ProviderServiceZipCode
    {
        public int ID { get; set; }

        public DateTime DateCreated { get; set; }

        [Timestamp]
        public byte[] rv { get; set; }

        public int ProviderID { get; set; }

        public string ZipCode { get; set; }

        public bool IsPrimary { get; set; }

        public virtual Provider Provider { get; set; }

        public virtual ZipInfo ZipInfo { get; set; }
    }
}
