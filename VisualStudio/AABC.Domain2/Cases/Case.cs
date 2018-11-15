using AABC.Domain2.Infrastructure;
using AABC.Domain2.Notes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AABC.Domain2.Cases
{

    public enum HoursEntryMode
    {
        ProviderEntry,
        ManagementEntry
    }


    public enum RestaffReason
    {
        [Description("[None]")]
        None = 0,
        [Description("SCA In Process")]
        SCAInProcess = 1,
        [Description("Need Rx")]
        NeedRx = 2,
        [Description("Need Auth")]
        NeedAuth = 3,
        [Description("Need Intake")]
        NeedIntake = 4,
        [Description("Start in Jan")]
        StartInJan = 5,
        [Description("Start in Feb")]
        StartInFeb = 6,
        [Description("OH: Insurance")]
        OHInsurance = 7,
        [Description("OH: Parent")]
        OHParent = 8,
        [Description("OH: Agency Review")]
        OHAgencyReview = 9,
        [Description("Restaff: Mom wants new Aide")]
        RestaffMomWantsNewAide = 10,
        [Description("Restaff: Mom wants new BCBA")]
        RestaffMomWantsNewBCBA = 11,
        [Description("Restaff: Provider needs replacement")]
        RestaffProviderNeedsReplacement = 12,
        [Description("Other")]
        Other = 13,
        [Description("New - Needs ABA Aide")]
        NewNeedsAide = 14,
        [Description("New - Needs BCBA")]
        NewNeedsBCBA = 15,
        [Description("New - Needs BCBA & Aide")]
        NewNeedsBCBAAndAide = 16,
        [Description("Start in June")]
        StartInJune = 17,
        [Description("Start in July")]
        StartInJuly = 18,
        [Description("Start in Aug")]
        StartInAug = 19,
        [Description("Start in Sept")]
        StartInSept = 20,
        [Description("Start in Oct")]
        StartInOct = 21,
        [Description("Start in Nov")]
        StartInNov = 22,
        [Description("Start in Dec")]
        StartInDec = 23,
        [Description("Start in March")]
        StartInMarch = 24,
        [Description("Start in April")]
        StartInApril = 25,
        [Description("Start in May")]
        StartInMay = 26,
        [Description("Atidaynu")]
        Atidaynu = 27,
        [Description("Yaldenu")]
        Yaldenu = 28
    }

    public class Case
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public int PatientID { get; set; }
        public int? GeneratingReferralID { get; set; }
        public CaseStatus Status { get; set; }
        public string StatusNotes { get; set; }
        public DateTime? StartDate { get; set; }
        public int? AssignedStaffID { get; set; }
        public string RequiredHoursNotes { get; set; }
        public string RequiredServicesNotes { get; set; }
        public bool HasPrescription { get; set; }
        public bool HasAssessment { get; set; }
        public bool HasIntake { get; set; }
        public CaseStatusReason StatusReason { get; set; }
        public string DischargeNotes { get; set; }
        public int? DefaultServiceLocationID { get; set; }
        public bool NeedsRestaffing { get; set; }
        public string RestaffingReason { get; set; }
        public bool NeedsStaffing { get; set; }
        public int? RestaffReasonID { get; set; }
        public int? FunctioningLevelID { get; set; }

        public virtual FunctioningLevel FunctioningLevel { get; set; }
        public virtual Patients.Patient Patient { get; set; }
        public virtual ICollection<Authorizations.Authorization> Authorizations { get; set; } = new List<Authorizations.Authorization>();
        public virtual ICollection<MonthlyPeriod> Periods { get; set; } = new List<MonthlyPeriod>();
        public virtual ICollection<Hours.Hours> Hours { get; set; } = new List<Hours.Hours>();
        public virtual ICollection<CaseProvider> Providers { get; set; } = new List<CaseProvider>();
        public virtual ICollection<CaseInsurance> Insurances { get; set; } = new List<CaseInsurance>();
        public virtual ICollection<CasePaymentPlan> PaymentPlans { get; set; } = new List<CasePaymentPlan>();
        public virtual ICollection<CaseBillingCorrespondence> CaseBillingCorrespondences { get; set; } = new List<CaseBillingCorrespondence>();
        public virtual ICollection<CaseNote> Notes { get; set; } = new List<CaseNote>();


        public static IEnumerable<Tuple<RestaffReason, string>> GetRestaffReasonsList()
        {

            var items = new List<Tuple<RestaffReason, string>>
            {
                new Tuple<RestaffReason, string>(RestaffReason.None, "[None]"),
                new Tuple<RestaffReason, string>(RestaffReason.SCAInProcess, "SCA In Process"),
                new Tuple<RestaffReason, string>(RestaffReason.NeedRx, "Need Rx"),
                new Tuple<RestaffReason, string>(RestaffReason.NeedAuth, "Need Auth"),
                new Tuple<RestaffReason, string>(RestaffReason.NeedIntake, "Need Intake"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInJan, "Start in Jan"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInFeb, "Start in Feb"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInJune, "Start in June"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInJuly, "Start in July"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInAug, "Start in Aug"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInSept, "Start in Sept"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInOct, "Start in Oct"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInNov, "Start in Nov"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInDec, "Start in Dec"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInMarch, "Start in March"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInApril, "Start in April"),
                new Tuple<RestaffReason, string>(RestaffReason.StartInMay, "Start in May"),
                new Tuple<RestaffReason, string>(RestaffReason.OHInsurance, "OH: Insurance"),
                new Tuple<RestaffReason, string>(RestaffReason.OHParent, "OH: Parent"),
                new Tuple<RestaffReason, string>(RestaffReason.OHAgencyReview, "OH: Agency Review"),
                new Tuple<RestaffReason, string>(RestaffReason.NewNeedsAide, "New - Needs ABA Aide"),
                new Tuple<RestaffReason, string>(RestaffReason.NewNeedsBCBA, "New - Needs BCBA"),
                new Tuple<RestaffReason, string>(RestaffReason.NewNeedsBCBAAndAide, "New - Needs BCBA & Aide"),
                new Tuple<RestaffReason, string>(RestaffReason.RestaffMomWantsNewAide, "Restaff: Mom wants new Aide"),
                new Tuple<RestaffReason, string>(RestaffReason.RestaffMomWantsNewBCBA, "Restaff: Mom wants new BCBA"),
                new Tuple<RestaffReason, string>(RestaffReason.RestaffProviderNeedsReplacement, "Restaff: Provider needs replacement"),
                new Tuple<RestaffReason, string>(RestaffReason.Other, "Other"),
                new Tuple<RestaffReason, string>(RestaffReason.Atidaynu, "Atidaynu"),
                new Tuple<RestaffReason, string>(RestaffReason.Yaldenu, "Yaldenu")
            };

            return items;
        }


        public List<Hours.Hours> GetAllHoursOfMonth(DateTime refDate)
        {
            if (Hours == null || Hours.Count == 0)
            {
                return new List<Hours.Hours>();
            }
            DateTime firstDayOfMonth = new DateTime(refDate.Year, refDate.Month, 1, 0, 0, 0);
            DateTime firstOfNextMonth = firstDayOfMonth.AddMonths(1);
            return Hours.Where(x => x.Date >= firstDayOfMonth && x.Date < firstOfNextMonth).ToList();
        }


        public List<Hours.Hours> GetPrecheckedSessions()
        {
            return Hours.Where(x => x.Status == Domain2.Hours.HoursStatus.PreChecked).ToList();
        }


        /// <summary>
        /// Get all of the Authorizations that are currently active
        /// </summary>
        public List<Authorizations.Authorization> GetActiveAuthorizations()
        {
            var refDate = DateTimeService.Current.Now;
            return GetActiveAuthorizations(refDate);
        }


        /// <summary>
        /// Get all of the non-BCBA Authorizations that are currently active
        /// </summary>
        public List<Authorizations.Authorization> GetActiveNonBCBAAuthorizations()
        {
            var refDate = DateTimeService.Current.Now;
            return GetActiveNonBCBAAuthorizations(refDate);
        }


        /// <summary>
        /// Get all of the Authorizations that were active at the specified reference date
        /// </summary>
        public List<Authorizations.Authorization> GetActiveAuthorizations(DateTime refDate)
        {
            if (Authorizations == null || Authorizations.Count == 0)
            {
                return new List<Authorizations.Authorization>();
            }
            var auths =
                Authorizations
                    .Where(x =>
                        x.EndDate >= refDate.Date &&
                        x.StartDate <= refDate.Date)
                    .ToList();
            return auths;
        }


        /// <summary>
        /// Get all of the non-BCBA Authorizations that were active at the specified reference date
        /// </summary>
        public List<Authorizations.Authorization> GetActiveNonBCBAAuthorizations(DateTime refDate)
        {
            var auths =
                Authorizations
                    .Where(x =>
                        x.EndDate >= refDate &&
                        x.StartDate <= refDate &&
                        x.AuthorizationCode.Code == "GENERAL")
                    .ToList();
            return auths;
        }


        public List<CaseInsurance> GetActiveInsurance(DateTime StartDate, DateTime EndDate)
        {
            var insurances =
                Insurances
                    .Where(x =>
                        x.DatePlanEffective.GetValueOrDefault(DateTime.MinValue) <= EndDate &&
                        x.DatePlanTerminated.GetValueOrDefault(DateTime.MaxValue) >= StartDate)
                    .OrderBy(x => x.DatePlanTerminated)
                    .OrderByDescending(x => x.DatePlanEffective)
                    .ToList();
            return insurances;
        }


        public CaseInsurance GetActiveInsuranceAtDate(DateTime refDate)
        {
            var insurances =
                Insurances
                    .Where(x =>
                        x.DatePlanEffective.GetValueOrDefault(DateTime.MinValue) <= refDate &&
                        x.DatePlanTerminated.GetValueOrDefault(DateTime.MaxValue) >= refDate)
                    .OrderBy(x => x.DatePlanTerminated)
                    .OrderByDescending(x => x.DatePlanEffective)
                    .ToList();
            return insurances.FirstOrDefault();
        }


        public List<CaseProvider> GetAuthorizedBCBA(DateTime StartDate, DateTime EndDate)
        {
            var providers =
                Providers
                    .Where(x =>
                        x.StartDate.GetValueOrDefault(DateTime.MinValue) <= EndDate &&
                        x.EndDate.GetValueOrDefault(DateTime.MaxValue) >= StartDate &&
                        x.Active == true &&
                        x.IsAuthorizedBCBA == true)
                    .OrderBy(x => x.EndDate)
                    .OrderByDescending(x => x.StartDate)
                    .ToList();
            return providers;
        }


        /// <summary>
        /// Returns all MonthlyPeriods applicable to this case, whether or
        /// not they're persisted to the database
        /// </summary>
        /// <returns></returns>
        public List<MonthlyPeriod> GetAllPeriods(bool includeEmptyGaps = false)
        {
            if (Hours == null || Hours.Count == 0)
            {
                return null;
            }
            var minDate = Hours.Min(x => x.Date);
            var maxDate = Hours.Max(x => x.Date);
            // track a list of months that have any hours associated with them
            List<DateTime> dates = new List<DateTime>();
            var d = new DateTime(minDate.Year, minDate.Month, 1);
            dates.Add(d);
            d = d.AddMonths(1);
            while (d < maxDate)
            {
                if (includeEmptyGaps)
                {
                    dates.Add(d);
                }
                else
                {
                    if (Hours.Where(x => x.Date >= d && x.Date <= d.AddMonths(1).AddDays(-1)).Count() > 0)
                    {
                        dates.Add(d);
                    }
                }
                d = d.AddMonths(1);
            }
            var periods = new List<MonthlyPeriod>();
            // now get the periods for those months
            foreach (var date in dates)
            {
                periods.Add(GetPeriod(date.Year, date.Month));
            }
            return periods;
        }


        public MonthlyPeriod GetPeriod(int year, int month)
        {
            var period = Periods.Where(x => x.FirstDayOfMonth == new DateTime(year, month, 1)).SingleOrDefault();
            if (period == null)
            {
                period = new MonthlyPeriod(year, month, this);
                Periods.Add(period);
            }
            return period;
        }


        public CaseProvider GetAssessorAtDate(DateTime refDate)
        {
            // using lastOrDefault to help account for legacy possible duplicate per date condition
            // (assumes inherent sort based on order of entry to database)
            return GetProvidersAtDate(refDate).Where(x => x.IsAssessor).LastOrDefault();
        }

        public CaseProvider GetSupervisorAtDate(DateTime refDate)
        {
            // using lastOrDefault to help account for legacy possible duplicate per date condition
            // (assumes inherent sort based on order of entry to database)
            return GetProvidersAtDate(refDate).Where(x => x.IsSupervisor).LastOrDefault();
        }

        public CaseProvider GetAuthorizedBCBAAtDate(DateTime refDate)
        {
            // using lastOrDefault to help account for legacy possible duplicate per date condition
            // (assumes inherent sort based on order of entry to database)
            return GetProvidersAtDate(refDate).Where(x => x.IsAuthorizedBCBA).LastOrDefault();
        }



        /// <summary>
        /// Get a list of providers applicable at the specified date
        /// </summary>
        /// <param name="refDate"></param>
        /// <returns></returns>
        public List<CaseProvider> GetProvidersAtDate(DateTime refDate)
        {

            // providers that have a start and end date on file
            var explicitRangedProviders =
                Providers?.Where(x =>
                    x.StartDate <= refDate
                    && x.EndDate >= refDate);

            // providers with neithor start nor end date
            var openStartAndEndDateProviders =
                Providers?.Where(x => x.StartDate == null && x.EndDate == null);

            // providers with start date but no end date
            var openEndRangedProviders =
                Providers?.Where(x =>
                    x.EndDate == null
                    && x.StartDate <= refDate);

            // providers with end date but no start date
            var openStartRangedProviders =
                Providers?.Where(x =>
                    x.StartDate == null
                    && x.EndDate >= refDate);

            var results = new List<CaseProvider>();

            if (explicitRangedProviders != null)
            {
                results.AddRange(explicitRangedProviders);
            }

            if (openStartAndEndDateProviders != null)
            {
                results.AddRange(openStartAndEndDateProviders);
            }

            if (openEndRangedProviders != null)
            {
                results.AddRange(openEndRangedProviders);
            }

            if (openStartRangedProviders != null)
            {
                results.AddRange(openStartRangedProviders);
            }


            return results;
        }

    }
}
