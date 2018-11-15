using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AABC.Web.Models.ExternalData.Catalyst
{
    public class StudentAttendanceImportResults
    {

        public List<StudentAttendanceSuccessItem> SuccessItems { get; set; }
        public List<StudentAttendanceFailureItem> FailureItems { get; set; }

        public StudentAttendanceImportResults() {
            SuccessItems = new List<StudentAttendanceSuccessItem>();
            FailureItems = new List<StudentAttendanceFailureItem>();
        }

        public StudentAttendanceImportResults(IEnumerable<DomainServices.Integrations.Catalyst.HasDataResultItem> results) {

            SuccessItems = new List<StudentAttendanceSuccessItem>();
            FailureItems = new List<StudentAttendanceFailureItem>();

            var successes = results.Where(x => x.Result == "Success");
            foreach (var s in successes) {
                SuccessItems.Add(new StudentAttendanceSuccessItem() {
                    Date = s.VisitDate.Value,
                    PatientName = s.StudentName,
                    ProviderInitials = s.ProviderInitials
                });
            }

            var failures = results.Where(x => x.Result != "Success");
            foreach(var f in failures) {
                FailureItems.Add(new StudentAttendanceFailureItem() {
                    Date = f.VisitDate,
                    ErrorMessage = f.Result,
                    PatientName = f.StudentName,
                    ProviderInitials = f.ProviderInitials
                });
            }
        }

        public StudentAttendanceImportResults(DataTable results) {

            SuccessItems = new List<StudentAttendanceSuccessItem>();
            FailureItems = new List<StudentAttendanceFailureItem>();

            fillSuccessItems(results);
            fillFailureItems(results);
        }

        void fillSuccessItems(DataTable results) {
        
            foreach (DataRow row in results.Rows) {
                
                if (row.ToIntOrNull("HoursID") != null) {

                    var item = new StudentAttendanceSuccessItem();
                    item.PatientName = row.ToStringValue("PatientName");
                    item.Date = row.ToDateTimeOrNull("HoursDate");
                    item.ProviderInitials = row.ToStringValue("ProviderName");

                    SuccessItems.Add(item);

                }
            }
        }

        void fillFailureItems(DataTable results) {

            foreach (DataRow row in results.Rows) {

                if (row.ToIntOrNull("HoursID") == null) {

                    var item = new StudentAttendanceFailureItem();

                    item.PatientName = row.ToStringValue("PatientName");
                    item.Date = row.ToDateTimeOrNull("HoursDate");
                    item.ProviderInitials = row.ToStringValue("ProviderName");
                    item.ErrorMessage = row.ToStringValue("Error");

                    FailureItems.Add(item);

                }
            }
        }
    }



    public class StudentAttendanceSuccessItem
    {
        public string PatientName { get; set; }
        public DateTime? Date { get; set; }
        public string ProviderInitials { get; set; }
    }

    public class StudentAttendanceFailureItem
    {
        public string PatientName { get; set; }
        public DateTime? Date { get; set; }
        public string ProviderInitials { get; set; }
        public string ErrorMessage { get; set; }
    }

}