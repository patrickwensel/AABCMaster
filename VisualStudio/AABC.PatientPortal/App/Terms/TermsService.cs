using AABC.Domain2.PatientPortal;
using System;
using System.Linq;

namespace AABC.PatientPortal.App.Terms
{
    public class TermsService
    {
        private Data.V2.CoreContext _context;

        public TermsService()
        {
            _context = AppService.Current.Data.Context;
        }

        internal Domain2.PatientPortal.Terms GetLatestTerms()
        {
            var max = _context.Terms.Max(m => (DateTime?)m.Created);
            if (max.HasValue)
            {
                return _context.Terms.SingleOrDefault(m => m.Active && m.Created == max);
            }
            return _context.Terms.OrderByDescending(m => m.Created).FirstOrDefault();
        }

        internal void AcceptTerms()
        {
            var latestTerm = GetLatestTerms();
            if (latestTerm == null)
            {

            }
            var acceptedTerms = new AcceptedTerms()
            {
                TermsId = latestTerm.ID,
                LoginId = AppService.Current.User.ID
            };
            var user = _context.PatientPortalLogins.Where(x => x.ID == AppService.Current.User.CurrentUser.ID).Single();
            user.AcceptedTerms.Add(acceptedTerms);
            _context.SaveChanges();
        }

        internal bool UserHasAcceptedLatestTerms()
        {
            var latestTerms = GetLatestTerms();
            if (latestTerms == null) {
                return true;
            }
            return UserHasAcceptedTerms(latestTerms);
        }

        internal bool UserHasAcceptedTerms(Domain2.PatientPortal.Terms terms)
        {
            var ctx = new Data.V2.CoreContext();

            var user = ctx.PatientPortalLogins.Where(x => x.ID == AppService.Current.User.CurrentUser.ID).Single();
            return user.AcceptedTerms.Any(m => m.TermsId == terms.ID);
        }
    }


}