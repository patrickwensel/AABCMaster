using AABC.Data.Models;
using AABC.Domain.Cases;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Data.Mappings
{
    public static class CaseMappings
    {
                 
        
        public static MonthlyCasePeriod MonthlyCasePeriod(Models.CaseMonthlyPeriod entity) {
            var item = new MonthlyCasePeriod();
            item.ID = entity.ID;
            item.FirstDayOfMonth = entity.PeriodFirstDayOfMonth;
            return item;
        }

        public static List<MonthlyCasePeriod> MonthlyCasePeriods(List<CaseMonthlyPeriod> entities) {
            var items = new List<MonthlyCasePeriod>();
            foreach (var entity in entities) {
                items.Add(MonthlyCasePeriod(entity));
            }
            return items;
        }
                       
        public static Domain.Cases.CaseProvider CaseProvider(Models.CaseProvider entity) {

            var item = new Domain.Cases.CaseProvider();

            item.Active = entity.Active;
            item.Address1 = entity.Provider.ProviderAddress1;
            item.Address2 = entity.Provider.ProviderAddress2;
            item.Assessor = entity.IsAssessor;
            item.Availability = entity.Provider.ProviderAvailability;
            item.CanCall = entity.Provider.ProviderCanCall;
            item.CanEmail = entity.Provider.ProviderCanEmail;
            item.CanReachByPhone = entity.Provider.ProviderCanReachByPhone;
            item.CAQH = entity.Provider.ProviderCAQH;
            item.CaseProviderID = entity.ID;
            item.CertificationID = entity.Provider.ProviderCertificationID;
            item.CertificationRenewalDate = entity.Provider.ProviderCertificationRenewalDate;
            item.CertificationState = entity.Provider.ProviderCertificationState;
            item.City = entity.Provider.ProviderCity;
            item.CompanyName = entity.Provider.ProviderCompanyName;
            item.DateCreated = entity.Provider.DateCreated;
            item.DocumentStatus = entity.Provider.ProviderDocumentStatus;
            item.Email = entity.Provider.ProviderPrimaryEmail;
            item.Fax = entity.Provider.ProviderFax;
            item.FirstName = entity.Provider.ProviderFirstName;
            item.HasBackgroundCheck = entity.Provider.ProviderHasBackgroundCheck;
            item.HasReferences = entity.Provider.ProviderHasReferences;
            item.HasResume = entity.Provider.ProviderHasResume;
            item.HourlyRate = entity.Provider.ProviderRate;
            item.ID = entity.Provider.ID;
            item.IsHired = entity.Provider.ProviderIsHired;
            //item.Languages = 
            item.LastName = entity.Provider.ProviderLastName;
            item.LBA = entity.Provider.ProviderLBA;
            item.Notes = entity.Provider.ProviderNotes;
            item.NPI = entity.Provider.ProviderNPI;
            item.Phone = entity.Provider.ProviderPrimaryPhone;
            item.Phone2 = entity.Provider.ProviderPhone2;
            item.State = entity.Provider.ProviderState;
            item.Supervisor = entity.IsSupervisor;
            item.Type = Mappings.ProviderMappings.ProviderType(entity.Provider.ProviderType1);
            item.W9Date = entity.Provider.ProviderW9Date;
            item.Zip = entity.Provider.ProviderZip;
            item.InsuranceAuthorizedBCBA = entity.IsInsuranceAuthorizedBCBA;
            item.StartDate = entity.ActiveStartDate;
            item.EndDate = entity.ActiveEndDate;
            
            return item;
        }

        public static Domain.Cases.Service Service(Models.Service entity) {

            var s = new Domain.Cases.Service();

            s.Code = entity.ServiceCode;
            s.DateCreated = entity.DateCreated;
            s.Description = entity.ServiceDescription;
            s.ID = entity.ID;
            s.Name = entity.ServiceName;

            return s;
            
        }

        public static Domain.Cases.Case Case(Models.Case entity, bool skipPatientMap = false) {

            var c = new Domain.Cases.Case();

            c.DateCreated = entity.DateCreated;
            c.GeneratingReferralID = entity.CaseGeneratingReferralID;
            c.HasAssessment = entity.CaseHasAssessment;
            c.HasIntake = entity.CaseHasIntake;
            c.HasPrescription = entity.CaseHasPrescription;
            c.ID = entity.ID;
            c.RequiredHoursNotes = entity.CaseRequiredHoursNotes;
            c.RequiredServicesNotes = entity.CaseRequiredServicesNotes;
            c.StartDate = entity.CaseStartDate;
            c.Status = (Domain.Cases.CaseStatus)entity.CaseStatus;
            c.StatusNotes = entity.CaseStatusNotes;
            c.StatusReason = (Domain.Cases.CaseStatusReason)entity.CaseStatusReason;
            c.DefaultServiceLocationID = entity.DefaultServiceLocationID;

            c.NeedsStaffing = entity.CaseNeedsStaffing;
            c.NeedsRestaffing = entity.CaseNeedsRestaffing;
            c.RestaffingReason = entity.CaseRestaffingReason;

            if (entity.DefaultServiceLocationID.HasValue) {
                c.DefaultServiceLocation = new Services.ServicesService().GetActiveServiceLocations().Where(x => x.ID == c.DefaultServiceLocationID).FirstOrDefault();
            }

            if (!skipPatientMap) {
                c.Patient = Mappings.PatientMappings.Patient(entity.Patient);
            }
            
            return c;

        }

        public static List<Domain.Cases.Service> Services(List<Models.Service> entities) {
            var services = new List<Domain.Cases.Service>();
            foreach (var entity in entities) {
                services.Add(CaseMappings.Service(entity));
            }
            return services;
        }

        public static IEnumerable<Domain.Cases.Case> Cases(IEnumerable<Models.Case> entities, bool skipPatientMap = false) {
            var list = new List<Domain.Cases.Case>();
            foreach (var entity in entities) {
                list.Add(CaseMappings.Case(entity, skipPatientMap));
            }
            return list;
        }

    }
}
