using AABC.Domain2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain2.Cases
{
    public class MonthlyPeriod
    {

        #region Fields/Properties/CTORs


        

        /**************************
         * 
         * CORE PROPERTIES (ORM stuff)
         * 
         **************************/
        public int ID { get; set; }
        
        public int CaseID { get; set; }
        public DateTime FirstDayOfMonth { get; set; }
        
        public virtual Case Case { get; set; }
        public virtual ICollection<ParentApproval> ParentApprovals { get; set; }
        public virtual ICollection<HoursFinalization> Finalizations { get; set; }



        /**************************
         * 
         * PROPERTIES (derived from core properties)
         * 
         **************************/
        public int Year { get { return FirstDayOfMonth.Year; } }
        public int Month { get { return FirstDayOfMonth.Month; } }
        public DateTime StartDate { get { return FirstDayOfMonth; } }
        public DateTime EndDate { get { return FirstDayOfMonth.AddMonths(1).AddDays(-1); } }

        /// <summary>
        /// All hours applicable to this Period
        /// </summary>
        public List<Hours.Hours> Hours {
            get
            {
                return Case.Hours
                    .Where(x =>
                        x.Date >= StartDate && x.Date <= EndDate)
                    .ToList();
            }
        }

        /// <summary>
        /// True there are no unapproved hours in the period
        /// </summary>
        public bool IsFullyParentApproved {
            get
            {
                int unapprovedHoursCount = Hours.Where(x => x.ParentApprovalID.HasValue == false).Count();
                return unapprovedHoursCount == 0 ? true : false;
            }
        }

        /// <summary>
        /// True if there are any unapproved hours in the period, but false if it's fully approved
        /// </summary>
        public bool IsPartiallyParentApproved {
            get
            {
                if (IsFullyParentApproved) {
                    return false;
                }
                int approvedHoursCount = Hours.Where(x => x.ParentApprovalID.HasValue).Count();
                return approvedHoursCount == 0 ? false : true;
            }
        }

        /// <summary>
        /// True if any hours are unapproved
        /// </summary>
        public bool HasPendingParentApprovals {
            get
            {
                var unapprovedHours = Hours.Where(x => x.ParentApprovalID.HasValue == false).Count();
                return unapprovedHours == 0 ? false : true;
            }
        }

        /// <summary>
        /// Calculates and determines whether the parent is able to approve hours for this period
        /// </summary>
        public bool IsParentApprovalEligible {
            get
            {

                return getParentApprovalEligibility();
            }
        }



        public bool IsProviderFinalized(int providerID) {

            if (Finalizations == null || Finalizations.Count == 0) {
                return false;
            }

            var providerFinalizations = Finalizations.Where(x => x.ProviderID == providerID).ToList();

            return providerFinalizations.Count == 0 ? false : true;
        }


        /**************************
         * 
         * CTOR/DTORs
         * 
         **************************/

        public MonthlyPeriod() {
            // for EF6/pre-existing
        }

        public MonthlyPeriod(int year, int month, Case @case) {
            // for new instantiations
            FirstDayOfMonth = new DateTime(year, month, 1);
            CaseID = @case.ID;
            Case = @case;
            ParentApprovals = new List<ParentApproval>();
        }

        #endregion



        #region Methods

        /**************************
         * 
         * PRIVATE METHODS
         * 
         **************************/

        private bool getParentApprovalEligibility() {

            /* ******************
             *  Client will test various methods before settling into one.
             *  Current (initial) eligibility is within 5th to 10th day after
             *  target period, as shown below
             *******************/

            if (!HasPendingParentApprovals) {
                return false;
            }


            if (checkOverrideAny()) {
                return true;
            }


            if (System.Configuration.ConfigurationManager.AppSettings["PatientPortalAllowApproveAny"] == "true") {
                return true;
            }
            
            if (checkWhitelistedIDs() == true) {
                return true;
            }

            /******* TEST 1 **********
             
                must be target month, must be between 5th and 10th day
             
             */

            var currentDate = DateTimeService.Current.Now;
            // testDate is first day of previous month
            // it should match the startdate of this period, otherwise we're ineligible
            var testDate = currentDate.AddMonths(-1);
            testDate = new DateTime(testDate.Year, testDate.Month, 1);

            if (testDate != StartDate) {
                return false;
            }

            // current day should be the 5th to 10th day
            if (currentDate.Day >= 5 && currentDate.Day <= 10) {
                return true;
            } else {
                return false;
            }

        }

        bool checkOverrideAny() {

            string setting = System.Configuration.ConfigurationManager.AppSettings["PatientPortalAllowApproveAny"];
            bool parsedValue = false;
            bool parsed = bool.TryParse(setting, out parsedValue);

            return parsedValue;
        }

        bool checkWhitelistedIDs() {
            
            string applicableIDs = null;

            try {
                applicableIDs =
                    System.Configuration.ConfigurationManager.AppSettings["PatientPortalWhitelistPatientIDApprovals"] as string;
            } catch {
                // if this isn't explicitly available just bail out saying there's no match
                return false;
            }

            if (string.IsNullOrWhiteSpace(applicableIDs)) {
                return false;
            }

            int[] ids = applicableIDs.Split(';').Select(x => Convert.ToInt32(x)).ToArray();

            return ids.Contains(this.Case.PatientID) ? true : false;

        }




        #endregion
    }
}
