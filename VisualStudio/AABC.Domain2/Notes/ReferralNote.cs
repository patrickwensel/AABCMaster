using AABC.Domain2.Referrals;

namespace AABC.Domain2.Notes
{
    public class ReferralNote : BaseNote<ReferralNoteTask>
    {
        public int ReferralID { get; set; }
        public virtual Referral Referral { get; set; }

        public override int GetParentID()
        {
            return ReferralID;
        }

        public override void SetParentId(int parentId)
        {
            ReferralID = parentId;
        }

        public override string GetPatientName()
        {
            return (Referral.FirstName + " " + Referral.LastName).Trim();
        }
        
    }
}
