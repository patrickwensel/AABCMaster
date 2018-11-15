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
    public class SSGEntryTests
    {

        Mock<IResolutionService> _resolutionServiceMock;

        [TestInitialize]
        public void Initialize() {

            _resolutionServiceMock = new Mock<IResolutionService>();
            _resolutionServiceMock.Setup(x => x.SSGServiceID).Returns(1);
            _resolutionServiceMock.Setup(x => x.Issues).Returns(() => new ValidationIssueCollection());
        }



        private Mock<Repositories.IResolutionServiceRepository> GetRepositoryMock(bool includeFinalized = false) {

            var mock = new Mock<Repositories.IResolutionServiceRepository>();

            var case1 = new Domain2.Cases.Case();
            var case2 = new Domain2.Cases.Case();

            case1.ID = 1;
            case2.ID = 1;

            case1.Hours = new List<Domain2.Hours.Hours>();
            case2.Hours = new List<Domain2.Hours.Hours>();

            case1.Hours.Add(new Domain2.Hours.Hours() { Status = Domain2.Hours.HoursStatus.ComittedByProvider, Date = new DateTime(2017, 1, 1) });
            case1.Hours.Add(new Domain2.Hours.Hours() { Status = Domain2.Hours.HoursStatus.ComittedByProvider, Date = new DateTime(2017, 1, 1) });
            case2.Hours.Add(new Domain2.Hours.Hours() { Status = Domain2.Hours.HoursStatus.ComittedByProvider, Date = new DateTime(2017, 1, 1) });
            case2.Hours.Add(new Domain2.Hours.Hours() { Status = Domain2.Hours.HoursStatus.ComittedByProvider, Date = new DateTime(2017, 1, 1) });

            if (includeFinalized) {
                case2.Hours.Add(new Domain2.Hours.Hours() { Status = Domain2.Hours.HoursStatus.FinalizedByProvider, Date = new DateTime(2017, 1, 1) });
            }

            mock.Setup(x => x.GetSSGCases(It.IsAny<Domain2.Hours.Hours>())).Returns(() => new List<Domain2.Cases.Case>() {
                case1, case2
            });

            return mock;
        }



        [TestMethod]

        public void ValidateCoreEntriesSSGTrueIfNotSSGService() {

            var entry = new Domain2.Hours.Hours
            {
                ServiceID = 0
            };

            var resolver = new CoreValidations(_resolutionServiceMock.Object, null);

            bool result = resolver.ValidateSSG(entry);

            Assert.IsTrue(result);
        }

        [TestMethod]

        public void ValidateCoreEntriesSSGFalseIfNoSSGIDs() {

            var entry = new Domain2.Hours.Hours
            {
                ServiceID = 1
            };

            var resolver = new CoreValidations(_resolutionServiceMock.Object, null);

            bool result = resolver.ValidateSSG(entry);

            Assert.IsFalse(result);
        }

        [TestMethod]

        public void ValidateCoreEntriesSSGFalseIfLessThanTwoSSGIDs() {

            var entry = new Domain2.Hours.Hours
            {
                ServiceID = 1,
                SSGCaseIDs = new int[] { 1 }
            };

            var resolver = new CoreValidations(_resolutionServiceMock.Object, null);

            bool result = resolver.ValidateSSG(entry);

            Assert.IsFalse(result);
        }

        [TestMethod]

        public void ValidateCoreEntriesSSGFalseIfFinalizedTargetCaseAndProviderEntered() {

            var entry = new Domain2.Hours.Hours
            {
                ServiceID = 1,
                Date = new DateTime(2017, 1, 1),
                SSGCaseIDs = new int[] { 1, 2 }
            };

            _resolutionServiceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            var repoMock = GetRepositoryMock(true);

            var resolver = new CoreValidations(_resolutionServiceMock.Object, repoMock.Object);

            bool result = resolver.ValidateSSG(entry);

            Assert.IsFalse(result);
        }

        [TestMethod]

        public void ValidateCoreEntriesSSGTrueIfFinalizedTargetCaseAndManageEntered() {

            var entry = new Domain2.Hours.Hours
            {
                ServiceID = 1,
                Date = new DateTime(2017, 1, 1),
                SSGCaseIDs = new int[] { 1, 2 }
            };

            _resolutionServiceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ManagementEntry);
            var repoMock = GetRepositoryMock(true);

            var resolver = new CoreValidations(_resolutionServiceMock.Object, repoMock.Object);

            bool result = resolver.ValidateSSG(entry);

            Assert.IsTrue(result);
        }

        [TestMethod]

        public void ValidateCoreEntriesSSGTrue() {

            var entry = new Domain2.Hours.Hours
            {
                ServiceID = 1,
                Date = new DateTime(2017, 1, 1),
                SSGCaseIDs = new int[] { 1, 2 }
            };

            _resolutionServiceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            var repoMock = GetRepositoryMock(false);

            var resolver = new CoreValidations(_resolutionServiceMock.Object, repoMock.Object);

            bool result = resolver.ValidateSSG(entry);

            Assert.IsTrue(result);
        }


        [TestMethod]

        public void ValidateCoreEntriesSSGAddsErrorIssueOnInsufficientCaseCount() {

            var entry = new Domain2.Hours.Hours
            {
                ServiceID = 1,
                Date = new DateTime(2017, 1, 1),
                SSGCaseIDs = new int[] { 1 }
            };

            _resolutionServiceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _resolutionServiceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            var repoMock = GetRepositoryMock(true);
            var resolver = new CoreValidations(_resolutionServiceMock.Object, repoMock.Object);

            bool result = resolver.ValidateSSG(entry);

            Assert.IsTrue(_resolutionServiceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(_resolutionServiceMock.Object.Issues.HasErrors);
        }

        [TestMethod]

        public void ValidateCoreEntriesSSGAddsErrorIssueOnFinalizedCaseProviderEntry() {

            var entry = new Domain2.Hours.Hours
            {
                ServiceID = 1,
                Date = new DateTime(2017, 1, 1),
                SSGCaseIDs = new int[] { 1, 2 }
            };

            _resolutionServiceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _resolutionServiceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            var repoMock = GetRepositoryMock(true);
            var resolver = new CoreValidations(_resolutionServiceMock.Object, repoMock.Object);

            bool result = resolver.ValidateSSG(entry);

            Assert.IsTrue(_resolutionServiceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(_resolutionServiceMock.Object.Issues.HasErrors);
        }

        [TestMethod]

        public void ValidateCoreEntiresSSGAddsWarningIssueOnFinalizedCaseManageEntry() {

            var entry = new Domain2.Hours.Hours
            {
                ServiceID = 1,
                Date = new DateTime(2017, 1, 1),
                SSGCaseIDs = new int[] { 1, 2 }
            };

            _resolutionServiceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ManagementEntry);
            _resolutionServiceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            var repoMock = GetRepositoryMock(true);
            var resolver = new CoreValidations(_resolutionServiceMock.Object, repoMock.Object);

            bool result = resolver.ValidateSSG(entry);

            Assert.IsTrue(_resolutionServiceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(_resolutionServiceMock.Object.Issues.HasWarnings);
        }

        [TestMethod]

        public void ValidateCoreEntriesSSGNoIssuesOnSuccess() {

            var entry = new Domain2.Hours.Hours
            {
                ServiceID = 1,
                Date = new DateTime(2017, 1, 1),
                SSGCaseIDs = new int[] { 1, 2 }
            };

            _resolutionServiceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _resolutionServiceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            var repoMock = GetRepositoryMock(false);

            var resolver = new CoreValidations(_resolutionServiceMock.Object, repoMock.Object);

            bool result = resolver.ValidateSSG(entry);

            Assert.IsTrue(_resolutionServiceMock.Object.Issues.Issues.Count == 0);
        }



    }
}
