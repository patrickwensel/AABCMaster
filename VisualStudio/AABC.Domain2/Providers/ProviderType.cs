using System;
using System.Collections.Generic;

namespace AABC.Domain2.Providers
{

    public enum ProviderTypeIDs
    {
        BoardCertifiedBehavioralAnalyst = 15,
        Aide = 17,
        OccupationalTherapist = 18,
        PhysicalTherapist = 19,
        SpeechTherapist = 20,
        Supervisor = 21,
        MastersDegree = 27,
        MentalHealthLicensedProvider = 28,
        Phsycologist = 29,
        SpecialEdicationItinerantTeacher = 30,
        SocialWorker = 31,
        CaseManager = 32,
        OfficeStaff = 33
    }

    public class ProviderType
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsOutsourced { get; set; }
        public bool CanSupervise { get; set; }

        public virtual ICollection<ProviderTypeService> Services { get; set; }
        public virtual ICollection<Provider> Providers { get; set; }
        public virtual ICollection<Authorizations.AuthorizationMatchRule> AuthorizationMatchRules { get; set; }


    }
}
