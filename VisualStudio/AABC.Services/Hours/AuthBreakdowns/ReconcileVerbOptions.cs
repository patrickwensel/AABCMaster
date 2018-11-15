using CommandLine;
using System;

namespace AABC.Services.Hours.AuthBreakdowns
{
    public class ReconcileVerbOptions : VerbOptionsBase
    {
        

        [Option('b', "break", Required = false)]
        public bool BreakOnExceptionBypass { get; set; }

        [Option('s', "start", Required = false)]
        public string StartDate { get; set; }

        [Option('e', "end", Required = false)]
        public string EndDate { get; set; }

        public override bool ValidateInput() {

            // try to parse the dates
            DateTime refDate = DateTime.Now;
            bool parsed = false;

            DateTime? testStart = null;
            DateTime? testEnd = null;

            if (StartDate != null) {
                parsed = DateTime.TryParse(StartDate, out refDate);
                if (!parsed) {
                    return false;
                }
                testStart = refDate;
            }

            if (EndDate != null) {
                parsed = DateTime.TryParse(EndDate, out refDate);
                if (!parsed) {
                    return false;
                }
                testEnd = refDate;
            }

            return isValidDateOverlap(testStart, testEnd);
        }


        bool isValidDateOverlap(DateTime? start, DateTime? end) {

            if (start == null && end == null) {
                return true;
            }

            if (start == null && end != null) {
                return true;
            }

            if (end == null && start != null) {
                return true;
            }

            if (start.Value <= end.Value) {
                return true;
            }

            return false;
        }

        public override object Transform() {

            var ret = new ReconcileOptions();

            if (StartDate == null) {
                ret.StartDate = new DateTime(1901, 1, 1);
            } else {
                ret.StartDate = DateTime.Parse(StartDate);
            }

            if (EndDate == null) {
                ret.EndDate = new DateTime(2090, 1, 1);
            } else {
                ret.EndDate = DateTime.Parse(EndDate);
            }

            ret.BreakOnExceptionBypassed = this.BreakOnExceptionBypass;
            
            return ret;
        }

    }
}
