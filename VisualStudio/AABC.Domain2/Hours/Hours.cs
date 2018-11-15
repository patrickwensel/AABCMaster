using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain2.Hours
{
    public class Hours
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [Obsolete("Use AuthorizationBreakdowns instead: each entry can now have multiple auths it maps to")]
        public int? AuthorizationID { get; set; }
        /* Not all entered hours have an authorization assigned to them at the time
         * of entry.  As such, we'll need to track the case ID as well, instead of
         * tracing it back through the authorization.  Business logic enforces that
         * the CaseID is present for any entered hours. */         
        public int CaseID { get; set; }
        public int ProviderID { get; set; }
        public HoursStatus Status { get; set; } = HoursStatus.Pending;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? ServiceID { get; set; }
        public string Memo { get; set; }
        public decimal TotalHours { get; set; }
        public decimal BillableHours { get; set; }
        public decimal PayableHours { get; set; }
        public string BillingRef { get; set; }
        public string PayableRef { get; set; }
        public bool HasCatalystData { get; set; }
        public bool WatchEnabled { get; set; }
        public string WatchNote { get; set; }
        public int? SSGParentID { get; set; }
        public int? CorrelationID { get; set; }
        public string InternalMemo { get; set; }
        public bool IsAdjustment { get; set; }
        public int? ServiceLocationID { get; set; }
        public int? ParentApprovalID { get; set; }
        public bool ParentReported { get; set; }
        public bool IsTrainingEntry { get; set; }
        public string EntryApp { get; set; }
        
        public int[] SSGCaseIDs { get; set; }   // not persisted

        [Obsolete("Use AuthorizationBreakdowns instead: each entry can now have multiple auths it maps to")]
        public virtual Authorizations.Authorization Authorization { get; set; }
        public virtual Cases.Case Case { get; set; }
        public virtual Providers.Provider Provider { get; set; }
        public virtual Services.Service Service { get; set; }
        public virtual Cases.ParentApproval ParentApproval { get; set; }
        public virtual Services.ServiceLocation ServiceLocation { get; set; }

        public virtual ICollection<ReportLogItem> ReportLog { get; set; } = new List<ReportLogItem>();
        public virtual ICollection<AuthorizationBreakdown> AuthorizationBreakdowns { get; set; } = new List<AuthorizationBreakdown>();
        public virtual ICollection<ExtendedNote> ExtendedNotes { get; set; } = new List<ExtendedNote>();
        public virtual SessionReport Report { get; set; }
        public virtual SessionSignature SessionSignature { get; set; }


        // Determine if this hours entry is allowed to be edited by the specified provider
        public bool IsEditableByProvider(Providers.Provider provider) {

            // only allow edits if this provider owns it
            if (this.Provider.ID != provider.ID) {
                return false;
            }

            // only allow edits to pending and committed hours
            if (this.Status != HoursStatus.Pending && this.Status != HoursStatus.ComittedByProvider) {
                return false;
            }

            // make sure the month isn't finalized
            var period = this.Case.Periods?.Where(x => x.FirstDayOfMonth == new DateTime(this.Date.Year, this.Date.Month, 1)).SingleOrDefault();
            if (period != null) {
                var finalization = period.Finalizations.Where(x => x.ProviderID == provider.ID).SingleOrDefault();
                if (finalization != null) {
                    return false;
                }
            }

            // make sure if we're an SSG then this is the master entry for it
            if (this.SSGParentID.HasValue) {
                if (this.SSGParentID.Value != this.ID) {
                    return false;
                }
            }

            return true;
        }

    }
}
