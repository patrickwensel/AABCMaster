using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AABC.DomainServices.Sessions
{

    public static class SessionReportFormatterExtension
    {
        public static string FormatForReport(this SessionReport report)
        {
            return new ReportSessionReportFormatter(report).ToString();
        }
    }


    public class ReportSessionReportFormatter
    {

        public SessionReport Report { get; private set; }

        public ReportSessionReportFormatter(SessionReport report)
        {
            Report = report;
        }

        public override string ToString()
        {
            var lineBreak = "\r\n";
            var list = new List<string>();
            if (!string.IsNullOrEmpty(Report.Summary))
            {
                list.Add($"Summary:{lineBreak}{Report.Summary}{lineBreak}");
            }
            var sections = new List<ReportSection>() { Report.BehaviorsSection, Report.InterventionsSection, Report.ReinforcersSection, Report.GoalsSection, Report.BarriersSection };
            foreach (var s in sections)
            {
                if (SectionHasElements(s))
                {
                    var f = FormatSection(s);
                    if (!string.IsNullOrEmpty(f))
                    {
                        list.Add(f + lineBreak);
                    }
                }
            }
            return string.Join(lineBreak, list).TrimEnd(lineBreak.ToCharArray());
        }

        private static bool SectionHasElements(ReportSection s)
        {
            return s != null && s.GetItems().Count() > 0;
        }

        private static string FormatSection(ReportSection s)
        {
            var list = new List<string> { GetElementName(s) + ":" };
            foreach (var e in s.GetItems())
            {
                list.Add("-  " + e.ToString());
            }
            return string.Join("\r\n", list);
        }

        private static string GetElementName(ReportSection s)
        {
            var type = s.GetItemType();
            return type.GetCustomAttributes(typeof(DisplayNameAttribute), false).SingleOrDefault() is DisplayNameAttribute attribute && !string.IsNullOrEmpty(attribute.DisplayName) ? attribute.DisplayName : type.Name;
        }
    }

}
