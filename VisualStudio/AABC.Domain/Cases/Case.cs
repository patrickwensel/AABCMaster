using AABC.Domain2.Cases;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain.Cases
{

    public enum CaseStatus
    {
        History = -1,
        NotReady = 0,
        Good = 1,
        ConsiderForDischarge = 2
    }

    [Flags]
    public enum CaseStatusReason
    {
        NotSet = 0,
        NoBCBA = 1,
        NoPrescription = 2,
        NoIntake = 4,
        AuthExpired = 8
    }

    public class Case
    {

        public int? ID { get; set; }
        public DateTime? DateCreated { get; set; }

        public Patients.Patient Patient { get; set; }

        public List<CaseProvider> Providers { get; set; }
        public List<CaseTask> Tasks { get; set; }
        public List<CaseProviderNote> ProviderNotes { get; set; }
        public List<CaseAuthorization> Authorizations { get; set; }
        public List<MonthlyCasePeriod> MonthlyPeriods { get; set; }

        public int? GeneratingReferralID { get; set; }
        public CaseStatus Status { get; set; }
        public CaseStatusReason StatusReason { get; set; }
        public string StatusDisplay { get { return getStatusDisplay(); } }
        public string StatusNotes { get; set; }
        public DateTime? StartDate { get; set; }
        public OfficeStaff.OfficeStaff AssignedStaff { get; set; }
        public string RequiredHoursNotes { get; set; }
        public string RequiredServicesNotes { get; set; }
        public bool HasPrescription { get; set; }
        public bool HasAssessment { get; set; }
        public bool HasIntake { get; set; }
        public bool HasSupervisor { get { return hasSupervisor(); } }
        public bool HasAuthorization { get { return hasAuthorization(); } }
        public int? DefaultServiceLocationID { get; set; }
        public int? FunctioningLevelID { get; set; }

        public ICollection<Notes.Note> Notes { get; set; }

        public bool NeedsStaffing { get; set; }
        public bool NeedsRestaffing { get; set; }
        public string RestaffingReason { get; set; }

        public Services.ServiceLocation DefaultServiceLocation { get; set; }
        public FunctioningLevel FunctioningLevel { get; set; }

        public Case()
        {
            this.DateCreated = DateTime.UtcNow;
        }

        public CaseAuthorization GetCaseAuthForService(Service service, int authClassID)
        {

            // service will have a pre-mapped authClassID, we need to match that up with
            // whatever's on file for the cases authorizations

            var classedAuths = Authorizations.Where(x => x.AuthClass.ID == authClassID).ToList();

            if (classedAuths == null || classedAuths.Count == 0)
            {
                return null;
            }
            if (classedAuths.Count == 1)
            {
                return classedAuths[0];
            }

            // there's more than one auth matching the required class
            // let's grab the latest ending one...
            return classedAuths.OrderByDescending(x => x.EndDate).FirstOrDefault();

        }

        public static CaseStatusReason DefaultStatusReason
        {
            get
            {
                Case c = new Case();
                c.CalculateStatus();
                return c.StatusReason;
            }
        }

        public void CalculateStatus()
        {

            // if it's historic or CFD, don't calculate, return as is
            if (Status == CaseStatus.ConsiderForDischarge || Status == CaseStatus.History)
            {
                return;
            }

            CaseStatusReason reason = CaseStatusReason.NotSet;

            if (!HasSupervisor)
            {
                reason |= CaseStatusReason.NoBCBA;
            }

            if (!HasIntake)
            {
                reason |= CaseStatusReason.NoIntake;
            }

            if (!HasPrescription)
            {
                reason |= CaseStatusReason.NoPrescription;
            }

            if (!HasAuthorization)
            {
                reason |= CaseStatusReason.AuthExpired;
            }

            StatusReason = reason;

            if (reason > CaseStatusReason.NotSet)
            {
                Status = CaseStatus.NotReady;
            }
            else
            {
                Status = CaseStatus.Good;
            }

        }

        bool hasAuthorization()
        {

            if (Authorizations == null || Authorizations.Count == 0)
            {
                return false;
            }
            else
            {

                var latestAuth = Authorizations.OrderByDescending(x => x.EndDate).FirstOrDefault();

                if (latestAuth == null)
                {
                    return false;
                }
                else
                {
                    if (latestAuth.EndDate < DateTime.Now.Date)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        bool hasSupervisor()
        {

            if (Providers != null && Providers.Count > 0)
            {
                CaseProvider bcba =
                    Providers.Where(x =>
                        x.Active == true &&
                        x.Supervisor == true
                    ).FirstOrDefault();

                return bcba == null ? false : true;

            }
            else
            {
                return false;
            }

        }

        public static string GetStatusDisplayString(CaseStatus status, CaseStatusReason reason)
        {

            switch (status)
            {
                case CaseStatus.ConsiderForDischarge:
                    return "CFD";
                case CaseStatus.Good:
                    return "Good";
                case CaseStatus.History:
                    return "History";
                case CaseStatus.NotReady:

                    string s = "";
                    if (reason.HasFlag(CaseStatusReason.AuthExpired))
                    {
                        s += ",Auth";
                    }
                    if (reason.HasFlag(CaseStatusReason.NoBCBA))
                    {
                        s += ",BCBA";
                    }
                    if (reason.HasFlag(CaseStatusReason.NoIntake))
                    {
                        s += ",Int";
                    }
                    if (reason.HasFlag(CaseStatusReason.NoPrescription))
                    {
                        s += ",Rx";
                    }

                    if (s.Length > 0)
                    {
                        s = "NG:" + s.Substring(1);
                    }

                    return s;

                default:
                    return "Unknown";
            }
        }

        string getStatusDisplay()
        {
            return GetStatusDisplayString(Status, StatusReason);
        }

    }

}
