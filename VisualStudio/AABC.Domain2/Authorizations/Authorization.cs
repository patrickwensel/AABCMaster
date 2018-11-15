using AABC.Domain2.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain2.Authorizations
{
    public class Authorization
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public int CaseID { get; set; }
        public int AuthorizationCodeID { get; set; }
        public int AuthorizationClassID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal HoursApproved { get; set; }

        public virtual Cases.Case Case { get; set; }
        public virtual AuthorizationCode AuthorizationCode { get; set; }
        public virtual AuthorizationClass AuthorizationClass { get; set; }

        //public virtual ICollection<Hours.Hours> Hours { get; set; }
        public virtual ICollection<Hours.AuthorizationBreakdown> AuthorizationBreakdowns { get; set; }

        public decimal WeeksTotal {
            get
            {
                if (StartDate > EndDate) {
                    throw new InvalidOperationException("Start Date cannot be greater than End Date");
                }

                var days = (StartDate - EndDate).TotalDays;
                return (decimal)(days / 7);
            }
        }

        public decimal WeeksRemaining {
            get
            {
                var date = DateTimeService.Current.Now;
                if (date >= EndDate) {
                    return 0;
                }
                if (date < StartDate) {
                    return 0;
                }
                var days = (EndDate - date).TotalDays;
                return (decimal)(days / 7);
            }
        }

        public decimal HoursApprovedPerWeek {
            get
            {
                if (WeeksTotal == 0) {
                    return 0;
                }
                return Math.Round((HoursApproved / WeeksTotal) * -1, 2);
            }
        }

        public decimal HoursUtilized {
            get
            {
                decimal minutes = AuthorizationBreakdowns.Sum(x => x.Minutes);
                return minutes / 60;   
            }
        }

        public decimal HoursRemaining {
            get
            {
                return HoursApproved - HoursUtilized;
            }
        }

        public decimal HoursOverage {
            get
            {
                if (HoursUtilized <= HoursApproved) {
                    return 0;
                } else {
                    return HoursUtilized - HoursApproved;
                }
            }
        }

        public decimal HoursRemainingPerWeek {
            get
            {
                if (WeeksRemaining == 0) {
                    return 0;
                } else {
                    return Math.Round(HoursRemaining / WeeksRemaining, 2);
                }

            }
        }

        public List<MonthlyPeriod> GetMonthlyPeriods() {

            var periods = new List<MonthlyPeriod>();
            var workingDate = new DateTime(StartDate.Year, StartDate.Month, 1);

            while (workingDate <= EndDate) {
                var period = new MonthlyPeriod(this, workingDate);
                periods.Add(period);
                workingDate = workingDate.AddMonths(1);
            }

            return periods;
        }
        
        
        public class MonthlyPeriod
        {

            Authorization authorization;

            public MonthlyPeriod(Authorization authorization, DateTime firstDayOfMonth) {
                this.authorization = authorization;
                this.Date = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.Month, 1);
            }

            public DateTime Date { get; private set; }  // first day of month

            public decimal HoursUtilized {
                get
                {

                    decimal minutes = authorization.AuthorizationBreakdowns
                        .Where(x => x.HoursEntry.Date >= Date && x.HoursEntry.Date <= Date.AddMonths(1).AddDays(-1))
                        .Sum(x => x.Minutes);

                    return minutes / 60;
                       
                }
            }
        }

    }
}
