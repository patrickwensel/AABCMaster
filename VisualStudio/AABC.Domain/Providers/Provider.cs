using System;
using System.Collections.Generic;

namespace AABC.Domain.Providers
{
    [Obsolete]
    public class Provider
    {

        public int? ID { get; set; }
        public DateTime DateCreated { get; set; }

        [Obsolete]
        public bool Active { get; set; }
        public bool IsHired { get; set; }
        public DateTime? HireDate { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public char? Gender { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public decimal? HourlyRate { get; set; }
        public string Notes { get; set; }
        public string Availability { get; set; }

        public bool HasBackgroundCheck { get; set; }
        public bool HasReferences { get; set; }
        public bool HasResume { get; set; }
        public bool CanCall { get; set; }
        public bool CanReachByPhone { get; set; }
        public bool CanEmail { get; set; }

        public IEnumerable<DevExpress.Web.UploadedFile> ResumeFile { get; set; }
        public string ResumeLocation { get; set; }
        public string ResumeFileName { get; set; }

        public string DocumentStatus { get; set; }
        public string LBA { get; set; }
        public string NPI { get; set; }
        public string CertificationID { get; set; }
        public string CertificationState { get; set; }
        public DateTime? CertificationRenewalDate { get; set; }
        public DateTime? W9Date { get; set; }
        public string CAQH { get; set; }

        public string ProviderNumber { get; set; }

        public ProviderType Type { get; set; }
        public List<ServiceArea> ServiceAreas { get; set; }
        public List<ProviderLanguage> Languages { get; set; }
        public ProviderPortal.ProviderPortalUser PortalUser { get; set; }

        public int? PayrollID { get; set; }

        public string CommonName { get { return FirstName + " " + LastName; } }
        public string FormalName { get { return LastName + ", " + FirstName; } }

        public Provider()
        {
            DateCreated = DateTime.UtcNow;
            ServiceAreas = new List<ServiceArea>();
            Type = new ProviderType();
            Languages = new List<ProviderLanguage>();
        }

    }

    [Obsolete]
    public class ProviderType
    {
        public int? ID { get; set; }
        public DateTime DateCreated { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsOutsourced { get; set; }
        public bool CanSuperviseCase { get; set; }

        public List<Cases.Service> Services { get; set; }

        public ProviderType()
        {
            Services = new List<Cases.Service>();
        }

    }


    public class ServiceArea
    {
        public int? ID { get; set; }
        public string ZipCode { get; set; }
        public bool IsPrimary { get; set; }

        public ServiceArea()
        {
            IsPrimary = true;
        }
    }

    public class Aide : Provider
    {
        public string StrengthAreas { get; set; }
        public string WeaknessAreas { get; set; }
    }

    public class ProviderLanguage : General.GeneralLanguage
    {
        public int? ProviderLanguageID { get; set; }
        public DateTime ProviderLanguageDateCreated { get; set; }
    }


    public class ProviderSettings
    {


        const int AIDE_TYPE_ID = 17;    // TODO: refactor to settings db

        public static ProviderSettings _instance;

        public static ProviderSettings Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetDefaultInstance();
                }
                return _instance;
            }
        }

        private static ProviderSettings GetDefaultInstance()
        {

            var settings = new ProviderSettings();

            return settings;

        }

        public int AideTypeID { get { return AIDE_TYPE_ID; } }

    }

    public class PayrollHours
    {
        public int ID { get; set; }
        public int PayrollID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double TotalHours { get; set; }
        public DateTime FirstDayOfMonthOfPeriod { get; set; }
        public int EntriesMissingCatalystData { get; set; }
    }



}
