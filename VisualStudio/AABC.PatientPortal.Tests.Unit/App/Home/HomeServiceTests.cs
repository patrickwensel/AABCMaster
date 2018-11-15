using AABC.Domain2.Cases;
using AABC.Domain2.Hours;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.PatientPortal.App.Home.Tests.Unit
{

    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.PatientPortal")]
    public class HomeServiceTests
    {

        [TestMethod]        
        public void GetMonthlyHoursReturnsCorrectlyConvertedStartEndTime() {

            var period = new MonthlyPeriod();

            period.Case = new Case();
            period.Case.Hours = new List<Hours>();
            period.Case.Hours.Add(new Hours()
            {
                StartTime = new TimeSpan(2, 30, 0),
                EndTime = new TimeSpan(10, 45, 0),
                Provider = new Domain2.Providers.Provider()
            });

            var service = new HomeService(null, null);

            var sut = service.getMonthlyHours(period);

            Assert.IsTrue(sut.First().TimeIn == new DateTime(1, 1, 1, 2, 30, 0));
            Assert.IsTrue(sut.First().TimeOut == new DateTime(1, 1, 1, 10, 45, 0));

        }
    }
}
