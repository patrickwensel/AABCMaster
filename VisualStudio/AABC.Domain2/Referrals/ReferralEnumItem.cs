using System;

namespace AABC.Domain2.Referrals
{
    public class ReferralEnumItem
    {
        public const string InsuranceStatus = "InsuranceStatus";
        public const string IntakeStatus = "IntakeStatus";
        public const string RxStatus = "RxStatus";
        public const string InsuranceCardStatus = "InsuranceCardStatus";
        public const string EvaluationStatus = "EvaluationStatus";
        public const string PolicyBookStatus = "PolicyBookStatus";

        public int ID { get; set; }
        public DateTime DateCreated { get; private set; }
        public string StatusType { get; set; }
        public string Label { get; set; }
        public int Order { get; set; }
        public string ColorCode { get; set; }

        public ReferralEnumItem()
        {
            DateCreated = DateTime.UtcNow;
        }
    }
}
