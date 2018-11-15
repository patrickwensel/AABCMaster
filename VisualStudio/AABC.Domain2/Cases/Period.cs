using AABC.Domain2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain2.Cases
{

    /// <summary>
    /// Represents a monthly period and hours/approvals etc within the period
    /// </summary>
    public class Period
    {
        
        public Cases.Case Case { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }

        public DateTime StartDate { get { return new DateTime(Year, Month, 1); } }
        public DateTime EndDate { get { return StartDate.AddMonths(1).AddDays(-1); } }

        public List<Hours.Hours> Hours { get; private set; }
        public bool HasParentApproval { get; set; }
        public bool HasParentApprovalEligibility { get { return calculateParentApprovalEligibility(); } }

        public Period(int year, int month, Cases.Case @case) {
            Case = @case;
            Year = year;
            Month = month;
            Hours = Case.Hours.Where(x => x.Date >= StartDate && x.Date <= EndDate).ToList();
        }


        private bool calculateParentApprovalEligibility() {

            // we should have some hours in order to be eligible
            if (Hours == null || Hours.Count == 0) {
                return false;
            }

            // if we're already approved, we're not eligible
            if (this.HasParentApproval) {
                return false;
            }



            /* ******************
             *  Client will test various methods before settling into one.
             *  Current (initial) eligibility is within 5th to 10th day after
             *  target period, as shown below
             *******************/
            // current eligibility rule: 5-10 days after EOM
            var currentDate = DateTimeService.Current.Now;
            
            // testDate is first day of previous month
            // it should match the startdate of this period, otherwise we're ineligible
            var testDate = currentDate.AddMonths(-1);
            testDate = new DateTime(testDate.Year, testDate.Month, 1);

            if (testDate != StartDate) {
                return false;
            }

            // current day should be 5-10 for eligibility
            if (currentDate.Day >= 5 && currentDate.Day <= 10) {
                return true;
            } else {
                return false;
            }
            
        }

    }
}
