using AABC.Data.V2;
using AABC.Domain2.Referrals;
using AABC.DomainServices.Referrals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AABC.DomainServices.Tests.Unit.Referrals
{
    [TestClass]

    public class ReferralsTests
    {

        [TestMethod]
        [TestCategory("AABC.DomainServices.Referrals")]
        public void Case_and_patient_are_created_when_referral_is_accepted_and_has_no_associated_case()
        {
            var random = new Random();
            var patientId = random.Next(1, 1000);
            var caseId = random.Next(1, 1000);
            var mockedServices = CreateMockedServices(patientId, caseId);
            var service = new ReferralAcceptanceService(mockedServices.ContextMock.Object);
            var referral = new Referral()
            {
                Status = ReferralStatus.Accepted,
                GeneratedCaseID = null
            };
            service.HandleAcceptedReferral(referral);
            mockedServices.PatientDbSetMock.Verify(m => m.Add(It.IsAny<Domain2.Patients.Patient>()), Times.Once());
            mockedServices.CaseDbSetMock.Verify(m => m.Add(It.IsAny<Domain2.Cases.Case>()), Times.Once());
            mockedServices.ContextMock.Verify(m => m.SaveChanges(), Times.Exactly(3));
            Assert.AreEqual(patientId, referral.GeneratedPatientID);
            Assert.AreEqual(caseId, referral.GeneratedCaseID);
        }


        [TestMethod]
        [TestCategory("AABC.DomainServices.Referrals")]
        public void Case_and_patient_are_not_created_when_referral_is_accepted_but_has_associated_case()
        {
            var random = new Random();
            var patientId = random.Next(1, 1000);
            var caseId = random.Next(1, 1000);
            var mockedServices = CreateMockedServices(patientId, caseId);
            var service = new ReferralAcceptanceService(mockedServices.ContextMock.Object);
            var referral = new Referral()
            {
                Status = ReferralStatus.Accepted,
                GeneratedCaseID = caseId
            };
            service.HandleAcceptedReferral(referral);
            mockedServices.PatientDbSetMock.Verify(m => m.Add(It.IsAny<Domain2.Patients.Patient>()), Times.Never());
            mockedServices.CaseDbSetMock.Verify(m => m.Add(It.IsAny<Domain2.Cases.Case>()), Times.Never());
            mockedServices.ContextMock.Verify(m => m.SaveChanges(), Times.Never());
        }


        [TestMethod]
        [TestCategory("AABC.DomainServices.Referrals")]
        public void Case_and_patient_are_not_created_when_referral_is_not_accepted_and_has_associated_case()
        {
            var random = new Random();
            var patientId = random.Next(1, 1000);
            var caseId = random.Next(1, 1000);
            var mockedServices = CreateMockedServices(patientId, caseId);
            var service = new ReferralAcceptanceService(mockedServices.ContextMock.Object);
            var referral = new Referral()
            {
                Status = ReferralStatus.New,
                GeneratedCaseID = caseId
            };
            service.HandleAcceptedReferral(referral);
            mockedServices.PatientDbSetMock.Verify(m => m.Add(It.IsAny<Domain2.Patients.Patient>()), Times.Never());
            mockedServices.CaseDbSetMock.Verify(m => m.Add(It.IsAny<Domain2.Cases.Case>()), Times.Never());
            mockedServices.ContextMock.Verify(m => m.SaveChanges(), Times.Never());
        }


        [TestMethod]
        [TestCategory("AABC.DomainServices.Referrals")]
        public void Case_and_patient_are_not_created_when_referral_is_not_accepted_and_has_no_associated_case()
        {
            var random = new Random();
            var patientId = random.Next(1, 1000);
            var caseId = random.Next(1, 1000);
            var mockedServices = CreateMockedServices(patientId, caseId);
            var service = new ReferralAcceptanceService(mockedServices.ContextMock.Object);
            var referral = new Referral()
            {
                Status = ReferralStatus.New,
                GeneratedCaseID = null
            };
            service.HandleAcceptedReferral(referral);
            mockedServices.PatientDbSetMock.Verify(m => m.Add(It.IsAny<Domain2.Patients.Patient>()), Times.Never());
            mockedServices.CaseDbSetMock.Verify(m => m.Add(It.IsAny<Domain2.Cases.Case>()), Times.Never());
            mockedServices.ContextMock.Verify(m => m.SaveChanges(), Times.Never());
        }


        private static ReferralMockedServices CreateMockedServices(int patientId, int caseId)
        {
            var mServices = new ReferralMockedServices
            {
                PatientDbSetMock = new Mock<DbSet<Domain2.Patients.Patient>>()
            };
            mServices.PatientDbSetMock.Setup(m => m.Add(It.IsAny<Domain2.Patients.Patient>())).Callback((Domain2.Patients.Patient p) =>
            {
                p.ID = patientId;
            });

            mServices.CaseDbSetMock = new Mock<DbSet<Domain2.Cases.Case>>();
            mServices.CaseDbSetMock.Setup(m => m.Add(It.IsAny<Domain2.Cases.Case>())).Callback((Domain2.Cases.Case c) =>
            {
                c.ID = caseId;
            });

            var mockInsuranceSet = new Mock<DbSet<Domain2.Insurances.Insurance>>();
            var insurance = new List<Domain2.Insurances.Insurance>().AsQueryable();
            mockInsuranceSet.As<IQueryable<Domain2.Insurances.Insurance>>().Setup(m => m.Provider).Returns(insurance.Provider);
            mockInsuranceSet.As<IQueryable<Domain2.Insurances.Insurance>>().Setup(m => m.Expression).Returns(insurance.Expression);
            mockInsuranceSet.As<IQueryable<Domain2.Insurances.Insurance>>().Setup(m => m.ElementType).Returns(insurance.ElementType);
            mockInsuranceSet.As<IQueryable<Domain2.Insurances.Insurance>>().Setup(m => m.GetEnumerator()).Returns(insurance.GetEnumerator());

            mServices.ContextMock = new Mock<CoreContext>();
            mServices.ContextMock.Setup(m => m.Patients).Returns(mServices.PatientDbSetMock.Object);
            mServices.ContextMock.Setup(m => m.Cases).Returns(mServices.CaseDbSetMock.Object);
            mServices.ContextMock.Setup(m => m.Insurances).Returns(mockInsuranceSet.Object);
            return mServices;
        }

        public class ReferralMockedServices
        {
            public Mock<CoreContext> ContextMock { get; set; }
            public Mock<DbSet<Domain2.Patients.Patient>> PatientDbSetMock { get; set; }
            public Mock<DbSet<Domain2.Cases.Case>> CaseDbSetMock { get; set; }
        }
    }
}
