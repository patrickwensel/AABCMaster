using System;
using System.Collections.Generic;

namespace AABC.Domain2.Services
{

    // Fixed services are those that existed prior to the dynamic service management
    // system, and may have hardcoded values on their Code or ID.  Thus, fixed services
    // cannot be edited by the user.

    // FOR FIXED SERVICES ONLY
    public enum ServiceIDs
    {
        DirectCare = 9,
        ParentTraining = 10,
        InitialAssessment = 11,
        TreatmentPlanning = 12,
        DirectSupervision = 13,
        SocialSkillsGroup = 14,
        SupervisionReceived = 15,
        TeamMeeting = 16,
        Assessment = 17,
        FollowupAssessment = 18
    }

    // see dbo.ServiceTypeEnum for db version
    // see /ProviderPortal/Scripts/Pages/HoursEntry for client-side version
    public enum ServiceTypes
    {
        General = 0,
        Assessment = 1,
        Care = 2,
        Social = 3,
        Supervision = 4,
        Management = 5
    }


    public class Service
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsFixed { get; set; }
        public ServiceTypes Type { get; set; }

        public virtual ICollection<Authorizations.AuthorizationMatchRule> AuthorizationMatchRules { get; set; } = new List<Authorizations.AuthorizationMatchRule>();
        public virtual ICollection<Providers.ProviderTypeService> ProviderTypeServices { get; set; } = new List<Providers.ProviderTypeService>();
        public virtual ICollection<Insurances.InsuranceService> Insurances { get; set; } = new List<Insurances.InsuranceService>();
    }




}
