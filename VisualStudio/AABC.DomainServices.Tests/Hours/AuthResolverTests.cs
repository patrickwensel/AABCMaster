using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;


namespace AABC.DomainServices.Hours.Tests.Integration
{
    [TestClass]
    [TestCategory("__Integration")]
    [TestCategory("_I_AABC.DomainServices.Hours")]
    public class AuthResolverTests
    {

        AABC.Data.V2.CoreContext _context;

        [TestInitialize]
        public void Initialize() {
            _context = new Data.V2.CoreContext();
        }


        [TestMethod]
        public void AuthResolverGetsNoAuth() {

            var patient = _context.Patients.Find(1865);  // aetna insurance
            var hours = new Domain2.Hours.Hours();
            hours.Case = patient.ActiveCase;
            hours.Provider = patient.ActiveCase.GetProvidersAtDate(DateTime.Now).First().Provider;
            hours.Date = DateTime.Now;
            hours.Service = _context.Services.Where(x => x.Code == "TM").First();
            hours.ServiceID = hours.Service.ID;
            hours.TotalHours = 1.5M;

            var resolver = new AuthResolver(hours);

            var sut = resolver.GetAuthorizationBreakdowns();

            Assert.IsNull(sut);

        }
        
    }
    
}
