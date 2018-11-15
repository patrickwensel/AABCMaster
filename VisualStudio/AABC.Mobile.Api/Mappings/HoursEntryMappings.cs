using AABC.Domain2.Providers;
using AABC.Mobile.SharedEntities.Messages;
using AABC.Shared.Web.App.HoursEntry.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AABC.Mobile.Api.Mappings
{
    public class HoursEntryMappings 
    {

        Data.V2.CoreContext _context;
        Provider _provider;

        public HoursEntryMappings(Provider provider, Data.V2.CoreContext context) {
            _context = context;
            _provider = provider;
        }
        

        public HoursEntryRequestVM FromSessionValidationRequest(ValidationRequest source) {

            var session = source.RequestedValidatedSession;
            var result = new HoursEntryRequestVM
            {
                AllowHasDataChanges = false,
                CatalystPreloadID = null,
                Date = session.DateOfService,
                HoursID = session.ServerValidatedSessionID,
                IgnoreWarnings = false,
                IsTrainingEntry = false,
                PatientID = _context.Cases.Find(session.CaseID).PatientID,
                ProviderID = _provider.ID,
                ServiceID = session.ServiceID,
                ServiceLocationID = session.LocationID,
                SsgIDs = session.SsgCaseIds?.Split(',').Select(x => int.Parse(x)).ToArray(),
                TimeIn = session.DateOfService + session.StartTime,
                TimeOut = session.DateOfService + session.StartTime + session.Duration
            };
            return result;
        }


        public HoursEntryRequestVM FromSessionUpdateRequest(SessionUpdateRequest source) {

            var session = source.SessionDetails;
            var questions = source.QuestionResponses;

            var result = new HoursEntryRequestVM
            {
                Status = (int)Domain2.Hours.HoursStatus.ComittedByProvider,
                AllowHasDataChanges = false,
                CatalystPreloadID = null,
                Date = session.DateOfService,
                HoursID = session.ServerValidatedSessionID,
                IgnoreWarnings = false,
                IsTrainingEntry = false,
                PatientID = _context.Cases.Find(session.CaseID).PatientID,
                ProviderID = _provider.ID,
                ServiceID = session.ServiceID,
                ServiceLocationID = session.LocationID,
                SsgIDs = session.SsgCaseIds?.Split(',').Select(x => int.Parse(x)).ToArray(),
                TimeIn = session.DateOfService + session.StartTime,
                TimeOut = session.DateOfService + session.StartTime + session.Duration
            };
            if (_provider.ProviderTypeID == (int)ProviderTypeIDs.BoardCertifiedBehavioralAnalyst) {
                // get multiple questions
                result.ExtendedNotes = new List<HoursEntryRequestExtendedNoteVM>();
                foreach (var q in questions) {
                    result.ExtendedNotes.Add(new HoursEntryRequestExtendedNoteVM()
                    {
                        TemplateID = q.NoteQuestionID,
                        Answer = q.Answer
                    });
                }
            } else {
                // get single question
                if (questions.Count > 0) {
                    result.Note = questions.First().Answer;
                }
            }
            result.Base64Signatures = source.Base64Signatures.ToArray();
            return result;

        }


    }
}