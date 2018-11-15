using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AABC.Mobile.SharedEntities.Messages;
using AABC.Mobile.SharedEntities.Entities;

namespace AABC.Mobile.Api.Repositories.Test.Integration
{
    [TestClass]
    [TestCategory("_Integration")]
    [TestCategory("AABC.Mobile.Api.Repositories")]
    public class CaseRepositoryTests
    {

        Domain2.Providers.Provider _provider;
        Data.V2.CoreContext _context;
        CasesRepository _repo;

        [TestInitialize]
        public void Initialize() {
            _context = new Data.V2.CoreContext();
            _provider = _context.Providers.Find(257); // Abi Magid, 18994
            _repo = new CasesRepository(_context, _provider);
        }

        [TestMethod]
        public void GetInitialDataGetsInitialData() {
            
            var initialData = _repo.GetInitialData();

            Assert.IsTrue(initialData.Cases.Count > 0);
        }

        [TestMethod]
        public void GetLocationsAndServicesGetsLocationsAndServices() {

            var sut = _repo.GetLocationsAndServices(522, 257, DateTime.Now);

            Assert.IsTrue(sut.Services.Count > 0);
        }

        [TestMethod]
        public void ValidateProcessesSessionValidationRequest() {

            var vr = new ValidationRequest();
            var session = new ValidatedSession();

            session.CaseID = 522;
            session.DateOfService = DateTime.Today;
            session.Duration = TimeSpan.FromMinutes(45);
            session.LocationID = 2;
            session.ServerValidatedSessionID = 0;
            session.ServiceID = 9;
            session.StartTime = DateTime.Now.TimeOfDay;
            
            vr.RequestedValidatedSession = session;

            var sut = _repo.Validate(vr, false);

            Assert.IsTrue(sut.ServerValidatedSessionID == 0);
            Assert.IsTrue(sut.NoteQuestions.Count == 1);            
        }

        [TestMethod]
        public void SessionUpdateUpdatesSession() {

            var sur = new SessionUpdateRequest();
            var session = new ValidatedSession();

            session.CaseID = 522;
            session.DateOfService = DateTime.Today;
            session.Duration = TimeSpan.FromMinutes(45);
            session.LocationID = 2;
            session.ServerValidatedSessionID = 0;
            session.ServiceID = 9;
            session.StartTime = DateTime.Now.TimeOfDay;
            
            sur.SessionDetails = session;
            sur.QuestionResponses = new System.Collections.Generic.List<NoteQuestionResponse>();
            sur.QuestionResponses.Add(new NoteQuestionResponse() {
                Answer = "This is my answer.  It has to be enough characters.  It has to have words and sentences and periods.",
                 NoteQuestionID = 15
            });

            var sut = _repo.SessionUpdate(sur);

            Assert.IsTrue(sut.Success);
        }

    }
}
