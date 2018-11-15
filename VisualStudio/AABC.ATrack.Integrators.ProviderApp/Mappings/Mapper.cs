using AABC.Domain2.Hours;
using AABC.Shared.Web.App.HoursEntry.Models.Request;
using ATrack.Integrators.ProviderApp.Contracts.Entities;
using ATrack.Integrators.ProviderApp.Contracts.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.ATrack.Integrators.ProviderApp.Mappings
{

    static class Mapper
    {

        public static Case MapCase(Domain2.Cases.Case source)
        {
            if (source == null)
            {
                return null;
            }
            var result = new Case
            {
                ID = source.ID,
                Patient = MapPatient(source.Patient),
                ActiveAuthorizations = source.GetActiveAuthorizations().Select(MapAuthorization),
                ActiveInsurance = MapInsurance(source.GetActiveInsuranceAtDate(DateTime.Now)?.Insurance)
            };
            return result;
        }


        public static ValidatedSession MapSession(Domain2.Hours.Hours source, CaseValidationState state)
        {
            var session = new ValidatedSession
            {
                CaseID = source.CaseID,
                DateOfService = source.Date,
                Duration = source.EndTime - source.StartTime,
                LocationDescription = source.ServiceLocation.Name,
                LocationID = source.ServiceLocationID.Value,    // ensure all validated sessions have a Location
                ServerValidatedSessionID = source.ID,
                ServiceDescription = source.Service.Name,
                ServiceID = source.ServiceID.Value,
                SsgCaseIds = source.SSGCaseIDs == null ? string.Empty : string.Join(",", source.SSGCaseIDs),
                StartTime = source.StartTime,
                State = state
            };
            return session;
        }


        public static ValidatedSession MapSession(Domain2.Hours.Hours source)
        {
            return MapSession(source, CaseValidationState.None);
        }


        public static Location MapLocation(Domain2.Services.ServiceLocation source)
        {
            return new Location
            {
                ID = source.ID,
                Description = source.Name
            };
        }


        public static Service MapService(Domain2.Services.Service source, IEnumerable<Location> locations)
        {
            return new Service
            {
                ID = source.ID,
                Description = source.Name,
                Locations = locations
            };
        }


        public static HoursEntryRequestVM FromSessionValidationRequest(ValidationRequest source, int providerID, int patientID)
        {
            var session = source.RequestedValidatedSession;
            var result = new HoursEntryRequestVM
            {
                AllowHasDataChanges = false,
                CatalystPreloadID = null,
                Date = session.DateOfService,
                HoursID = session.ServerValidatedSessionID,
                IgnoreWarnings = false,
                IsTrainingEntry = false,
                PatientID = patientID,
                ProviderID = providerID,
                ServiceID = session.ServiceID,
                ServiceLocationID = session.LocationID,
                SsgIDs = session.SsgCaseIds?.Split(',').Select(x => int.Parse(x)).ToArray(),
                TimeIn = session.DateOfService + session.StartTime,
                TimeOut = session.DateOfService + session.StartTime + session.Duration
            };
            return result;
        }


        public static HoursEntryRequest2VM FromSessionUpdateRequest(SessionUpdateBaseRequest source, DomainServices.Sessions.SessionReport sessionReport, int providerID, bool isBCBA, int patientID)
        {
            var session = source.SessionDetails;
            var result = new HoursEntryRequest2VM
            {
                Status = (int)HoursStatus.ComittedByProvider,
                AllowHasDataChanges = false,
                CatalystPreloadID = null,
                Date = session.DateOfService,
                HoursID = session.ServerValidatedSessionID,
                IgnoreWarnings = false,
                IsTrainingEntry = false,
                PatientID = patientID,
                ProviderID = providerID,
                ServiceID = session.ServiceID,
                ServiceLocationID = session.LocationID,
                SsgIDs = session.SsgCaseIds?.Split(',').Select(x => int.Parse(x)).ToArray(),
                TimeIn = session.DateOfService + session.StartTime,
                TimeOut = session.DateOfService + session.StartTime + session.Duration,
                Signatures = source.GetSignatures()?.Select(m => new Shared.Web.App.HoursEntry.Models.Request.Signature
                {
                    Name = m.Name,
                    Base64Signature = m.Base64Signature
                }).ToArray()
            };
            if (isBCBA)
            {
                //// get multiple questions
                //result.ExtendedNotes = source.QuestionResponses?.Select(m => new HoursEntryRequestExtendedNoteVM
                //{
                //    TemplateID = m.NoteQuestionID,
                //    Answer = m.Answer
                //}).ToList();
                throw new InvalidOperationException("BCBA extended notes not supported in this app version");
            }
            else
            {
                result.SessionReport = sessionReport;
                //// get single question
                //if (source.QuestionResponses?.Count() > 0)
                //{
                //    result.Note = source.QuestionResponses.First()?.Answer;
                //}
            }
            return result;
        }


        private static Patient MapPatient(Domain2.Patients.Patient source)
        {
            if (source == null)
            {
                return null;
            }
            var result = new Patient
            {
                Gender = GetGender(source.Gender),
                ID = source.ID,
                PatientAddress1 = source.Address1,
                PatientAddress2 = source.Address2,
                PatientCity = source.City,
                PatientFirstName = source.FirstName,
                PatientGuardian2FirstName = source.Guardian2FirstName,
                PatientGuardian2LastName = source.Guardian2LastName,
                PatientGuardian2Phone = source.Guardian2HomePhone,
                PatientGuardian2Relationship = GetGuardianRelationship(source.Guardian2RelationshipID),
                PatientGuardianFirstName = source.GuardianFirstName,
                PatientGuardianLastName = source.GuardianLastName,
                PatientGuardianPhone = source.GuardianHomePhone,
                PatientGuardianRelationship = GetGuardianRelationship(source.GuardianRelationshipID),
                PatientLastName = source.LastName,
                PatientState = source.State,
                PatientZip = source.Zip
            };
            return result;
        }


        private static ActiveAuthorization MapAuthorization(Domain2.Authorizations.Authorization source)
        {
            if (source == null)
            {
                return null;
            }
            var result = new ActiveAuthorization
            {
                ID = source.ID,
                Name = source.AuthorizationCode.Code,
                TimeRemainingMinutes = (int)TimeSpan.FromHours((double)source.HoursRemaining).TotalMinutes
            };
            return result;
        }


        private static Insurance MapInsurance(Domain2.Insurances.Insurance source)
        {
            if (source == null)
            {
                return null;
            }
            var result = new Insurance
            {
                ID = source.ID,
                InsuranceName = source.Name
            };
            return result;
        }


        private static string GetGuardianRelationship(int? relationshipID)
        {
            if (!relationshipID.HasValue)
            {
                return null;
            }
            // this is more or less hardcoded in the database (dbo.GuardianRelationships), should be handled better...
            switch (relationshipID.Value)
            {
                case 1: return "Father";
                case 3: return "Grandparent";
                case 2: return "Guardian";
                case 0: return "Mother";
                case 5: return "Other";
                case 4: return "Relative";
                default: return null;
            }
        }


        private static Gender GetGender(string input)
        {
            switch (input)
            {
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
