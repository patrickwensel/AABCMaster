using AABC.Domain2.Patients;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Patients
{
    public class PatientService
    {
        public PatientEx Get(int Id)
        {
            var patient = _context.Patients.Where(l => l.ID == Id).FirstOrDefault();
            return Transform(patient);
        }
        public List<PatientEx> GetAll()
        {
            var patients = _context.Patients;
            return Transform(patients);
        }
        public List<PatientEx> GetPatientsExceedingMaxHoursPerDay(DateTime period, int Limit = 5)
        {
            DateTime StartDate = new DateTime(period.Year, period.Month, 1);
            DateTime EndDate = StartDate.AddMonths(1).AddDays(-1);
            var aPatients = _context.Database.SqlQuery<PatientEx>("GetPatientsExceedingDailyLimit {0}, {1}, {2}", Limit, StartDate, EndDate);
            return aPatients.ToList();
        }

        public void Save(PatientEx patient)
        {
            Patient m = Transform(patient);
            if (m.ID == 0)
            {
                _context.Patients.Add(m);
            }
            _context.SaveChanges();
            patient.ID = m.ID;
        }

        private List<PatientEx> Transform(IQueryable<Patient> n)
        {
            List<PatientEx> r = new List<PatientEx>();
            foreach (var a in n)
            {
                r.Add(Transform(a));
            }
            return r;
        }
        private PatientEx Transform(Patient n)
        {
            PatientEx r = new PatientEx();
            r.ID = n.ID;
            r.Address1 = n.Address1;
            r.Address2 = n.Address2;
            r.City = n.City;
            r.DateCreated = n.DateCreated;
            r.DateOfBirth = n.DateOfBirth;
            r.Email = n.Email;
            r.FirstName = n.FirstName;
            r.Gender = n.Gender;
            r.GeneratingReferralID = n.GeneratingReferralID;
            r.Guardian2CellPhone = n.Guardian2CellPhone;
            r.Guardian2Email = n.Guardian2Email;
            r.Guardian2FirstName = n.Guardian2FirstName;
            r.Guardian2HomePhone = n.Guardian2HomePhone;
            r.Guardian2LastName = n.Guardian2LastName;
            r.Guardian2Notes = n.Guardian2Notes;
            r.Guardian2RelationshipID = n.Guardian2RelationshipID;
            r.Guardian2WorkPhone = n.Guardian2WorkPhone;
            r.Guardian3CellPhone = n.Guardian3CellPhone;
            r.Guardian3Email = n.Guardian3Email;
            r.Guardian3FirstName = n.Guardian3FirstName;
            r.Guardian3HomePhone = n.Guardian3HomePhone;
            r.Guardian3LastName = n.Guardian3LastName;
            r.Guardian3Notes = n.Guardian3Notes;
            r.Guardian3RelationshipID = n.Guardian3RelationshipID;
            r.Guardian3WorkPhone = n.Guardian3WorkPhone;
            r.GuardianCellPhone = n.GuardianCellPhone;
            r.GuardianEmail = n.GuardianEmail;
            r.GuardianFirstName = n.GuardianFirstName;
            r.GuardianHomePhone = n.GuardianHomePhone;
            r.GuardianLastName = n.GuardianLastName;
            r.GuardianNotes = n.GuardianNotes;
            r.GuardianRelationship = n.GuardianRelationship;
            r.GuardianRelationshipID = n.GuardianRelationshipID;
            r.GuardianWorkPhone = n.GuardianWorkPhone;
            r.ID = n.ID;
            r.InsuranceCompanyName = n.InsuranceCompanyName;
            r.InsuranceCompanyProviderPhone = n.InsuranceCompanyProviderPhone;
            r.InsuranceID = n.InsuranceID;
            r.InsuranceMemberID = n.InsuranceMemberID;
            r.InsurancePrimaryCardholderDateOfBirth = n.InsurancePrimaryCardholderDateOfBirth;
            r.LastName = n.LastName;
            r.Notes = n.Notes;
            r.Phone = n.Phone;
            r.Phone2 = n.Phone2;
            r.PhysicianAddress = n.PhysicianAddress;
            r.PhysicianContact = n.PhysicianContact;
            r.PhysicianEmail = n.PhysicianEmail;
            r.PhysicianFax = n.PhysicianFax;
            r.PhysicianName = n.PhysicianName;
            r.PhysicianNotes = n.PhysicianNotes;
            r.PhysicianPhone = n.PhysicianPhone;
            r.PrimarySpokenLangauge = n.PrimarySpokenLangauge;
            r.PrimarySpokenLanguageID = n.PrimarySpokenLanguageID;
            r.State = n.State;
            r.Zip = n.Zip;
            return r;
        }
        private Patient Transform(PatientEx n)
        {
            Patient r;
            if (n.ID > 0)
            {
                r = _context.Patients.Where(p => p.ID == n.ID).FirstOrDefault();
            }
            else
            {
                r = _context.Patients.Create();
            }
            r.ID = n.ID;
            r.Address1 = n.Address1;
            r.Address2 = n.Address2;
            r.City = n.City;
            r.DateCreated = n.DateCreated;
            r.DateOfBirth = n.DateOfBirth;
            r.Email = n.Email;
            r.FirstName = n.FirstName;
            r.Gender = n.Gender;
            r.GeneratingReferralID = n.GeneratingReferralID;
            r.Guardian2CellPhone = n.Guardian2CellPhone;
            r.Guardian2Email = n.Guardian2Email;
            r.Guardian2FirstName = n.Guardian2FirstName;
            r.Guardian2HomePhone = n.Guardian2HomePhone;
            r.Guardian2LastName = n.Guardian2LastName;
            r.Guardian2Notes = n.Guardian2Notes;
            r.Guardian2RelationshipID = n.Guardian2RelationshipID;
            r.Guardian2WorkPhone = n.Guardian2WorkPhone;
            r.Guardian3CellPhone = n.Guardian3CellPhone;
            r.Guardian3Email = n.Guardian3Email;
            r.Guardian3FirstName = n.Guardian3FirstName;
            r.Guardian3HomePhone = n.Guardian3HomePhone;
            r.Guardian3LastName = n.Guardian3LastName;
            r.Guardian3Notes = n.Guardian3Notes;
            r.Guardian3RelationshipID = n.Guardian3RelationshipID;
            r.Guardian3WorkPhone = n.Guardian3WorkPhone;
            r.GuardianCellPhone = n.GuardianCellPhone;
            r.GuardianEmail = n.GuardianEmail;
            r.GuardianFirstName = n.GuardianFirstName;
            r.GuardianHomePhone = n.GuardianHomePhone;
            r.GuardianLastName = n.GuardianLastName;
            r.GuardianNotes = n.GuardianNotes;
            r.GuardianRelationship = n.GuardianRelationship;
            r.GuardianRelationshipID = n.GuardianRelationshipID;
            r.GuardianWorkPhone = n.GuardianWorkPhone;
            r.ID = n.ID;
            r.InsuranceCompanyName = n.InsuranceCompanyName;
            r.InsuranceCompanyProviderPhone = n.InsuranceCompanyProviderPhone;
            r.InsuranceID = n.InsuranceID;
            r.InsuranceMemberID = n.InsuranceMemberID;
            r.InsurancePrimaryCardholderDateOfBirth = n.InsurancePrimaryCardholderDateOfBirth;
            r.LastName = n.LastName;
            r.Notes = n.Notes;
            r.Phone = n.Phone;
            r.Phone2 = n.Phone2;
            r.PhysicianAddress = n.PhysicianAddress;
            r.PhysicianContact = n.PhysicianContact;
            r.PhysicianEmail = n.PhysicianEmail;
            r.PhysicianFax = n.PhysicianFax;
            r.PhysicianName = n.PhysicianName;
            r.PhysicianNotes = n.PhysicianNotes;
            r.PhysicianPhone = n.PhysicianPhone;
            r.PrimarySpokenLangauge = n.PrimarySpokenLangauge;
            r.PrimarySpokenLanguageID = n.PrimarySpokenLanguageID;
            r.State = n.State;
            r.Zip = n.Zip;
            return r;
        }
        private Data.V2.CoreContext _context;

        public PatientService(Data.V2.CoreContext context)
        {
            _context = context;
        }

    }
}
