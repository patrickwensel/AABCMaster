using AABC.Domain.Cases;
using AABC.Domain.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.Providers.Tests.Integration
{
    [TestClass]
    [TestCategory("__Integration")]
    [TestCategory("_I_AABC.DomainServices.Providers")]
    public class HoursEntryResolverTests
    {
        
        Data.Services.ICaseService _caseService;

        [TestInitialize]
        public void Initialize() {
            _caseService = new CaseServiceStub();
        }


        //[TestMethod]
        //[TestCategory("Integration")]
        //public void HoursEntryBasicInputValidSetsMinValidationStatusProviderPending() {

        //    Case c = getTimeOverlapTestCaseStub();
        //    c.Authorizations = new List<CaseAuthorization>();

        //    var hoursToTest = new CaseAuthorizationHours()
        //    {
        //        Date = DateTime.Now.AddDays(-1),
        //        TimeIn = new DateTime(2016, 7, 2, 10, 30, 0),
        //        TimeOut = new DateTime(2016, 7, 2, 11, 30, 0),
        //        Service = new Service() { ID = 0 },
        //        Notes = "fake notes",
        //        Provider = new CaseProvider() { Type = new Domain.Providers.ProviderType() { ID = 0 } }
        //    };

        //    var resolver = new HoursEntryResolver(c, hoursToTest, _caseService);

        //    resolver.Resolve();

        //    Assert.IsTrue(resolver.PassingStatus >= HoursEntryResolver.ValidationResultStatus.ProviderPending);
        //}

        [TestMethod]
        public void HoursEntryFutureDateSetsValidationStatusNone() {

            Case c = getTimeOverlapTestCaseStub();
            
            var hoursToTest = new CaseAuthorizationHours()
            {
                Date = DateTime.Now.AddDays(1),
                TimeIn = new DateTime(2016, 7, 2, 10, 30, 0),
                TimeOut = new DateTime(2016, 7, 2, 11, 30, 0),
                Service = new Service() { ID = 0 },
                Notes = "fake notes",
                Provider = new CaseProvider() { Type = new Domain.Providers.ProviderType() { ID = 0 } }
            };

            var resolver = new HoursEntryResolver(c, hoursToTest, _caseService);

            resolver.Resolve();

            Assert.IsTrue(resolver.PassingStatus == HoursEntryResolver.ValidationResultStatus.None);
        }

        


        [TestMethod]
        public void HoursEntryTimeInOutDatePartsMatchDateProperty() {

            Case c = getTimeOverlapTestCaseStub();

            var hoursToTest = new CaseAuthorizationHours()
            {
                Date = DateTime.Now.AddDays(-2),
                TimeIn = new DateTime(2011, 7, 2, 10, 30, 0),
                TimeOut = new DateTime(2011, 7, 2, 11, 30, 0),
                Service = new Service() { ID = 0 },
                Notes = "fake notes",
                Provider = new CaseProvider() { Type = new Domain.Providers.ProviderType() { ID = 0 } }
            };

            var resolver = new HoursEntryResolver(c, hoursToTest, _caseService);

            resolver.Resolve();

            Assert.IsTrue(hoursToTest.TimeIn.Date == hoursToTest.Date);
            Assert.IsTrue(hoursToTest.TimeOut.Date == hoursToTest.Date);
            
        }

        [TestMethod]
        public void HoursEntryBillablePayableRoundNearestQuarter() {
            
            Case c = getTimeOverlapTestCaseStub();

            var hoursToTest = new CaseAuthorizationHours()
            {
                CaseID = 420,
                Date = DateTime.Now.AddDays(-2),
                TimeIn = new DateTime(2011, 7, 2, 10, 02, 0),
                TimeOut = new DateTime(2011, 7, 2, 11, 28, 0),
                Service = new Service() { ID = 0 },
                Notes = "fake notes",
                Provider = new CaseProvider() { Type = new Domain.Providers.ProviderType() { ID = 0 } }
            };

            var resolver = new HoursEntryResolver(c, hoursToTest, _caseService);

            resolver.Resolve();

            Assert.IsTrue(hoursToTest.BillableHours == 1.5);
            Assert.IsTrue(hoursToTest.PayableHours == 1.5);
        }
        

        Case getTimeOverlapTestCaseStub() {

            var c = new Case();

            c.ID = 420;

            c.Authorizations = new List<CaseAuthorization>();

            c.Authorizations.Add(
                new CaseAuthorization() {
                    Hours = new List<CaseAuthorizationHours>(),
                    AuthClass = new AuthorizationClass()
                    {
                        ID = 0
                    }                    
                });

            c.Authorizations.Add(
                new CaseAuthorization()
                {
                    Hours = new List<CaseAuthorizationHours>(),
                    AuthClass = new AuthorizationClass()
                    {
                        ID = 0
                    }
                });
            
            
            c.Authorizations[0].Hours.Add(new CaseAuthorizationHours()
            {
                Date = new DateTime(2016, 5, 1),
                TimeIn = new DateTime(2016, 5, 1, 10, 0, 0),
                TimeOut = new DateTime(2016, 5, 1, 11, 0, 0)
            });
            
            c.Authorizations[1].Hours.Add(new CaseAuthorizationHours()
            {
                Date = new DateTime(2016, 5, 2),
                TimeIn = new DateTime(2016, 5, 2, 10, 0, 0),
                TimeOut = new DateTime(2016, 5, 2, 11, 0, 0)
            });
            
            c.Authorizations[1].Hours.Add(new CaseAuthorizationHours()
            {
                Date = new DateTime(2016, 5, 3),
                TimeIn = new DateTime(2016, 5, 3, 10, 0, 0),
                TimeOut = new DateTime(2016, 5, 3, 11, 0, 0)
            });
            
            c.Authorizations[1].Hours.Add(new CaseAuthorizationHours()
            {
                Date = new DateTime(2016, 5, 4),
                TimeIn = new DateTime(2016, 5, 4, 10, 0, 0),
                TimeOut = new DateTime(2016, 5, 4, 11, 0, 0)
            });

            return c;
            
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
                var list = new List<CaseAuthorizationHours>();

                list.Add(new CaseAuthorizationHours()
                {
                    Date = new DateTime(2016, 5, 1),
                    TimeIn = new DateTime(2016, 5, 1, 10, 0, 0),
                    TimeOut = new DateTime(2016, 5, 1, 11, 0, 0)
                });

                list.Add(new CaseAuthorizationHours()
                {
                    Date = new DateTime(2016, 5, 2),
                    TimeIn = new DateTime(2016, 5, 2, 10, 0, 0),
                    TimeOut = new DateTime(2016, 5, 2, 11, 0, 0)
                });

                list.Add(new CaseAuthorizationHours()
                {
                    Date = new DateTime(2016, 5, 3),
                    TimeIn = new DateTime(2016, 5, 3, 10, 0, 0),
                    TimeOut = new DateTime(2016, 5, 3, 11, 0, 0)
                });

                list.Add(new CaseAuthorizationHours()
                {
                    Date = new DateTime(2016, 5, 4),
                    TimeIn = new DateTime(2016, 5, 4, 10, 0, 0),
                    TimeOut = new DateTime(2016, 5, 4, 11, 0, 0)
                });

                return list;

            }

            public List<CaseAuthorizationHours> GetCaseHoursByCaseAndDate(int caseID, DateTime date) {
                throw new NotImplementedException();
            }

            public List<CaseAuthorizationHours> GetCaseHoursByCaseByProvider(int caseID, int providerID, DateTime? cutoffDate = null) {
                throw new NotImplementedException();
            }

            public List<CaseAuthorizationHours> GetCaseHoursByDateRange(DateTime startDate, DateTime endDate) {
                throw new NotImplementedException();
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
