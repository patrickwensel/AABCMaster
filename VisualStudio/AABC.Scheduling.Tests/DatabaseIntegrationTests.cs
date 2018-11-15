using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace AABC.Scheduling.Tests.Integration
{
    [TestClass]
    public class DatabaseIntegrationTests
    {
        [TestMethod]
        [TestCategory("__Integration")]
        [TestCategory("_I_AABC.Scheduling")]
        public void SchedulerCreatesSlotsFromDataInDB()
        {
            Assert.Inconclusive("This test is brittle, relying on specific data in the test db (which we're not formalized enough to have yet).  Should re-write...");
            
            //var scheduler = Scheduler.Create("SchedulingDataContext");
            //var slots = scheduler.GetSchedule(383, 436, new DateTime(2017,08,01), new DateTime(2017,08,31));
            //Assert.IsTrue(slots.Count() == 5);
            //Assert.IsTrue(slots.Any(m => m.Date == new DateTime(2017, 8, 7)));
            //Assert.IsTrue(slots.Any(m => m.Date == new DateTime(2017, 8, 14)));
            //Assert.IsTrue(slots.Any(m => m.Date == new DateTime(2017, 8, 21)));
            //Assert.IsTrue(slots.Any(m => m.Date == new DateTime(2017, 8, 23)));
            //Assert.IsTrue(slots.Any(m => m.Date == new DateTime(2017, 8, 28)));
        }
    }
}
