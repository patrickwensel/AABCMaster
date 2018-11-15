using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.Domain2.Hours;
using AABC.Domain2.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.HoursResolution.Repositories
{
    public class ResolutionServiceRepository : IResolutionServiceRepository
    {

        private readonly CoreContext _context;

        public ResolutionServiceRepository(CoreContext context)
        {
            _context = context;
        }


        // hours fetches are restricted within this window
        public DateTime DateRangeLow { get; set; } = new DateTime(1900, 1, 1);
        public DateTime DateRangeHigh { get; set; } = new DateTime(2100, 1, 1);


        public Domain2.Insurances.Insurance GetActiveInsurance(int caseID, DateTime refDate)
        {
            var caseInsurance = _context.Cases.Find(caseID).GetActiveInsuranceAtDate(refDate);
            return caseInsurance == null ? null : _context.Insurances.Find(caseInsurance.InsuranceID);
        }

        public Case GetCase(int caseID)
        {
            return _context.Cases.Find(caseID);
        }

        public List<Domain2.Hours.Hours> GetCaseHours(int caseID)
        {
            return GetCase(caseID).Hours
                .Where(x => x.Date >= DateRangeLow && x.Date <= DateRangeHigh)
                .ToList();
        }

        public int? GetPriorEntryServiceID(Domain2.Hours.Hours entry)
        {

            if (entry.ID == 0)
            {
                return null;
            }
            var result = _context.Database.SqlQuery<int?>("SELECT HoursServiceID FROM dbo.CaseAuthHours WHERE ID = " + entry.ID);
            return result.FirstOrDefault<int?>();
        }



        public Provider GetProvider(int providerID)
        {
            return _context.Providers.Find(providerID);
        }

        public List<Domain2.Hours.Hours> GetProviderHours(int providerID)
        {
            return _context.Hours
                .Where(x =>
                    x.ProviderID == providerID &&
                    x.Date >= DateRangeLow &&
                    x.Date <= DateRangeHigh)
                .ToList();
        }

        public List<Case> GetSSGCases(Domain2.Hours.Hours entry)
        {
            var results = new List<Case>();
            foreach (int caseID in entry.SSGCaseIDs)
            {
                results.Add(_context.Cases.Find(caseID));
            }
            return results;
        }

        public void RemoveAuthorizationBreakdown(AuthorizationBreakdown e)
        {
            _context.AuthorizationBreakdowns.Remove(e);
            _context.SaveChanges();
        }

        public void AddSSGEntry(Domain2.Hours.Hours newSSGHoursEntry)
        {
            _context.Hours.Add(newSSGHoursEntry);
            _context.SaveChanges();
        }

        public void RemoveOrphanedSSGRecords(int ssgParentID)
        {

            var hours = _context.Hours
                .Where(x =>
                    x.SSGParentID == ssgParentID &&
                    x.SSGParentID != x.ID &&
                    x.Date >= DateRangeLow &&
                    x.Date <= DateRangeHigh
                ).ToList();

            if (hours.Count > 0)
            {
                _context.Hours.RemoveRange(hours);
                _context.SaveChanges();
            }
        }

        public void SaveEntry(Domain2.Hours.Hours entry)
        {
            _context.Hours.Add(entry);
            _context.SaveChanges();
        }

        public int[] GetAssessmentIDs()
        {

            var sp = new DomainServices.Services.ServiceProvider(_context);
            return sp.GetServicesByType(Domain2.Services.ServiceTypes.Assessment).Select(x => x.ID).ToArray();

        }
    }
}
