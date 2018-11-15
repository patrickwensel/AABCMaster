using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;


namespace AABC.DomainServices.HoursResolution.Tests.Unit
{
    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.DomainServices.HoursResolution")]
    public class NormalizeTimesTests
    {

        Mock<Repositories.IResolutionServiceRepository> _repoMock;

        [TestInitialize]
        public void Initialize() {

            _repoMock = new Mock<Repositories.IResolutionServiceRepository>();

            _repoMock.Setup(x => x.GetCase(0)).Returns(() => new Domain2.Cases.Case());
            _repoMock.Setup(x => x.GetProvider(0)).Returns(() => new Domain2.Providers.Provider());
            _repoMock.Setup(x => x.GetCaseHours(0)).Returns(() => new List<Domain2.Hours.Hours>());
            _repoMock.Setup(x => x.GetProviderHours(0)).Returns(() => new List<Domain2.Hours.Hours>());
        }

        private List<Domain2.Hours.Hours> GetEntryList(Domain2.Hours.Hours entry) {
            return new List<Domain2.Hours.Hours>() { entry };
        }


        [TestMethod]

        public void NormalizeTimesStripsTimeFromHoursDate() {

            var entry = new Domain2.Hours.Hours
            {
                Case = new Domain2.Cases.Case(),
                Provider = new Domain2.Providers.Provider(),
                Date = new DateTime(2017, 1, 2, 3, 4, 5)
            };
            var resolver = new ResolutionService(GetEntryList(entry), _repoMock.Object);
            resolver.NormalizeCoreData(EntryApp.Unknown);
            Assert.AreEqual(new DateTime(2017, 1, 2, 0, 0, 0), entry.Date);
        }

        [TestMethod]

        public void NormalizeTimesSetsTotalHours() {

            var entry = new Domain2.Hours.Hours
            {
                Case = new Domain2.Cases.Case(),
                Provider = new Domain2.Providers.Provider(),
                Date = DateTime.Now,
                StartTime = new TimeSpan(6, 15, 0),
                EndTime = new TimeSpan(9, 30, 0)
            };

            var resolver = new ResolutionService(GetEntryList(entry), _repoMock.Object);
            resolver.NormalizeCoreData(EntryApp.Unknown);

            Assert.AreEqual((decimal)3.25, entry.TotalHours);

        }

        [TestMethod]

        public void NormalizeTimesSetsBillableHours() {

            var entry = new Domain2.Hours.Hours
            {
                Case = new Domain2.Cases.Case(),
                Provider = new Domain2.Providers.Provider(),
                Date = DateTime.Now,
                StartTime = new TimeSpan(6, 15, 0),
                EndTime = new TimeSpan(9, 30, 0)
            };

            var resolver = new ResolutionService(GetEntryList(entry), _repoMock.Object);
            resolver.NormalizeCoreData(EntryApp.Unknown);

            Assert.AreEqual((decimal)3.25, entry.BillableHours);

        }


        [TestMethod]

        public void NormalizeTimesSetsPayableHours() {

            var entry = new Domain2.Hours.Hours
            {
                Case = new Domain2.Cases.Case(),
                Provider = new Domain2.Providers.Provider(),
                Date = DateTime.Now,
                StartTime = new TimeSpan(6, 15, 0),
                EndTime = new TimeSpan(9, 30, 0)
            };

            var resolver = new ResolutionService(GetEntryList(entry), _repoMock.Object);
            resolver.NormalizeCoreData(EntryApp.Unknown);

            Assert.AreEqual((decimal)3.25, entry.PayableHours);

        }


    }
}
