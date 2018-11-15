using Dymeng.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.HoursResolution.Tests.Unit
{


    // TODO:
    // finish AideMaxHoursPerDayPerCase at bottom of this class
    // add validateBCBAMaxHoursPerEntry tests
    // add validateBCBAMaxAssessmentHoursPerCasePerDay
    // move on to auths tests


    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.DomainServices.HoursResolution")]
    public class CaseAndProviderValidationTests
    {

        Mock<IResolutionService> _serviceMock;
        List<Domain2.Hours.Hours> _selfOverlapHours;
        List<Domain2.Hours.Hours> _aideDROverlapHours;
        List<Domain2.Hours.Hours> _aideMaxHoursPerDayPerCaseHours;
        List<Domain2.Hours.Hours> _bcbaMaxAssessmentCaseHours;


        [TestInitialize]
        public void Initialize()
        {

            _serviceMock = new Mock<IResolutionService>();
            _serviceMock.Setup(x => x.DRServiceID).Returns((int)Domain2.Services.ServiceIDs.DirectCare);

            _selfOverlapHours = GetSelfOverlapHours();  // 1-2, 3-4, 5-6 (2017-01-01)
            _aideDROverlapHours = GetAideNotOverlapDRHours();
            _aideMaxHoursPerDayPerCaseHours = GetAideMaxHoursPerDayPerCaseHours();  // 3 hours 2017-01-01, 2 hours 2017-02-01
            _bcbaMaxAssessmentCaseHours = GetBCBAMaxAssessmentCaseHours();
        }

        private void ResetService()
        {
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.Issues).Returns(() => new ValidationIssueCollection());
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider());
            _serviceMock.Setup(x => x.ProposedEntries).Returns(() => null);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => null);
            _serviceMock.Setup(x => x.AllProposedProviderHours).Returns(() => null);
        }




        #region Aide Self Overlap

        private List<Domain2.Hours.Hours> GetSelfOverlapHours()
        {

            var items = new List<Domain2.Hours.Hours>
            {
                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    StartTime = new TimeSpan(1, 0, 0),
                    EndTime = new TimeSpan(2, 0, 0),
                    Provider = new Domain2.Providers.Provider() { ID = 5 }
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    StartTime = new TimeSpan(3, 0, 0),
                    EndTime = new TimeSpan(4, 0, 0),
                    Provider = new Domain2.Providers.Provider() { ID = 5 }
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    StartTime = new TimeSpan(5, 0, 0),
                    EndTime = new TimeSpan(6, 0, 0),
                    Provider = new Domain2.Providers.Provider() { ID = 5 }
                },

                // overlap on a different date
                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 2, 1),
                    StartTime = new TimeSpan(2, 0, 0),
                    EndTime = new TimeSpan(4, 0, 0),
                    Provider = new Domain2.Providers.Provider() { ID = 5 }
                }
            };

            return items;
        }




        [TestMethod]
        public void ValidateProviderNotSelfOverlapIgnoresBCBAs()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(1, 30, 0),
                EndTime = new TimeSpan(2, 30, 0)
            };

            ResetService();
            _serviceMock.Setup(x => x.AllProposedProviderHours).Returns(() => _selfOverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 15 });
            _serviceMock.Object.AllProposedProviderHours.Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateProviderNotSelfOverlap(entry);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateProviderNotSelfOverlapFalseIfOverlappedViaProvider()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(1, 30, 0),
                EndTime = new TimeSpan(2, 30, 0)
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedProviderHours).Returns(() => _selfOverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Object.AllProposedProviderHours.Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateProviderNotSelfOverlap(entry);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateProviderNotSelfOverlapAddsErrorIssueOnProviderEntered()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(1, 30, 0),
                EndTime = new TimeSpan(2, 30, 0)
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedProviderHours).Returns(() => _selfOverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            _serviceMock.Object.AllProposedProviderHours.Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateProviderNotSelfOverlap(entry);

            Assert.IsTrue(_serviceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(_serviceMock.Object.Issues.HasErrors == true);

        }

        [TestMethod]
        public void ValidateProviderNotSelfOverlapAddsWarningIssueOnManageEntered()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(1, 30, 0),
                EndTime = new TimeSpan(2, 30, 0)
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ManagementEntry);
            _serviceMock.Setup(x => x.AllProposedProviderHours).Returns(() => _selfOverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            _serviceMock.Object.AllProposedProviderHours.Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateProviderNotSelfOverlap(entry);

            Assert.IsTrue(_serviceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(_serviceMock.Object.Issues.HasWarnings == true);

        }

        [TestMethod]
        public void ValidateProviderNotSelfOverlapTrueIfNoOverlaps()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(12, 30, 0),
                EndTime = new TimeSpan(13, 30, 0)
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedProviderHours).Returns(() => _selfOverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            _serviceMock.Object.AllProposedProviderHours.Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateProviderNotSelfOverlap(entry);

            Assert.IsTrue(result);
            Assert.IsTrue(_serviceMock.Object.Issues.Issues.Count == 0);
        }

        #endregion

        #region Aide Not DR Overlap On Case

        private List<Domain2.Hours.Hours> GetAideNotOverlapDRHours()
        {

            var items = new List<Domain2.Hours.Hours>
            {
                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    StartTime = new TimeSpan(1, 0, 0),
                    EndTime = new TimeSpan(2, 0, 0),
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ServiceID = 9
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    StartTime = new TimeSpan(3, 0, 0),
                    EndTime = new TimeSpan(4, 0, 0),
                    Provider = new Domain2.Providers.Provider() { ID = 10 },
                    ServiceID = 9
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    StartTime = new TimeSpan(5, 0, 0),
                    EndTime = new TimeSpan(6, 0, 0),
                    Provider = new Domain2.Providers.Provider() { ID = 15 },
                    ServiceID = 9
                },


                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    StartTime = new TimeSpan(1, 0, 0),
                    EndTime = new TimeSpan(2, 0, 0),
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ServiceID = 10
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    StartTime = new TimeSpan(3, 0, 0),
                    EndTime = new TimeSpan(4, 0, 0),
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ServiceID = 11
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 2, 1),
                    StartTime = new TimeSpan(1, 0, 0),
                    EndTime = new TimeSpan(2, 0, 0),
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ServiceID = 9
                }
            };

            return items;
        }

        [TestMethod]

        public void ValidateAideNotDROverlapOnCaseIgnoresBCBA()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(1, 30, 0),
                EndTime = new TimeSpan(2, 30, 0),
                ServiceID = 9
            };

            ResetService();
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideDROverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 15 });
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideNotDROverlapOnCase(entry);

            Assert.IsTrue(result);

        }

        [TestMethod]

        public void ValidateAideNotDROverlapFalseIfOverlapped()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(1, 30, 0),
                EndTime = new TimeSpan(2, 30, 0),
                ServiceID = 9
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideDROverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideNotDROverlapOnCase(entry);

            Assert.IsFalse(result);

        }

        [TestMethod]

        public void ValidateAideNotDROverlapOnCaseIgnoresOtherDates()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 3, 1),
                StartTime = new TimeSpan(1, 30, 0),
                EndTime = new TimeSpan(2, 30, 0),
                ServiceID = 9
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideDROverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideNotDROverlapOnCase(entry);

            Assert.IsTrue(result);
        }

        [TestMethod]

        public void ValidateAideNotDROverlapOnCaseIgnoresOtherServices()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(1, 30, 0),
                EndTime = new TimeSpan(2, 30, 0),
                ServiceID = 11
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideDROverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideNotDROverlapOnCase(entry);

            Assert.IsTrue(result);

        }

        [TestMethod]

        public void ValidateAideNotDROverlapOnCaseAddsErrorIssueIfProviderEntered()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(1, 30, 0),
                EndTime = new TimeSpan(2, 30, 0),
                ServiceID = 9
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideDROverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideNotDROverlapOnCase(entry);

            Assert.IsTrue(_serviceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(_serviceMock.Object.Issues.HasErrors == true);
        }

        [TestMethod]

        public void ValidateAideNotDROverlapOnCaseAddsWarningIssueIfProviderEntered()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(1, 30, 0),
                EndTime = new TimeSpan(2, 30, 0),
                ServiceID = 9
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ManagementEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideDROverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideNotDROverlapOnCase(entry);

            Assert.IsTrue(_serviceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(_serviceMock.Object.Issues.HasWarnings == true);
        }

        [TestMethod]

        public void ValidateAideNotDROverlapValidatesCorrectly()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                StartTime = new TimeSpan(15, 30, 0),
                EndTime = new TimeSpan(16, 30, 0),
                ServiceID = 9
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideDROverlapHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideNotDROverlapOnCase(entry);

            Assert.IsTrue(result);
        }


        #endregion

        #region Aide Max Hours Per Day Per Case

        private List<Domain2.Hours.Hours> GetAideMaxHoursPerDayPerCaseHours()
        {


            var items = new List<Domain2.Hours.Hours>
            {
                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 1,
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ProviderID = 5
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 1,
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ProviderID = 5
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 1,
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ProviderID = 5
                },

                // different provider
                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 1,
                    Provider = new Domain2.Providers.Provider() { ID = 10 },
                    ProviderID = 10
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 1,
                    Provider = new Domain2.Providers.Provider() { ID = 10 },
                    ProviderID = 10
                },

                // overlap on a different date
                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 2, 1),
                    TotalHours = 2,
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ProviderID = 5
                }
            };

            return items;

        }

        [TestMethod]

        public void ValidateAideMaxHoursPerDayPerCaseIgnoresBCBAs()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                TotalHours = 5
            };

            ResetService();
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideMaxHoursPerDayPerCaseHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 15 });
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideMaxHoursPerDayPerCase(entry);

            Assert.IsTrue(result);
        }


        [TestMethod]

        public void ValidateAideMaxHoursPerdayPerCaseFalseIfOver()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                TotalHours = 2,
                ProviderID = 5
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideMaxHoursPerDayPerCaseHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideMaxHoursPerDayPerCase(entry);

            Assert.IsFalse(result);
        }

        [TestMethod]

        public void ValidateAideMaxHoursPerDayPerCaseIgnoresOtherDates()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 4, 1),
                TotalHours = 3
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideMaxHoursPerDayPerCaseHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideMaxHoursPerDayPerCase(entry);

            Assert.IsTrue(result);

        }

        [TestMethod]

        public void ValidateAideMaxHoursPerDayPerCaseIgnoresOtherAides()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 4, 1),
                TotalHours = 4
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideMaxHoursPerDayPerCaseHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 999, ProviderTypeID = 17 });
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideMaxHoursPerDayPerCase(entry);

            Assert.IsTrue(result);

        }

        [TestMethod]

        public void ValidateAideMaxHoursPerDayPerCaseAddsErrorIssueOnProviderEntered()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                TotalHours = 2,
                ProviderID = 5
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideMaxHoursPerDayPerCaseHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideMaxHoursPerDayPerCase(entry);

            Assert.IsTrue(_serviceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(_serviceMock.Object.Issues.HasErrors);

        }

        [TestMethod]

        public void ValidateAideMaxHoursPerDayPerCaseAddsWarningIssueOnManagementEntered()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                TotalHours = 2,
                ProviderID = 5
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ManagementEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideMaxHoursPerDayPerCaseHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideMaxHoursPerDayPerCase(entry);

            Assert.IsTrue(_serviceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(_serviceMock.Object.Issues.HasWarnings);

        }

        [TestMethod]

        public void ValidateAideMaxHoursPerDayPerCaseTrueIfNotOver()
        {

            var entry = new Domain2.Hours.Hours
            {
                Date = new DateTime(2017, 1, 1),
                TotalHours = (decimal)0.25,
                ProviderID = 5
            };

            ResetService();
            _serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ManagementEntry);
            _serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(() => _aideMaxHoursPerDayPerCaseHours);
            _serviceMock.Setup(x => x.Provider).Returns(new Domain2.Providers.Provider() { ID = 5, ProviderTypeID = 17 });
            _serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            _serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var resolver = new CaseAndProviderValidations(_serviceMock.Object);

            var result = resolver.ValidateAideMaxHoursPerDayPerCase(entry);

            Assert.IsTrue(_serviceMock.Object.Issues.Issues.Count == 0);
            Assert.IsTrue(result);
        }

        #endregion

        #region BCBA Max Hours Per Entry

        [TestMethod]

        public void ValidateBCBAMaxHoursPerEntryIgnoresNonBCBAs()
        {

            var entry = new Domain2.Hours.Hours();
            var serviceMock = new Mock<IResolutionService>();

            entry.TotalHours = 3;
            entry.ServiceID = (int)Domain2.Services.ServiceIDs.DirectSupervision;
            entry.Service = new Domain2.Services.Service() { ID = entry.ServiceID.Value };

            serviceMock
                .Setup(x => x.Provider)
                .Returns(() => new Domain2.Providers.Provider() { ProviderTypeID = (int)Domain2.Providers.ProviderTypeIDs.Aide });
            serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());

            var validator = new CaseAndProviderValidations(serviceMock.Object);

            var result = validator.ValidateBCBAMaxHoursPerEntry(entry);

            Assert.IsTrue(result);
        }

        [TestMethod]

        public void ValidateBCBAMaxHoursPerEntryIgnoresAssessments()
        {

            var entry = new Domain2.Hours.Hours();
            var serviceMock = new Mock<IResolutionService>();

            entry.TotalHours = 3;
            entry.ServiceID = (int)Domain2.Services.ServiceIDs.Assessment;
            entry.Service = new Domain2.Services.Service() { ID = entry.ServiceID.Value };

            serviceMock
                .Setup(x => x.Provider)
                .Returns(() => new Domain2.Providers.Provider() { ProviderTypeID = (int)Domain2.Providers.ProviderTypeIDs.BoardCertifiedBehavioralAnalyst });
            serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            serviceMock.Setup(x => x.AssessmentServiceIDs).Returns(new int[] { 11, 17, 18 });
            var validator = new CaseAndProviderValidations(serviceMock.Object);

            var result = validator.ValidateBCBAMaxHoursPerEntry(entry);

            Assert.IsTrue(result);
        }

        [TestMethod]

        public void ValidateBCBAMaxHoursPerEntryFalseIfOver()
        {

            var entry = new Domain2.Hours.Hours();
            var serviceMock = new Mock<IResolutionService>();

            entry.TotalHours = 3;
            entry.ServiceID = (int)Domain2.Services.ServiceIDs.DirectSupervision;
            entry.Service = new Domain2.Services.Service() { ID = entry.ServiceID.Value };

            serviceMock
                .Setup(x => x.Provider)
                .Returns(() => new Domain2.Providers.Provider() { ProviderTypeID = (int)Domain2.Providers.ProviderTypeIDs.BoardCertifiedBehavioralAnalyst });
            serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());

            var validator = new CaseAndProviderValidations(serviceMock.Object);

            var result = validator.ValidateBCBAMaxHoursPerEntry(entry);

            Assert.IsFalse(result);
        }

        [TestMethod]

        public void ValidateBCBAMaxHoursPerEntryTrueIfOk()
        {

            var entry = new Domain2.Hours.Hours();
            var serviceMock = new Mock<IResolutionService>();

            entry.TotalHours = 1;
            entry.ServiceID = (int)Domain2.Services.ServiceIDs.DirectSupervision;
            entry.Service = new Domain2.Services.Service() { ID = entry.ServiceID.Value };

            serviceMock
                .Setup(x => x.Provider)
                .Returns(() => new Domain2.Providers.Provider() { ProviderTypeID = (int)Domain2.Providers.ProviderTypeIDs.BoardCertifiedBehavioralAnalyst });
            serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());

            var validator = new CaseAndProviderValidations(serviceMock.Object);

            var result = validator.ValidateBCBAMaxHoursPerEntry(entry);

            Assert.IsTrue(result);
        }

        [TestMethod]

        public void ValidateBCBAMaxHoursPerEntryAddsErrorIssueOnProviderEntry()
        {

            var entry = new Domain2.Hours.Hours();
            var serviceMock = new Mock<IResolutionService>();

            entry.TotalHours = 3;
            entry.ServiceID = (int)Domain2.Services.ServiceIDs.DirectSupervision;
            entry.Service = new Domain2.Services.Service() { ID = entry.ServiceID.Value };

            serviceMock
                .Setup(x => x.Provider)
                .Returns(() => new Domain2.Providers.Provider() { ProviderTypeID = (int)Domain2.Providers.ProviderTypeIDs.BoardCertifiedBehavioralAnalyst });
            serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ProviderEntry);

            var validator = new CaseAndProviderValidations(serviceMock.Object);

            var result = validator.ValidateBCBAMaxHoursPerEntry(entry);

            Assert.IsTrue(serviceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(serviceMock.Object.Issues.HasErrors);
        }

        [TestMethod]

        public void ValidateBCBAMaxHoursPerEntryAddsWarningIssueOnManagementEntry()
        {

            var entry = new Domain2.Hours.Hours();
            var serviceMock = new Mock<IResolutionService>();

            entry.TotalHours = 3;
            entry.ServiceID = (int)Domain2.Services.ServiceIDs.DirectSupervision;
            entry.Service = new Domain2.Services.Service() { ID = entry.ServiceID.Value };

            serviceMock
                .Setup(x => x.Provider)
                .Returns(() => new Domain2.Providers.Provider() { ProviderTypeID = (int)Domain2.Providers.ProviderTypeIDs.BoardCertifiedBehavioralAnalyst });
            serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            serviceMock.Setup(x => x.EntryMode).Returns(Domain2.Cases.HoursEntryMode.ManagementEntry);

            var validator = new CaseAndProviderValidations(serviceMock.Object);

            var result = validator.ValidateBCBAMaxHoursPerEntry(entry);

            Assert.IsTrue(serviceMock.Object.Issues.Issues.Count == 1);
            Assert.IsTrue(serviceMock.Object.Issues.HasWarnings);
        }


        #endregion

        #region BCBA Max Assessment Hours Per Case Per Day


        private List<Domain2.Hours.Hours> GetBCBAMaxAssessmentCaseHours()
        {


            var items = new List<Domain2.Hours.Hours>
            {
                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 2,
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ServiceID = (int)Domain2.Services.ServiceIDs.Assessment,
                    Service = new Domain2.Services.Service() { ID = (int)Domain2.Services.ServiceIDs.Assessment }
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 2,
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ServiceID = (int)Domain2.Services.ServiceIDs.Assessment,
                    Service = new Domain2.Services.Service() { ID = (int)Domain2.Services.ServiceIDs.Assessment }
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 2,
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ServiceID = (int)Domain2.Services.ServiceIDs.DirectSupervision,
                    Service = new Domain2.Services.Service() { ID = (int)Domain2.Services.ServiceIDs.DirectSupervision }
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 2,
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ServiceID = (int)Domain2.Services.ServiceIDs.DirectSupervision,
                    Service = new Domain2.Services.Service() { ID = (int)Domain2.Services.ServiceIDs.DirectSupervision }
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 2,
                    Provider = new Domain2.Providers.Provider() { ID = 10 },
                    ServiceID = (int)Domain2.Services.ServiceIDs.Assessment,
                    Service = new Domain2.Services.Service() { ID = (int)Domain2.Services.ServiceIDs.Assessment }
                },

                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 1, 1),
                    TotalHours = 2,
                    Provider = new Domain2.Providers.Provider() { ID = 10 },
                    ServiceID = (int)Domain2.Services.ServiceIDs.Assessment,
                    Service = new Domain2.Services.Service() { ID = (int)Domain2.Services.ServiceIDs.Assessment }
                },

                // overlap on a different date
                new Domain2.Hours.Hours()
                {
                    Date = new DateTime(2017, 2, 1),
                    TotalHours = 2,
                    Provider = new Domain2.Providers.Provider() { ID = 5 },
                    ServiceID = (int)Domain2.Services.ServiceIDs.Assessment,
                    Service = new Domain2.Services.Service() { ID = (int)Domain2.Services.ServiceIDs.Assessment }
                }
            };

            return items;

        }

        [TestMethod]

        public void ValidateBCBAMaxAssessmentHoursPerCasePerDayIgnoresNonBCBAs()
        {

            var entry = new Domain2.Hours.Hours();
            var serviceMock = new Mock<IResolutionService>();
            var provider = new Domain2.Providers.Provider();

            entry.TotalHours = 8;
            provider.ProviderTypeID = (int)Domain2.Providers.ProviderTypeIDs.Aide;
            serviceMock.Setup(x => x.Provider).Returns(provider);

            var validator = new CaseAndProviderValidations(serviceMock.Object);

            var result = validator.ValidateBCBAMaxAssessmentHoursPerCasePerDay(entry);

            Assert.IsTrue(result);
        }

        [TestMethod]

        public void ValidateBCBAMaxAssessmentHoursPerCasePerDayIgnoresNonAssessmentServices()
        {

            var entry = new Domain2.Hours.Hours();
            var serviceMock = new Mock<IResolutionService>();
            var provider = new Domain2.Providers.Provider();

            entry.TotalHours = 8;
            entry.ServiceID = (int)Domain2.Services.ServiceIDs.DirectSupervision;
            entry.Service = new Domain2.Services.Service() { ID = entry.ServiceID.Value };
            provider.ProviderTypeID = (int)Domain2.Providers.ProviderTypeIDs.BoardCertifiedBehavioralAnalyst;
            serviceMock.Setup(x => x.Provider).Returns(provider);

            var validator = new CaseAndProviderValidations(serviceMock.Object);

            var result = validator.ValidateBCBAMaxAssessmentHoursPerCasePerDay(entry);

            Assert.IsTrue(result);
        }

        [TestMethod]

        public void ValidateBCBAMaxAssessmentHoursPerCasePerDayTrueIfNotOver()
        {

            var entry = new Domain2.Hours.Hours();
            var serviceMock = new Mock<IResolutionService>();
            var provider = new Domain2.Providers.Provider();

            entry.TotalHours = 1;
            entry.Date = new DateTime(2017, 1, 1);
            entry.ServiceID = (int)Domain2.Services.ServiceIDs.Assessment;
            entry.Service = new Domain2.Services.Service() { ID = entry.ServiceID.Value };
            entry.Provider = provider;
            provider.ID = 5;
            provider.ProviderTypeID = (int)Domain2.Providers.ProviderTypeIDs.BoardCertifiedBehavioralAnalyst;
            serviceMock.Setup(x => x.Provider).Returns(provider);
            serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(_bcbaMaxAssessmentCaseHours);
            serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var validator = new CaseAndProviderValidations(serviceMock.Object);

            var result = validator.ValidateBCBAMaxAssessmentHoursPerCasePerDay(entry);

            Assert.IsTrue(result);
        }

        [TestMethod]

        public void ValidateBCBAMaxAssessmentHoursPerCasePerDayFalseIfOver()
        {

            var entry = new Domain2.Hours.Hours();
            var serviceMock = new Mock<IResolutionService>();
            var provider = new Domain2.Providers.Provider();

            entry.TotalHours = 5;
            entry.Date = new DateTime(2017, 1, 1);
            entry.ServiceID = (int)Domain2.Services.ServiceIDs.Assessment;
            entry.Service = new Domain2.Services.Service() { ID = entry.ServiceID.Value };
            entry.Provider = provider;
            provider.ID = 5;
            provider.ProviderTypeID = (int)Domain2.Providers.ProviderTypeIDs.BoardCertifiedBehavioralAnalyst;
            serviceMock.Setup(x => x.Provider).Returns(provider);
            serviceMock.Setup(x => x.AllProposedCaseHours(It.IsAny<int>())).Returns(_bcbaMaxAssessmentCaseHours);
            serviceMock.Setup(x => x.Issues).Returns(new ValidationIssueCollection());
            serviceMock.Setup(x => x.AssessmentServiceIDs).Returns(new int[] { 11, 17, 18 });
            serviceMock.Object.AllProposedCaseHours(It.IsAny<int>()).Add(entry);

            var validator = new CaseAndProviderValidations(serviceMock.Object);

            var result = validator.ValidateBCBAMaxAssessmentHoursPerCasePerDay(entry);

            Assert.IsFalse(result);
        }

        //[TestMethod]
        //public void ValidateBCBAMaxAssessmentHoursPerCasePerDayAddsErrorIssueOnProviderEntry() {

        //}

        //[TestMethod]
        //public void ValidateBCBAMaxAssessmentHoursPerCasePerDayAddsWarningIssuesOnManagementEntry() {

        //}





        #endregion




    }
}
