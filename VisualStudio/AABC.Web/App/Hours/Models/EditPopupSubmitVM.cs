using System;
using System.Collections.Generic;

namespace AABC.Web.App.Hours.Models
{
    public class ExtendedNote
    {
        public int id { get; set; }
        public int templateId { get; set; }
        public string value { get; set; }
    }

    public class EditPopupSubmitVM
    {


        public int id { get; set; }
        public string billableHours { get; set; }
        public string billingRef { get; set; }
        public DateTime date { get; set; }
        public bool hasData { get; set; }
        public string notes { get; set; }
        public List<ExtendedNote> extendedNotes { get; set; }
        public string payableHours { get; set; }
        public int? serviceID { get; set; }
        public int? serviceLocationID { get; set; }
        public DateTime timeIn { get; set; }
        public DateTime timeOut { get; set; }
        public string totalHours { get; set; }
        public int? ssgParentID { get; set; }
        public string status { get; set; }

        public Domain.Cases.AuthorizationHoursStatus ConvertedStatus {
            get
            {
                switch (status) {
                    case nameof(Domain.Cases.AuthorizationHoursStatus.Pending):
                        return Domain.Cases.AuthorizationHoursStatus.Pending;

                    case nameof(Domain.Cases.AuthorizationHoursStatus.ComittedByProvider):
                        return Domain.Cases.AuthorizationHoursStatus.ComittedByProvider;

                    case nameof(Domain.Cases.AuthorizationHoursStatus.FinalizedByProvider):
                        return Domain.Cases.AuthorizationHoursStatus.FinalizedByProvider;

                    case nameof(Domain.Cases.AuthorizationHoursStatus.ScrubbedByAdmin):
                        return Domain.Cases.AuthorizationHoursStatus.ScrubbedByAdmin;

                    default:
                        throw new ArgumentException("Status '" + status + "' not registered");
                }
            }
        }

    }
}