using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.Hours.Models
{
    public class EditPopupVM
    {


        internal void LoadFromDomainHours(Domain2.Hours.Hours hours)
        {
            this.Status = hours.Status;
            this.BillableHours = hours.BillableHours;
            this.BillingRef = hours.BillingRef;
            this.CaseID = hours.Case.ID;
            this.Date = hours.Date;
            this.DateCreated = hours.DateCreated;
            this.HasCatalystData = hours.HasCatalystData;
            this.ID = hours.ID;
            this.Notes = hours.Memo;
            this.PatientName = hours.Case.Patient.CommonName;
            this.PayableHours = hours.PayableHours;
            this.ProviderID = hours.ProviderID;
            this.ProviderName = Helpers.CommonListItems.GetCommonName(hours.Provider.FirstName, hours.Provider.LastName);
            this.ServiceID = hours.Service.ID;
            this.ServiceLocationID = hours.ServiceLocationID;
            this.SSGParentID = hours.SSGParentID;
            this.TimeIn = hours.Date + hours.StartTime;
            this.TimeOut = hours.Date + hours.EndTime;
            this.TotalHours = hours.TotalHours;
            this.Authorizations = String.Join(", ", hours.AuthorizationBreakdowns.Select(x => x.Authorization.AuthorizationCode.Code));

            if (hours.Provider.ProviderType.ID == 15)
            {
                this.ExtendedNotes = new AABC.DomainServices.Hours.HoursMultiNotes().GetExistingNotesWithTemplateMerge(15, hours.ID);
            }


        }


        public AuthListItem Auth;
        public string Authorizations { get; set; }
        public int ID { get; set; }
        public Domain2.Hours.HoursStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public int? CaseAuthID { get; set; }
        public int ProviderID { get; set; }
        public string ProviderName { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public decimal TotalHours { get; set; }
        public int? ServiceID { get; set; }
        public string Notes { get; set; }
        public Dictionary<string, List<Domain.Hours.Note>> ExtendedNotes { get; set; }
        public int CaseID { get; set; }
        public string PatientName { get; set; }
        public decimal? BillableHours { get; set; }
        public string BillingRef { get; set; }
        public decimal? PayableHours { get; set; }
        public bool HasCatalystData { get; set; }
        public int? SSGParentID { get; set; }
        public int? ServiceLocationID { get; set; }


        public List<ServiceListItem> ServicesList = new List<ServiceListItem>();
        public List<ServiceLocationListItem> ServiceLocationsList = new List<ServiceLocationListItem>();


        public class AuthListItem
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
        }

        public class ServiceListItem
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class ServiceLocationListItem
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
    }
}