using AABC.Domain2.Authorizations;
using AABC.DomainServices.HoursResolution.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.HoursResolution.Tests.Unit
{
    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.DomainServices.HoursResolution")]
    public class AuthorizationBreakdownTests
    {

        [TestMethod]
        public void AuthResolverGetsInitialBreakdown()
        {
            var repoMock = CreateMockResolutionServiceRepository();
            var targetCase = repoMock.Object.GetCase(0);
            var hours = GetMockHours();
            var insurance = targetCase.Patient.Insurance;

            hours.Date = new DateTime(2017, 1, 1);
            hours.Service.ID = 9;                   // DR
            hours.Provider.ProviderType.ID = 17;    // Aide

            targetCase.Authorizations = new List<Authorization>
            {
                new Authorization()
                {
                    ID = 1,
                    AuthorizationCodeID = 100,
                    StartDate = new DateTime(2016, 1, 1),
                    EndDate = new DateTime(2018, 1, 1)
                },
                new Authorization()
                {
                    ID = 2,
                    AuthorizationCodeID = 200,
                    StartDate = new DateTime(2016, 1, 1),
                    EndDate = new DateTime(2018, 1, 1)
                }
            };

            var matchRule = new AuthorizationMatchRule()
            {
                InitialAuthorizationID = 100,
                FinalAuthorizationID = 200,
                InitialAuthorization = new AuthorizationCode(),
                FinalAuthorization = new AuthorizationCode(),
                ServiceID = 9,          // DR
                ProviderTypeID = 17,     // Aide
                InitialMinimumMinutes = 16,
                InitialUnitSize = 30,
                FinalMinimumMinutes = 16,
                FinalUnitSize = 30
            };
            insurance.AuthorizationMatchRules = new List<AuthorizationMatchRule>
            {
                matchRule
            };
            hours.TotalHours = (decimal)0.5;

            var authService = new AuthorizationResolution(repoMock.Object, null);
            var result = authService.Resolve(new List<Domain2.Hours.Hours>() { hours });
            Assert.IsTrue(hours.AuthorizationBreakdowns.Count == 1);
            Assert.IsTrue(hours.AuthorizationBreakdowns.First().Minutes == 30);
        }


        [TestMethod]
        public void AuthResolverGetsFinalBreakdown()
        {
            var repoMock = CreateMockResolutionServiceRepository();
            var targetCase = repoMock.Object.GetCase(0);
            var hours = GetMockHours();
            var insurance = targetCase.Patient.Insurance;

            hours.Date = new DateTime(2017, 1, 1);
            hours.Service.ID = 9;                   // DR
            hours.Provider.ProviderType.ID = 17;    // Aide

            targetCase.Authorizations = new List<Authorization>
            {
                new Authorization()
                {
                    ID = 1,
                    AuthorizationCodeID = 100,
                    StartDate = new DateTime(2016, 1, 1),
                    EndDate = new DateTime(2018, 1, 1)
                },
                new Authorization()
                {
                    ID = 2,
                    AuthorizationCodeID = 200,
                    StartDate = new DateTime(2016, 1, 1),
                    EndDate = new DateTime(2018, 1, 1)
                }
            };

            var matchRule = new AuthorizationMatchRule()
            {
                InitialAuthorizationID = 100,
                FinalAuthorizationID = 200,
                InitialAuthorization = new AuthorizationCode(),
                FinalAuthorization = new AuthorizationCode(),
                ServiceID = 9,          // DR
                ProviderTypeID = 17,     // Aide
                InitialMinimumMinutes = 16,
                InitialUnitSize = 30,
                FinalMinimumMinutes = 16,
                FinalUnitSize = 30
            };
            insurance.AuthorizationMatchRules = new List<AuthorizationMatchRule>
            {
                matchRule
            };
            hours.TotalHours = 2;

            var authService = new AuthorizationResolution(repoMock.Object, null);
            var result = authService.Resolve(new List<Domain2.Hours.Hours>() { hours });
            Assert.AreEqual(2, hours.AuthorizationBreakdowns.Count);
            Assert.AreEqual(90, hours.AuthorizationBreakdowns.ToList()[1].Minutes);
        }


        //[TestMethod]
        //public void AuthResolverRemovesExistingBreakdowns() {

        //}

        //[TestMethod]
        //public void AuthResolverNoAuthsIfNoInsurance() {

        //}

        //[TestMethod]
        //public void AuthResolveNoAuthsIfNoInsuranceAuthMatchRules() {

        //}

        //[TestMethod]
        //public void AuthResolverMatchesRulesOnServiceAndProviderType() {

        //}

        private static Mock<IResolutionServiceRepository> CreateMockResolutionServiceRepository()
        {
            var repoMock = new Mock<IResolutionServiceRepository>();
            var c = new Domain2.Cases.Case();
            var patient = new Domain2.Patients.Patient();
            var insurance = new Domain2.Insurances.Insurance();

            c.Patient = patient;
            patient.Insurance = insurance;

            repoMock.Setup(x => x.GetCase(It.IsAny<int>())).Returns(c);
            repoMock.Setup(x => x.GetActiveInsurance(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(c.Patient.Insurance);
            return repoMock;
        }


        private static Domain2.Hours.Hours GetMockHours()
        {
            var hours = new Domain2.Hours.Hours
            {
                Case = new Domain2.Cases.Case(),
                Service = new Domain2.Services.Service(),
                Provider = new Domain2.Providers.Provider()
            };
            hours.Provider.ProviderType = new Domain2.Providers.ProviderType();
            hours.Case.Patient = new Domain2.Patients.Patient
            {
                Insurance = new Domain2.Insurances.Insurance()
            };
            return hours;
        }

    }
}
