using AABC.Domain2.PatientPortal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Patients
{
    public class SignInService
    {
        public SignInEx Get(int Id)
        {
            var signin = _context.PatientPortalSignIns.Where(l => l.Id == Id).FirstOrDefault();
            return Transform(signin);
        }
        public int GetUserId(string Username)
        {
            int r = _context.PatientPortalWebMembershipDetails.Where(l => l.User.Email == Username).FirstOrDefault().User.ID;
            return r;
        }
        public List<SignInEx> GetSignIns(int UserId, DateTime StartDate, DateTime EndDate)
        {
            List<SignInEx> r = new List<SignInEx>();
            var aSignIn = _context.PatientPortalSignIns.Where(p => p.UserId == UserId && p.SignInDate >= StartDate && p.SignInDate < EndDate.AddDays(1).Date);
            foreach (var c in aSignIn)
            {
                r.Add(Transform(c));
            }
            return r;
        }
        public List<SignInSummaryEx> GetSignInSummary(DateTime? StartDate, DateTime? EndDate)
        {

            var aSignIn = _context.PatientPortalSignIns.AsQueryable();
            if (StartDate.HasValue) {
                aSignIn = aSignIn.Where(p => p.SignInDate >= StartDate);
            }
            if (EndDate.HasValue)
            {
                DateTime endDate = EndDate.Value.AddDays(1).Date;
                aSignIn = aSignIn.Where(p => p.SignInDate < endDate);
            }
            return aSignIn.GroupBy(p => p.UserId).Select(p => new SignInSummaryEx() { UserId = p.FirstOrDefault().UserId, FirstName = p.FirstOrDefault().PatientPortalWebMembership.User.FirstName, LastName = p.FirstOrDefault().PatientPortalWebMembership.User.LastName, Email = p.FirstOrDefault().PatientPortalWebMembership.User.Email, Count = p.Count(), LastSignIn = p.OrderByDescending(d => d.SignInDate).FirstOrDefault().SignInDate }).ToList();
        }
        public void Save(SignInEx signin)
        {
            PatientPortalSignIn m = Transform(signin);
            if (m.Id == 0)
            {
                _context.PatientPortalSignIns.Add(m);
            }
            _context.SaveChanges();
            signin.Id = m.Id;
        }

        private List<SignInEx> Transform(IQueryable<PatientPortalSignIn> n)
        {
            List<SignInEx> r = new List<SignInEx>();
            foreach (var a in n)
            {
                r.Add(Transform(a));
            }
            return r;
        }
        private SignInEx Transform(PatientPortalSignIn n)
        {
            SignInEx r = new SignInEx();
            r.Id = n.Id;
            r.SignInDate = n.SignInDate;
            r.SignInType = n.SignInType;
            r.UserId = n.UserId;
            return r;
        }
        private PatientPortalSignIn Transform(SignInEx n)
        {
            PatientPortalSignIn r;
            if (n.Id > 0)
            {
                r = _context.PatientPortalSignIns.Where(p => p.Id == n.Id).FirstOrDefault();
            }
            else
            {
                r = _context.PatientPortalSignIns.Create();
            }
            r.Id = n.Id;
            r.SignInDate = n.SignInDate;
            r.SignInType = n.SignInType;
            r.UserId = n.UserId;
            return r;
        }
        private Data.V2.CoreContext _context;

        public SignInService(Data.V2.CoreContext context)
        {
            _context = context;
        }

    }
}
