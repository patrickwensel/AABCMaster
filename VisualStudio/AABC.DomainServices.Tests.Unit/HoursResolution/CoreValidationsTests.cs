using AABC.DomainServices.HoursResolution.Repositories;
using Dymeng.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.HoursResolution.Tests.Unit
{

    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.DomainServices.HoursResolution")]
    public class CoreValidationsTests
    {

        // These tests are brittle.  Each test asserts that the result is false,
        // but the result could be false for any number of reasons, not necessarily
        // the tested reason.
        // The target code should have broken out methods to support each test


        Mock<IResolutionService> _resolutionServiceMock;
        Mock<IResolutionServiceRepository> _resolutionServiceRepositoryMock;

        [TestInitialize]
        public void Initialize() {

            _resolutionServiceMock = new Mock<IResolutionService>();
            _resolutionServiceMock.Setup(x => x.SSGServiceID).Returns(1);
            _resolutionServiceMock.Setup(x => x.EntryType).Returns(EntryType.Full);
            _resolutionServiceMock.Setup(x => x.Issues).Returns(() => new ValidationIssueCollection());
            _resolutionServiceMock.Setup(x => x.IsPreCheck).Returns(false);
            _resolutionServiceMock.Setup(x => x.PreCheckAdvancedDaysAllowance).Returns(7);

            _resolutionServiceRepositoryMock = new Mock<IResolutionServiceRepository>();
            _resolutionServiceRepositoryMock.Setup(x => x.GetCase(It.IsAny<int>())).Returns(new Domain2.Cases.Case());
            
        }
        

        [TestMethod]

        public void ValidateCoreEntriesDateLTENow() {

            var entry = new Domain2.Hours.Hours();

            entry.Date = DateTime.Now.AddDays(1);
            entry.StartTime = new TimeSpan(1, 2, 3);
            entry.EndTime = new TimeSpan(2, 3, 4);
            entry.Provider = new Domain2.Providers.Provider() { ProviderTypeID = 17 };
            entry.Memo = "asdf";
            entry.Service = new Domain2.Services.Service();

            var resolver = new CoreValidations(_resolutionServiceMock.Object, _resolutionServiceRepositoryMock.Object);

            bool result = resolver.ValidateCoreRequirements(entry);

            Assert.IsFalse(result);
        }

        [TestMethod]

        public void ValidateCoreEntriesStartTimeLTEndTime() {

            var entry = new Domain2.Hours.Hours();

            entry.Date = DateTime.Now.AddDays(-1);
            entry.EndTime = new TimeSpan(1, 2, 3);
            entry.StartTime = new TimeSpan(2, 3, 4);
            entry.Provider = new Domain2.Providers.Provider() { ProviderTypeID = 17 };
            entry.Memo = "asdf";
            entry.Service = new Domain2.Services.Service();

            var resolver = new CoreValidations(_resolutionServiceMock.Object, _resolutionServiceRepositoryMock.Object);

            bool result = resolver.ValidateCoreRequirements(entry);

            Assert.IsFalse(result);
        }

        [TestMethod]

        public void ValidateCoreEntriesServiceRequired() {

            var entry = new Domain2.Hours.Hours();

            entry.Date = DateTime.Now.AddDays(-1);
            entry.StartTime = new TimeSpan(1, 2, 3);
            entry.EndTime = new TimeSpan(2, 3, 4);
            entry.Provider = new Domain2.Providers.Provider() { ProviderTypeID = 17 };
            entry.Memo = "asdf";
            entry.Service = null;

            var resolver = new CoreValidations(_resolutionServiceMock.Object, _resolutionServiceRepositoryMock.Object);

            bool result = resolver.ValidateCoreRequirements(entry);

            Assert.IsFalse(result);

        }

        [TestMethod]

        public void ValidateCoreEntriesAideMemoRequired() {

            var entry = new Domain2.Hours.Hours();

            entry.Date = DateTime.Now.AddDays(-1);
            entry.StartTime = new TimeSpan(1, 2, 3);
            entry.EndTime = new TimeSpan(2, 3, 4);
            entry.Provider = new Domain2.Providers.Provider() { ProviderTypeID = 17 };
            entry.Memo = "";
            entry.Service = new Domain2.Services.Service();

            _resolutionServiceMock.Setup(x => x.EntryType).Returns(EntryType.Full);
            var resolver = new CoreValidations(_resolutionServiceMock.Object, _resolutionServiceRepositoryMock.Object);

            bool result = resolver.ValidateCoreRequirements(entry);

            Assert.IsFalse(result);
        }

        [TestMethod]

        public void ValidateCoreEntriesIgnoresAideMemoOnBasicType() {

            var entry = new Domain2.Hours.Hours();

            entry.Date = DateTime.Now.AddDays(-1);
            entry.StartTime = new TimeSpan(1, 2, 3);
            entry.EndTime = new TimeSpan(2, 3, 4);
            entry.Provider = new Domain2.Providers.Provider() { ProviderTypeID = 17 };
            entry.Memo = null;
            entry.Service = new Domain2.Services.Service();

            _resolutionServiceMock.Setup(x => x.EntryType).Returns(EntryType.Basic);
            var resolver = new CoreValidations(_resolutionServiceMock.Object, _resolutionServiceRepositoryMock.Object);

            bool result = resolver.ValidateCoreRequirements(entry);

            Assert.IsTrue(result);

        }

        [TestMethod]

        public void ValidateCoreEntriesBCBAExtendedNotesRequired() {

            var entry = new Domain2.Hours.Hours();

            entry.Date = DateTime.Now.AddDays(-1);
            entry.StartTime = new TimeSpan(1, 2, 3);
            entry.EndTime = new TimeSpan(2, 3, 4);
            entry.Provider = new Domain2.Providers.Provider() { ProviderTypeID = 15 };
            entry.ExtendedNotes = new List<Domain2.Hours.ExtendedNote>();
            entry.Service = new Domain2.Services.Service();

            _resolutionServiceMock.Setup(x => x.EntryType).Returns(EntryType.Full);
            var resolver = new CoreValidations(_resolutionServiceMock.Object, _resolutionServiceRepositoryMock.Object);

            bool result = resolver.ValidateCoreRequirements(entry);

            Assert.IsFalse(result);
        }

        [TestMethod]

        public void ValidateCoreEntriesIgnoresExtendedNotesOnBasicType () {

            var entry = new Domain2.Hours.Hours();

            entry.Date = DateTime.Now.AddDays(-1);
            entry.StartTime = new TimeSpan(1, 2, 3);
            entry.EndTime = new TimeSpan(2, 3, 4);
            entry.Provider = new Domain2.Providers.Provider() { ProviderTypeID = 15 };
            entry.ExtendedNotes = new List<Domain2.Hours.ExtendedNote>();
            entry.Service = new Domain2.Services.Service();
            
            _resolutionServiceMock.Setup(x => x.EntryType).Returns(EntryType.Basic);
            var resolver = new CoreValidations(_resolutionServiceMock.Object, _resolutionServiceRepositoryMock.Object);

            bool result = resolver.ValidateCoreRequirements(entry);

            Assert.IsTrue(result);

        }


    }
}
