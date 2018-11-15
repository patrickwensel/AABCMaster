using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.Models.Patients
{
    public class PatientVM : Domain.Patients.Patient
    {

        public IViewModelBase Base { get; set; }
        public WebViewHelper ViewHelper { get; set; }
        public int? CaseId { get; set; }

        public PatientVM()
        {
            ViewHelper = new WebViewHelper(this);
        }

        public class WebViewHelper : IWebViewHelper
        {
            public List<Domain.General.GuardianRelationship> GuardianRelationships { get; set; }
            public bool IsNewPatientEntry { get; set; }
            public bool HasValidationErrors { get; set; }
            PatientVM parent;
            public List<Domain.General.State> StatesList { get; set; }
            public List<InsuranceListItem> Insurances { get; set; }

            public WebViewHelper(PatientVM parent)
            {
                this.parent = parent;
                StatesList = Domain.General.State.GetStates();
            }

            public string ReturnErrorMessage { get; set; }

            public void BindModel()
            {

            }

            public bool Validate()
            {
                String[] validTypes = { ".pdf", ".txt", ".rtf", ".doc", ".docx" };
                if (string.IsNullOrEmpty(parent.FirstName) || string.IsNullOrEmpty(parent.LastName))
                {
                    return false;
                }
                if (parent.DateOfBirth != null)
                {
                    if (parent.DateOfBirth > DateTime.Now)
                    {
                        return false;
                    }
                }
                if (parent.InsurancePrimaryCardholderDOB != null)
                {
                    if (parent.InsurancePrimaryCardholderDOB > DateTime.Now)
                    {
                        return false;
                    }
                }
                if (parent.PrescriptionFile.Count() > 0)
                {
                    string fileName = parent.PrescriptionFile.First().FileName;
                    if (fileName != "")
                    {
                        string extension = fileName.Substring(fileName.LastIndexOf('.'));
                        if (!validTypes.Contains(extension))
                        {
                            parent.PrescriptionFile.First().IsValid = false;
                            return false;
                        }
                    }
                }

                var repo = new Repositories.PatientRepository();
                if (!parent.ID.HasValue)
                {
                    if (repo.DuplicateExists(parent.FirstName, parent.LastName, parent.DateOfBirth.Value))
                    {
                        ReturnErrorMessage = "A patient already exists with this name and date of birth.";
                        HasValidationErrors = true;
                        return false;
                    }
                }
                return true;
            }
        }
    }
}