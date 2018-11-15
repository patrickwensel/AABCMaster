using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AABC.DomainServices.Hours.Tests.Unit
{

    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.DomainServices.Hours")]
    public class AuthResolverTests
    {
        

        private Domain2.Hours.Hours getMockHours() {

            var hours = new Domain2.Hours.Hours();

            hours.Case = new Domain2.Cases.Case();
            hours.Service = new Domain2.Services.Service();
            hours.Provider = new Domain2.Providers.Provider();
            hours.Provider.ProviderType = new Domain2.Providers.ProviderType();

            hours.Case.Patient = new Domain2.Patients.Patient();
            hours.Case.Patient.Insurance = new Domain2.Insurances.Insurance();
            
            return hours;
        }



        //[TestMethod]
        //[TestCategory("DomainServices.Hours")]
        //public void AuthResolverGetsInitialBreakdown() {

        //    var hours = getMockHours();
        //    var insurance = hours.Case.Patient.Insurance;

        //    hours.Date = new DateTime(2017, 1, 1);
        //    hours.Service.ID = 9;                   // DR
        //    hours.Provider.ProviderType.ID = 17;    // Aide

        //    hours.Case.Authorizations = new List<Authorization>();
        //    hours.Case.Authorizations.Add(new Authorization()
        //    {
        //        ID = 1,
        //        AuthorizationCodeID = 100,
        //        StartDate = new DateTime(2016, 1, 1),
        //        EndDate = new DateTime(2018, 1, 1)
        //    });
        //    hours.Case.Authorizations.Add(new Authorization()
        //    {
        //        ID = 2,
        //        AuthorizationCodeID = 200,
        //        StartDate = new DateTime(2016, 1, 1),
        //        EndDate = new DateTime(2018, 1, 1)
        //    });

        //    var matchRule = new AuthorizationMatchRule()
        //    {
        //        InitialAuthorizationID = 100,
        //        FinalAuthorizationID = 200,
        //        InitialAuthorization = new AuthorizationCode(),
        //        FinalAuthorization = new AuthorizationCode(),
        //        ServiceID = 9,          // DR
        //        ProviderTypeID = 17,     // Aide
        //        InitialMinimumMinutes = 16,
        //        InitialUnitSize = 30,
        //        FinalMinimumMinutes = 16,
        //        FinalUnitSize = 30
        //    };

        //    insurance.AuthorizationMatchRules = new List<AuthorizationMatchRule>();
        //    insurance.AuthorizationMatchRules.Add(matchRule);
            
            
        //    hours.TotalHours = (decimal)0.5;

        //    var resolver = new AuthResolver(hours);
            
        //    var sut = resolver.GetAuthorizationBreakdowns();

        //    Assert.AreEqual(1, sut.Count);
        //    Assert.AreEqual(30, sut[0].Minutes);

        //}

        //[TestMethod]
        //[TestCategory("DomainServices.Hours")]
        //public void AuthResolverGetsFinalBreakdown() {

        //    var hours = getMockHours();
        //    var insurance = hours.Case.Patient.Insurance;

        //    hours.Date = new DateTime(2017, 1, 1);
        //    hours.Service.ID = 9;                   // DR
        //    hours.Provider.ProviderType.ID = 17;    // Aide

        //    hours.Case.Authorizations = new List<Authorization>();
        //    hours.Case.Authorizations.Add(new Authorization()
        //    {
        //        ID = 1,
        //        AuthorizationCodeID = 100,
        //        StartDate = new DateTime(2016, 1, 1),
        //        EndDate = new DateTime(2018, 1, 1)
        //    });
        //    hours.Case.Authorizations.Add(new Authorization()
        //    {
        //        ID = 2,
        //        AuthorizationCodeID = 200,
        //        StartDate = new DateTime(2016, 1, 1),
        //        EndDate = new DateTime(2018, 1, 1)
        //    });

        //    var matchRule = new AuthorizationMatchRule()
        //    {
        //        InitialAuthorizationID = 100,
        //        FinalAuthorizationID = 200,
        //        InitialAuthorization = new AuthorizationCode(),
        //        FinalAuthorization = new AuthorizationCode(),
        //        ServiceID = 9,          // DR
        //        ProviderTypeID = 17,     // Aide
        //        InitialMinimumMinutes = 16,
        //        InitialUnitSize = 30,
        //        FinalMinimumMinutes = 16,
        //        FinalUnitSize = 30
        //    };

        //    insurance.AuthorizationMatchRules = new List<AuthorizationMatchRule>();
        //    insurance.AuthorizationMatchRules.Add(matchRule);
            
        //    hours.TotalHours = (decimal)2;

        //    var resolver = new AuthResolver(hours);

        //    var sut = resolver.GetAuthorizationBreakdowns();

        //    Assert.AreEqual(2, sut.Count);
        //    Assert.AreEqual(90, sut[1].Minutes);

        //}

    }
}
