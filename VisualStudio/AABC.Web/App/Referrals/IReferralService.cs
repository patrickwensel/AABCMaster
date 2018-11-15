using AABC.Domain.General;
using AABC.Domain.Referrals;
using System.Collections.Generic;

namespace AABC.Web.App.Referrals
{
    public interface IReferralService<TReferral,TStaff, TLanguage, TDismissalReasonType, TSourceType>
    {
        TReferral GetReferral(int id);
        TReferral Insert(TReferral referral);
        TReferral Save(TReferral referral);
        void Deactivate(int referralID);
        void Delete(int referralID);
        IEnumerable<TDismissalReasonType> GetDismissalReasonTypes();
        IEnumerable<TLanguage> GetLanguages();
        IEnumerable<TSourceType> GetSourceTypes();
        IEnumerable<State> GetStates();
        IEnumerable<StatusDescriptor> GetStatusList();
        IEnumerable<TStaff> GetOfficeStaffList();
    }
}