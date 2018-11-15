using AABC.Domain.Providers;
using AABC.Domain2.Providers;
using System.Collections.Generic;
using System.Linq;
using Provider = AABC.Domain.Providers.Provider;
using ProviderType = AABC.Domain.Providers.ProviderType;

namespace AABC.Data.Mappings
{
    public static class ProviderMappings
    {

        public static Provider Provider(Models.Provider entity) {

            var p = new Provider();

            p.Active = entity.ProviderStatus == (int) ProviderStatus.Active;
            p.IsHired = entity.ProviderIsHired;
            p.Address1 = entity.ProviderAddress1;
            p.Address2 = entity.ProviderAddress2;
            p.Availability = entity.ProviderAvailability;
            p.CanCall = entity.ProviderCanCall;
            p.CanEmail = entity.ProviderCanEmail;
            p.CanReachByPhone = entity.ProviderCanReachByPhone;
            p.CAQH = entity.ProviderCAQH;
            p.CertificationID = entity.ProviderCertificationID;
            p.CertificationRenewalDate = entity.ProviderCertificationRenewalDate;
            p.CertificationState = entity.ProviderCertificationState;
            p.City = entity.ProviderCity;
            p.CompanyName = entity.ProviderCompanyName;
            p.DateCreated = entity.DateCreated;
            p.DocumentStatus = entity.ProviderDocumentStatus;
            p.Email = entity.ProviderPrimaryEmail;
            p.Fax = entity.ProviderFax;
            p.FirstName = entity.ProviderFirstName;
            p.HasBackgroundCheck = entity.ProviderHasBackgroundCheck;
            p.HasReferences = entity.ProviderHasReferences;
            p.HasResume = entity.ProviderHasResume;
            p.HourlyRate = entity.ProviderRate;
            p.ID = entity.ID;
            p.LastName = entity.ProviderLastName;
            p.LBA = entity.ProviderLBA;
            p.Notes = entity.ProviderNotes;
            p.NPI = entity.ProviderNPI;
            p.Phone = entity.ProviderPrimaryPhone;
            p.Phone2 = entity.ProviderPhone2;
            p.State = entity.ProviderState;
            p.W9Date = entity.ProviderW9Date;
            p.Zip = entity.ProviderZip;
            p.ResumeFileName = entity.ResumeFileName;
            p.ResumeLocation = entity.ResumeLocation;

            p.ProviderNumber = entity.ProviderNumber;

            p.Type = ProviderMappings.ProviderType(entity.ProviderType1);
            p.ServiceAreas = ProviderMappings.ServiceAreas(entity.ProviderServiceZipCodes.ToList()).ToList();
            p.Languages = ProviderMappings.ProviderLanguages(entity.ProviderLanguages.ToList()).ToList();

            return p;

        }

        public static IEnumerable<ProviderLanguage> ProviderLanguages(IEnumerable<Models.ProviderLanguage> entities) {

            var models = new List<ProviderLanguage>();
            foreach (var entity in entities) {
                models.Add(ProviderMappings.ProviderLanguage(entity));
            }
            return models;

        }

        public static ProviderLanguage ProviderLanguage(Models.ProviderLanguage entity) {

            var model = new ProviderLanguage();
            var commonLanguage = GeneralMappings.GeneralLanguage(entity.ISO639_2_Lang);

            model.ProviderLanguageID = entity.ID;
            model.ProviderLanguageDateCreated = entity.DateCreated;
            model.ID = entity.LanguageID;
            model.Code = commonLanguage.Code;
            model.Description = commonLanguage.Description;

            return model;
        }

        public static IEnumerable<Provider> Providers(IEnumerable<Models.Provider> entities) {

            var providers = new List<Provider>();

            foreach (var entity in entities) {
                providers.Add(ProviderMappings.Provider(entity));
            }

            return providers;

        }



        public static IEnumerable<ServiceArea> ServiceAreas(IEnumerable<Models.ProviderServiceZipCode> entities) {

            List<ServiceArea> serviceAreas = new List<ServiceArea>();
            foreach (var entity in entities) {
                serviceAreas.Add(new ServiceArea()
                {
                    ID = entity.ID,
                    IsPrimary = entity.IsPrimary,
                    ZipCode = entity.ZipCode
                });
            }
            return serviceAreas;
        }



        public static ProviderType ProviderType(Models.ProviderType entity) {
            
            if (entity == null) {
                return null;
            }

            var pt = new ProviderType();
            pt.CanSuperviseCase = entity.ProviderTypeCanSuperviseCase;
            pt.Code = entity.ProviderTypeCode;
            pt.DateCreated = entity.DateCreated;
            pt.ID = entity.ID;
            pt.IsOutsourced = entity.ProviderTypeIsOutsourced;
            pt.Name = entity.ProviderTypeName;

            return pt;

        }


    }
}
