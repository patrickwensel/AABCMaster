using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Collections.Generic;

namespace AABC.Web.App.Staffing.Models
{
    public class StaffingLogVM
    {
        public IViewModelBase Base { get; set; }
        public WebViewHelper ViewHelper { get; set; }

        public int PatientID { get; set; }
        public int CaseID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }
        public string FunctioningLevel { get; set; }
        public string Notes { get; set; }
        public IEnumerable<GuardianInfoVM> Guardians { get; set; }
        public IEnumerable<CaseProviderVM> CaseProviders { get; set; }
        public string ParentalRestaffRequest { get; set; }
        public decimal? HoursOfABATherapy { get; set; }
        public string AidesRespondingNo { get; set; }
        public string AidesRespondingMaybe { get; set; }
        public ScheduleRequestVM ScheduleRequest { get; set; }
        public DateTime? DateWentToRestaff { get; set; }
        public List<SpecialAttentionNeedVM> SystemSpecialAttentionNeeds { get; set; }
        public char? ProviderGenderPreference { get; set; }
        public List<int> SpecialAttentionNeedIDs { get; set; }


        public StaffingLogVM()
        {
            ViewHelper = new WebViewHelper(this);
        }


        public class WebViewHelper : IWebViewHelper
        {
            public bool HasValidationErrors { get; set; }
            public string ReturnErrorMessage { get; set; }
            private readonly StaffingLogVM parent;


            public WebViewHelper(StaffingLogVM parent)
            {
                this.parent = parent;
            }


            public void BindModel()
            {

            }


            public bool Validate()
            {
                return true;
            }
        }
    }

    public class GuardianInfoVM
    {
        public int Index { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Relationship { get; set; }
        public string Email { get; set; }
        public string CellPhone { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Notes { get; set; }
    }

    public class CaseProviderVM
    {
        public int ProviderID { get; set; }
        public string ProviderLastName { get; set; }
        public string ProviderFirstName { get; set; }
        public string ProviderType { get; set; }
        public bool IsBCBA { get; set; }
        public bool IsActive { get; set; }

        public string FullName
        {
            get { return ProviderFirstName + " " + ProviderLastName; }
        }
    }
}