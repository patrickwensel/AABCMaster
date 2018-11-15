using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.Domain2.Providers;
using System;
using System.Linq;


namespace AABC.DomainServices.Staffing
{
    public class StaffingService : IStaffingService
    {
        
        private readonly CoreContext Context;


        public StaffingService(CoreContext context)
        {
            Context = context;
        }

        // returns true if an update was made to the staffing records
        public bool PerformCheckByProviderId(int providerId)
        {
            bool retval = false;

            // check for non-existant staff records that need to be added
            var cases = GetNewCasesForRestaff().Where(m => m.Providers.Any(p => p.ID == providerId)).ToList();
            if (cases.Count() > 0)
            {
                foreach (var c in cases)
                {
                    var staffingLog = CreateStaffingLog(c.ID);
                    Context.StaffingLog.Add(staffingLog);
                }
                Context.SaveChanges();
                retval = true;
            }

            // check for existing staffing records that need to be updated to active
            cases = GetUpdateCasesForRestaff().Where(m => m.Providers.Any(p => p.ID == providerId)).ToList();
            if (cases.Count() > 0) {
                foreach (var c in cases) {
                    var staffingLog = Context.StaffingLog.Find(c.ID);
                    staffingLog.Active = true;
                    staffingLog.DateWentToRestaff = DateTime.Today;
                }
                Context.SaveChanges();
                retval = true;
            }

            // check for existing staffing records that need to be updated to inactive
            cases = GetUpdateCasesForRestaffRemoval().Where(m => m.Providers.Any(p => p.ID == providerId)).ToList();
            if (cases.Count() > 0) {
                foreach (var c in cases) {
                    var staffingLog = Context.StaffingLog.Find(c.ID);
                    staffingLog.Active = false;
                }
                Context.SaveChanges();
                retval = true;
            }

            return retval;
        }


        // returns true if an update was made to the staffing records
        public bool PerformCheckByCaseId(int caseId)
        {
            bool retval = false;

            // check for non-existant staff records that need to be added
            var @case = GetNewCasesForRestaff().SingleOrDefault(m => m.ID == caseId);
            if (@case != null)
            {
                var staffingLog = CreateStaffingLog(@case.ID);
                Context.StaffingLog.Add(staffingLog);
                Context.SaveChanges();
                retval = true;
            }

            // check for existing staffing records that need to be updated to active
            @case = GetUpdateCasesForRestaff().SingleOrDefault(m => m.ID == caseId);
            if (@case != null) {
                var staffingLog = Context.StaffingLog.Find(@case.ID);
                staffingLog.Active = true;
                staffingLog.DateWentToRestaff = DateTime.Today;
                Context.SaveChanges();
                retval = true;
            }

            // check for existing staffing records that need to be updated to inactive
            @case = GetUpdateCasesForRestaffRemoval().SingleOrDefault(m => m.ID == caseId);
            if (@case != null) {
                var staffingLog = Context.StaffingLog.Find(@case.ID);
                if (staffingLog != null) {
                    staffingLog.Active = false;
                    Context.SaveChanges();
                    retval = true;
                }                
            }
            return retval;
        }


        // Get cases with existing staffing logs that need to be set to inactive
        private IQueryable<Case> GetUpdateCasesForRestaffRemoval() {

            var cases = Context.Cases.Where(c =>
                (Context.StaffingLog.Any(sl => c.ID == sl.ID && sl.Active == true)) &&
                (!c.NeedsRestaffing) ||
                (c.NeedsRestaffing && (
                    (c.RestaffReasonID != (int)RestaffReason.NewNeedsBCBA && c.RestaffReasonID != (int)RestaffReason.NewNeedsAide) ||
                    (c.Providers.Any(p => 
                        p.Provider.ProviderSubTypeID == (int)ProviderTypeIDs.Aide &&
                        p.Active &&
                        (p.StartDate == null || p.StartDate <= DateTime.Now) && (p.EndDate == null || p.EndDate >= DateTime.Now)))
                ))
            );

            return cases;
        }


        // Get cases with existing staffing logs that need to be set to active
        private IQueryable<Case> GetUpdateCasesForRestaff() {

            var cases = Context.Cases.Where(c =>
                (Context.StaffingLog.Any(sl => c.ID == sl.ID && sl.Active == false)) &&
                (c.Status != CaseStatus.History) &&
                (c.NeedsRestaffing) &&
                (
                    (c.RestaffReasonID == (int)RestaffReason.NewNeedsBCBA || c.RestaffReasonID == (int)RestaffReason.NewNeedsAide) ||
                    (!c.Providers.Any(p =>
                        p.Provider.ProviderTypeID == (int)ProviderTypeIDs.Aide &&
                        p.Active &&
                        (p.StartDate == null || p.StartDate <= DateTime.Now) && (p.EndDate == null || p.EndDate >= DateTime.Now)))
                )
            );
            
            return cases;
        }


        // Get cases without staffing logs where the staffing log needs to be added
        private IQueryable<Case> GetNewCasesForRestaff()
        {
            var cases = Context.Cases.Where(c =>
                                (!Context.StaffingLog.Any(sl => c.ID == sl.ID)) &&
                                (c.Status != CaseStatus.History) &&
                                (c.NeedsRestaffing) &&
                                (
                                    (c.RestaffReasonID == (int)RestaffReason.NewNeedsBCBA || c.RestaffReasonID == (int)RestaffReason.NewNeedsAide) ||
                                    (!c.Providers.Any(p =>
                                        p.Provider.ProviderTypeID == (int)ProviderTypeIDs.Aide &&
                                        p.Active &&
                                        (p.StartDate == null || p.StartDate <= DateTime.Now) && (p.EndDate == null || p.EndDate >= DateTime.Now)))
                                )
                            );
            return cases;
        }


        private static StaffingLog CreateStaffingLog(int caseId)
        {
            var staffingLog = new StaffingLog
            {
                ID = caseId,
                DateWentToRestaff = DateTime.Today
            };
            return staffingLog;
        }
    }
}
