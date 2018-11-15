using AABC.Domain.Cases;
using AABC.Domain.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Providers.Tests.Integration
{

    [TestClass]
    [TestCategory("__Integration")]
    [TestCategory("_I_AABC.DomainServices.Providers")]
    public class CrossHoursValidationTests
    {
        
        Data.Services.ICaseService _caseService;

        [TestInitialize]
        public void Initialize() {
            _caseService = new CaseServiceStub();
        }



        [TestMethod]
        public void CrossHoursProviderOverlapCatchesOverlap() {

            var v = new DomainServices.Hours.CrossHoursValidation(_caseService, new DateTime(2016, 7, 1, 0, 0, 0));
            
            v .Validate();
            
            Assert.AreEqual(
                1, 
                v.Errors
                    .Where(x => x.ErrorType == Hours.CrossHoursValidation.ValidationErrors.ProviderOverlapSelf)
                    .Count());
        

        }





















        class CaseServiceStub : Data.Services.ICaseService
        {
            public List<Case> GetActiveCasesByProvider(int providerID, int visibleAfterEndDateDays) {
                throw new NotImplementedException();
            }

            public List<Case> GetActiveCasesByProvider(int providerID, bool omitDischargedPatients) {
                throw new NotImplementedException();
            }

            public int? GetAssociatedCaseAuthID(int providerTypeID, int serviceID) {
                return null;
            }

            public AuthorizationClass GetAuthClassByCode(string authClassCode) {
                return new AuthorizationClass() { ID = 0 };
            }

            public Case GetCase(int caseID) {
                throw new NotImplementedException();
            }

            public List<CaseAuthorizationHours> GetCaseAuthorizationHoursProviderAndCase(int providerID, int caseID) {
                throw new NotImplementedException();
            }

            public List<CaseAuthorization> GetCaseAuthorizationsAndGeneralHours(int caseID, bool omitBCBAAuths = false) {
                throw new NotImplementedException();
            }

            public List<CaseAuthorization> GetCaseAuthorizationsAndHours(int caseID) {
                throw new NotImplementedException();
            }

            public List<CaseAuthorizationHours> GetCaseHoursByCase(int caseID) {
                throw new NotImplementedException();
            }

            public List<CaseAuthorizationHours> GetCaseHoursByCase(int caseID, DateTime? cutoff = null) {
                throw new NotImplementedException();
            }

            public List<CaseAuthorizationHours> GetCaseHoursByCaseAndDate(int caseID, DateTime date) {
                throw new NotImplementedException();
            }

            public List<CaseAuthorizationHours> GetCaseHoursByCaseByProvider(int caseID, int providerID, DateTime? cutoffDate = null) {
                throw new NotImplementedException();
            }

            public List<CaseAuthorizationHours> GetCaseHoursByDateRange(DateTime startDate, DateTime endDate) {


                var list = new List<CaseAuthorizationHours>();

                // 2016-01-01, 9:00-9:30
                list.Add(new CaseAuthorizationHours()
                {
                    ID = 0,
                    ProviderID = 0,
                    Date = new DateTime(2016, 1, 1),
                    TimeIn = new DateTime(2016, 1, 1, 9, 0, 0),
                    TimeOut = new DateTime(2016, 1, 1, 9, 30, 0),
                    Service = new Service() {  Code = "DUMMY" }
                });

                // 2016-01-01, 9:15-10:00 (overlap)
                list.Add(new CaseAuthorizationHours()
                {
                    ID = 1,
                    ProviderID = 0,
                    Date = new DateTime(2016, 1, 1),
                    TimeIn = new DateTime(2016, 1, 1, 9, 15, 0),
                    TimeOut = new DateTime(2016, 1, 1, 10, 0, 0),
                    Service = new Service() { Code = "DUMMY" }
                });

                // 2016-01-01, 9:15-10:00, different provider (no overlap)
                list.Add(new CaseAuthorizationHours()
                {
                    ID = 2,
                    ProviderID = 1,
                    Date = new DateTime(2016, 1, 1),
                    TimeIn = new DateTime(2016, 1, 1, 9, 15, 0),
                    TimeOut = new DateTime(2016, 1, 1, 10, 0, 0),
                    Service = new Service() { Code = "DUMMY" }
                });

                // 2016-02-01, 9:15-10:00, different date (no overlap)
                list.Add(new CaseAuthorizationHours()
                {
                    ID = 3,
                    ProviderID = 0,
                    Date = new DateTime(2016, 2, 1),
                    TimeIn = new DateTime(2016, 2, 1, 9, 15, 0),
                    TimeOut = new DateTime(2016, 2, 1, 10, 0, 0),
                    Service = new Service() { Code = "DUMMY" }
                });






                return list;

            }

            public CaseAuthorizationHours GetCaseHoursItem(int hoursID) {
                throw new NotImplementedException();
            }

            public List<MonthlyCasePeriod> GetCaseMonthlyPeriods(int caseID, DateTime startDate, DateTime endDate) {
                throw new NotImplementedException();
            }

            public CaseProvider GetCaseProviderByProviderAndCaseIDs(int providerID, int caseID) {
                throw new NotImplementedException();
            }

            public List<Case> GetCasesByPatientName(string firstName, string lastName) {
                throw new NotImplementedException();
            }

            public List<TimeScrubOverviewItem> GetCaseTimeScrubOverviewItems(DateTime startDate, DateTime endDate) {
                throw new NotImplementedException();
            }

            public List<GuardianRelationship> GetGuardianRelationships() {
                throw new NotImplementedException();
            }

            public List<Service> GetServices() {
                throw new NotImplementedException();
            }

            public List<Service> GetServicesByProviderType(int providerTypeID) {
                throw new NotImplementedException();
            }





        }


    }




    
}
