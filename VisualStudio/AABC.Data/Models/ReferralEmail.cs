using System;
using System.ComponentModel.DataAnnotations;

namespace AABC.Data.Models
{
    public class ReferralEmail
    {
        [Key]
        public string MessageID { get; set; }

        public string MessageSubject { get; set; }

        public string MessageStatus { get; set; }

        public DateTime ProcessedTime { get; set; }
    }
}
