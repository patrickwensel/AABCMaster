using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain.Cases
{
    public class CaseAuthorization : Authorization
    {
        public int? CaseAuthorizationID { get; set; }
        public DateTime CaseAuthorizationDateCreated { get; set; }

        public AuthorizationClass AuthClass { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalHoursApproved { get; set; }

        public List<CaseAuthorizationHours> Hours { get; set; }
        public List<CaseAuthorizationGeneralHours> GeneralHours { get; set; }

        public double GeneralHoursRemaining { get { return calculateGeneralHoursRemaining(); } }
        public double AverageWeeklyHours { get { return calculateAverageWeeklyHours(); } }
        public double? AverageRemainingWeeklyHours { get { return calculateAverageRemainingWeeklyHours(); } }

        public CaseAuthorization() {
            CaseAuthorizationDateCreated = DateTime.UtcNow;
        }


        private double? calculateAverageRemainingWeeklyHours() {

            try {
                int days = 0;

                if (StartDate > DateTime.Now.Date) {
                    days = (EndDate - StartDate).Days;
                } else {
                    days = (EndDate - DateTime.Now.Date).Days;
                }

                if (days < 1) {
                    return null;
                }

                double d = GeneralHoursRemaining / days * 7;

                if (d < 0) {
                    return 0;
                } else {
                    return d;
                }

            }
            catch {
                return 0;
            }

        }

        private double calculateAverageWeeklyHours() {
            try {
                int days = (EndDate - StartDate).Days;
                return TotalHoursApproved / days * 7;
            }
            catch {
                return 0;
            }
        }

        private double calculateGeneralHoursRemaining() {
            if (GeneralHours == null) {
                return TotalHoursApproved;
            } else {
                return TotalHoursApproved - GeneralHours.Sum(x => x.Hours);
            }
        }
    }
}
