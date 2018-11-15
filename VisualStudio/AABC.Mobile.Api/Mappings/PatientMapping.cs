using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AABC.Domain2.Patients;
using AABC.Mobile.SharedEntities.Entities;

namespace AABC.Mobile.Api.Mappings
{
    class PatientMapping : IDomainMapperReadonly<Domain2.Patients.Patient, SharedEntities.Entities.Patient>
    {
        

        public SharedEntities.Entities.Patient FromDomain(Domain2.Patients.Patient source) {

            var result = new SharedEntities.Entities.Patient();

            result.Gender = getGender(source.Gender);
            result.ID = source.ID;
            result.PatientAddress1 = source.Address1;
            result.PatientAddress2 = source.Address2;
            result.PatientCity = source.City;
            result.PatientFirstName = source.FirstName;
            result.PatientGuardian2FirstName = source.Guardian2FirstName;
            result.PatientGuardian2LastName = source.Guardian2LastName;
            result.PatientGuardian2Phone = source.Guardian2HomePhone;
            result.PatientGuardian2Relationship = getGuardianRelationship(source.Guardian2RelationshipID);
            result.PatientGuardianFirstName = source.GuardianFirstName;
            result.PatientGuardianLastName = source.GuardianLastName;
            result.PatientGuardianPhone = source.GuardianHomePhone;
            result.PatientGuardianRelationship = getGuardianRelationship(source.GuardianRelationshipID);
            result.PatientLastName = source.LastName;
            result.PatientState = source.State;
            result.PatientZip = source.Zip;

            return result;
        }


        private string getGuardianRelationship(int? relationshipID) {

            if (!relationshipID.HasValue) {
                return null;
            }

            // this is more or less hardcoded in the database (dbo.GuardianRelationships), should be handled better...
            switch (relationshipID.Value) {
                case 1: return "Father";
                case 3: return "Grandparent";
                case 2: return "Guardian";
                case 0: return "Mother";
                case 5: return "Other";
                case 4: return "Relative";
                default: return null;
            }            
        }

        private SharedEntities.Entities.Gender getGender(string input) {

            switch (input) {

                case "m":
                case "M":
                case "0":
                    return Gender.Male;
                case "f":
                case "F":
                case "1":
                    return Gender.Female;
                default:
                    return Gender.Unknown;
            }
            
        }

    }
}