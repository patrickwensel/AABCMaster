using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AABC.Domain2.Hours.Tests.Unit
{
    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.Domain2.Hours")]
    public class HoursTests
    {

        [TestInitialize]
        public void Initialize() {

        }

        [TestMethod]        
        public void HoursIsEditableByProviderFalseIfNoSameProvider() {

            var testProvider = new Providers.Provider() { ID = 1 };
            var hours = new Hours();
            hours.Provider = new Providers.Provider() { ID = 2 };

            bool sut = hours.IsEditableByProvider(testProvider);

            Assert.IsFalse(sut);
        }

        [TestMethod]
        public void HoursIsEditableByProviderFalseIfNotPendingOrCommitted() {

            var testProvider = new Providers.Provider() { ID = 1 };
            var hours = new Hours();
            hours.Provider = new Providers.Provider() { ID = 1 };

            hours.Status = HoursStatus.FinalizedByProvider;
            Assert.IsFalse(hours.IsEditableByProvider(testProvider));

            hours.Status = HoursStatus.ProcessedComplete;
            Assert.IsFalse(hours.IsEditableByProvider(testProvider));

            hours.Status = HoursStatus.ScrubbedByAdmin;
            Assert.IsFalse(hours.IsEditableByProvider(testProvider));
        }

        [TestMethod]
        public void HoursIsEditableByProviderFalseIfCaseFinalized() {

            var testProvider = new Providers.Provider() { ID = 1 };
            var finalization = new Cases.HoursFinalization();
            var hours = new Hours();
            var period = new Domain2.Cases.MonthlyPeriod();
            var c = new Cases.Case();

            hours.Provider = new Providers.Provider() { ID = 1 };
            hours.Status = HoursStatus.Pending;
            finalization.ProviderID = 1;
            finalization.Provider = testProvider;
            c.Periods = new List<Cases.MonthlyPeriod>();
            c.Periods.Add(period);
            period.Finalizations = new List<Cases.HoursFinalization>();
            period.Finalizations.Add(finalization);
            hours.Case = c;

            Assert.IsFalse(hours.IsEditableByProvider(testProvider));
     
        }

        [TestMethod]
        public void HoursIsEditableByProviderFalseIfSSGParentNotSame() {

            var testProvider = new Providers.Provider() { ID = 1 };
            var hours = new Hours();
            hours.Provider = new Providers.Provider() { ID = 1 };
            hours.Status = HoursStatus.Pending;
            hours.Case = new Cases.Case();
            hours.ID = 12345;
            hours.SSGParentID = 54321;

            Assert.IsFalse(hours.IsEditableByProvider(testProvider));
        }

        [TestMethod]
        public void HoursIsEditableByProviderTrueifAllConditionsPass() {

            var testProvider = new Providers.Provider() { ID = 1 };
            var hours = new Hours();
            hours.Provider = new Providers.Provider() { ID = 1 };
            hours.Status = HoursStatus.Pending;
            hours.Case = new Cases.Case();

            Assert.IsTrue(hours.IsEditableByProvider(testProvider));
        }
    }
}
