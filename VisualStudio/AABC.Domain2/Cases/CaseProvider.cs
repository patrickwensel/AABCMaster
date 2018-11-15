using System;
using System.Linq;

namespace AABC.Domain2.Cases
{
    public class CaseProvider
    {


        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int CaseID { get; set; }
        public int ProviderID { get; set; }
        public bool Active { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsAssessor { get; set; }
        public bool IsAuthorizedBCBA { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Case Case { get; set; }
        public virtual Providers.Provider Provider { get; set; }
        





        /// <summary>
        /// Verify that the Supervisor, Assessor and AuthorizedBCBA Roles are valid per the given reference date
        /// </summary>
        /// <returns>Returns false if the Provider.Type is not a BCBA, or if there's more than one applied supervisor, authorized BCBA or Assessor active per the specified date</returns>
        public bool VerifyRoles(DateTime refDate) {

            // first verify that the three roles attempting to be applied are
            // being applied against a BCBA only
            if (IsAssessor || IsSupervisor || IsAuthorizedBCBA) {
                if (Provider.ProviderType.Code != "BCBA") {
                    return false;
                }
            }

            // next verify against existing case providers for the specified date
            var providers = Case.GetProvidersAtDate(refDate);
            
            int supervisorCount = providers.Where(x => x.IsSupervisor).Count();
            int authedBcbaCount = providers.Where(x => x.IsAuthorizedBCBA).Count();
            int assessorCount = providers.Where(x => x.IsAssessor).Count();

            if (supervisorCount > 1  || authedBcbaCount > 1 || assessorCount > 1) {
                return false;
            } else {
                return true;
            }

        }
        
    }
}
