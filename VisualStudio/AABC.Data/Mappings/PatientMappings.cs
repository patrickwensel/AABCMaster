
using System;

namespace AABC.Data.Mappings
{
    public static class PatientMappings
    {


        public static Domain.Patients.Patient Patient(Models.Patient entity)
        {

            var p = new Domain.Patients.Patient
            {
                Address1 = entity.PatientAddress1,
                Address2 = entity.PatientAddress2,
                City = entity.PatientCity,
                DateCreated = entity.DateCreated,
                DateOfBirth = entity.PatientDateOfBirth,
                Email = entity.PatientEmail,
                FirstName = entity.PatientFirstName,
                Gender = !string.IsNullOrEmpty(entity.PatientGender) ? (Domain.Gender?)Enum.Parse(typeof(Domain.Gender), entity.PatientGender) : null,
                GeneratingReferralID = entity.PatientGeneratingReferralID,
                ID = entity.ID,
                InsuranceCompanyName = entity.PatientInsuranceCompanyName,
                InsuranceMemberID = entity.PatientInsuranceMemberID,
                InsurancePrimaryCardholderDOB = entity.PatientInsurancePrimaryCardholderDateOfBirth,
                InsuranceProviderPhone = entity.PatientInsuranceCompanyProviderPhone,
                LastName = entity.PatientLastName,
                Phone = entity.PatientPhone,
                State = entity.PatientState,
                Zip = entity.PatientZip,
                Notes = entity.PatientNotes,
                InsuranceID = entity.PatientInsuranceID,

                GuardianFirstName = entity.PatientGuardianFirstName,
                GuardianLastName = entity.PatientGuardianLastName,
                GuardianRelationship = entity.PatientGuardianRelationship,    // deprecated, use object instead
                GuardianEmail = entity.PatientGuardianEmail,
                GuardianCellPhone = entity.PatientGuardianCellPhone,
                GuardianHomePhone = entity.PatientGuardianHomePhone,
                GuardianWorkPhone = entity.PatientGuardianWorkPhone,
                GuardianNotes = entity.PatientGuardianNotes,

                Guardian2FirstName = entity.PatientGuardian2FirstName,
                Guardian2LastName = entity.PatientGuardian2LastName,
                Guardian2Email = entity.PatientGuardian2Email,
                Guardian2CellPhone = entity.PatientGuardian2CellPhone,
                Guardian2HomePhone = entity.PatientGuardian2HomePhone,
                Guardian2WorkPhone = entity.PatientGuardian2WorkPhone,
                Guardian2Notes = entity.PatientGuardian2Notes,

                Guardian3FirstName = entity.PatientGuardian3FirstName,
                Guardian3LastName = entity.PatientGuardian3LastName,
                Guardian3Email = entity.PatientGuardian3Email,
                Guardian3CellPhone = entity.PatientGuardian3CellPhone,
                Guardian3HomePhone = entity.PatientGuardian3HomePhone,
                Guardian3WorkPhone = entity.PatientGuardian3WorkPhone,
                Guardian3Notes = entity.PatientGuardian3Notes,

                PhysicianAddress = entity.PatientPhysicianAddress,
                PhysicianContact = entity.PatientPhysicianContact,
                PhysicianEmail = entity.PatientPhysicianEmail,
                PhysicianFax = entity.PatientPhysicianFax,
                PhysicianName = entity.PatientPhysicianName,
                PhysicianNotes = entity.PatientPhysicianNotes,
                PhysicianPhone = entity.PatientPhysicianPhone
            };


            return p;
        }

    }
}
