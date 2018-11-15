using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.Domain2.Hours;
using AABC.Domain2.Infrastructure;
using AABC.Domain2.Insurances;
using AABC.Domain2.Notes;
using AABC.Domain2.Providers;
using AABC.Domain2.Referrals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AABC.Data.Tests.Integration
{
    [TestClass]
    [TestCategory("__Integration")]
    [TestCategory("_I_AABC.Data.V2.ModelBuilding")]
    public class ModelBuildingTests
    {

        CoreContext context = new CoreContext();

        [TestInitialize]
        public void Initialize() {
            context.Database.Log = (s) => System.Diagnostics.Debug.WriteLine(s);
        }


        [TestMethod]
        public void EFContextBuildsProviderPortalUsers() {

            var sut = context.ProviderPortalUsers.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Providers.PortalUser));

        }

        [TestMethod]        
        public void EFContextBuildsInsuranceLocalCarrier() {
            var sut = context.InsuranceLocalCarriers.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(LocalCarrier));        }

        [TestMethod]
        public void EFContexBuildsInsuranceServiceDefault() {
            var sut = context.InsuranceServiceDefaults.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Insurances.InsuranceServiceDefault));
        }

        [TestMethod]
        public void EFContextBuildsServiceLocation() {

            var sut = context.ServiceLocations.FirstOrDefault();

            if (sut == null)
                Assert.Inconclusive();

            Assert.IsInstanceOfType(sut, typeof(Domain2.Services.ServiceLocation));
        }

        [TestMethod]
        public void EFContextBuildsCatalystHasDataEntry() {

            var sut = context.CatalystHasDataEntries.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Integrations.Catalyst.HasDataEntry));
        }

        [TestMethod]
        public void EFContextBuildsProviderTypeServices() {

            var sut = context.ProviderTypeServices.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Providers.ProviderTypeService));
        }

        [TestMethod]
        public void EFContextBuildsInsuranceService() {

            var sut = context.InsuranceServices.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Insurances.InsuranceService));
        }

        [TestMethod]
        public void EFContextBuildsExtendedNoteTemplateGroup() {

            var sut = context.ExtendedNoteTemplateGroups.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(ExtendedNoteTemplateGroup));
        }

        [TestMethod]
        public void EFContextBuildsExtendedNoteTemplate() {

            var sut = context.ExtendedNoteTemplates.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(ExtendedNoteTemplate));
        }

        [TestMethod]
        public void EFContextBuildsExtendedNote() {

            var sut = context.ExtendedNotes.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(ExtendedNote));
        }


        [TestMethod]
        public void EFContextGetsProviderRate() {

            var sut = context.ProviderRates.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Providers.ProviderRate));
        }

        [TestMethod]
        public void EFContextGetsCaseRate() {

            var sut = context.ProviderCaseRates.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Providers.CaseRate));
        }

        [TestMethod]
        public void EFContextGetsServiceRate() {

            var sut = context.ProviderServiceRates.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Providers.ServiceRate));
        }


        [TestMethod]
        public void EFContextGetsProvider() {

            var sut = context.Providers.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Providers.Provider));
        }



        [TestMethod]
        public void EFContextGetsHoursFinalization() {

            var sut = context.HoursFinalizations.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(HoursFinalization));
        }



        [TestMethod]
        public void EFContextGetsAuthMatchRule() {

            var sut = context.AuthorizationMatchRules.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Authorizations.AuthorizationMatchRule));

        }




        [TestMethod]
        public void EFContextGetsInsurance() {

            var sut = context.Insurances.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Insurances.Insurance));

        }


        [TestMethod]
        public void EFContextGetsInsuranceViaPatient() {

            var sut = context.Patients.Where(x => x.InsuranceID != null).FirstOrDefault()?.Insurance;
            
            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Insurances.Insurance));

        }


        [TestMethod]
        public void EFContextGetsAuthCode() {

            var sut = context.AuthorizationCode.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null, cannot resolve");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Authorizations.AuthorizationCode));

        }


        [TestMethod]
        public void EFContextGetsAuthClass() {

            var sut = context.AuthorizationClass.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null, cannot resolve");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Authorizations.AuthorizationClass));

        }




        [TestMethod]
        public void EFContextGetsAuthorization() {

            var sut = context.Authorizations.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null, cannot resolve");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Authorizations.Authorization));

        }


        [TestMethod]
        public void EFContextGetsParentSignature() {

            var sut = context.ParentSignatures.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null, cannot resolve");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.PatientPortal.ParentSignature));

        }

        [TestMethod]
        public void EFContextGetsParentApprovals() {

            var sut = context.ParentApprovals.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null, unable to determine");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Cases.ParentApproval));

        }


        [TestMethod]
        public void EFContextGetsMonthlyPeriod() {

            var sut = context.MonthlyPeriods.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null, unable to determine");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Cases.MonthlyPeriod));

        }

        [TestMethod]
        public void EFContextGetsPatient() {

            var sut = context.Patients.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null, unable to determine");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Patients.Patient));

        }


        [TestMethod]
        public void EFContextGetsPatientPortalLoginPatient() {

            Domain2.PatientPortal.Login login = context.PatientPortalLogins.FirstOrDefault();

            if (login == null) {
                Assert.Inconclusive("Null, unable to conclude");
            }

            var sut = login.Patients.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null, unable to conclude");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.Patients.Patient));

        }

        [TestMethod]
        public void EFContextGetsPatientPortalLogin() {

            Domain2.PatientPortal.Login sut = context.PatientPortalLogins.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("Null, unable to conclude");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.PatientPortal.Login));

        }

        [TestMethod]
        public void EFContextGetsPatientPortalWebMembershipDetail() {

            Domain2.PatientPortal.WebMembershipDetail sut = context.PatientPortalWebMembershipDetails.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("Null, unable to conclude");
            }

            Assert.IsInstanceOfType(sut, typeof(Domain2.PatientPortal.WebMembershipDetail));

        }


        [TestMethod]
        public void EFContextGetsHours() {

            Domain2.Hours.Hours sut = context.Hours.First();

            Assert.IsNotNull(sut);
        }



        [TestMethod]
        public void EFContextGetsHoursProvider() {

            Domain2.Hours.Hours sut = context.Hours.First();

            Assert.IsNotNull(sut.Provider);
        }

        [TestMethod]
        public void EFContextGetsHoursCase() {

            Domain2.Hours.Hours sut = context.Hours.First();

            Assert.IsNotNull(sut.Case);
        }


        [TestMethod]
        public void EFContextGetsHoursService() {
            var sut = context.Hours.First();
            var sut2 = sut.Service;

            Assert.IsNotNull(sut2);
        }

        [TestMethod]
        public void EFContextGetsService() {
            Domain2.Services.Service sut = context.Services.First();

            Assert.IsNotNull(sut);
        }


        [TestMethod]
        public void EFContextGetsCase() {

            Case c = context.Cases.First();

            Assert.IsNotNull(c);
            
        }

        [TestMethod]
        public void EFContextGetsCaseNote()
        {
            CaseNote n = context.CaseNotes.First();
            Assert.IsNotNull(n);
        }

        [TestMethod]
        public void EFContextGetsCaseProviderViaCase() {

            var casesWithProviders = context.Cases.Where(x => x.Providers.Count != 0);
            var subject = casesWithProviders.First();

            Assert.IsNotNull(subject);
        }

        [TestMethod]
        public void EFContextGetsProviderViaCaseProvider() {

            var cp = context.CaseProviders.First();
            var p = cp.Provider;

            Assert.IsNotNull(p);

        }

        [TestMethod]
        public void EFContextGetsProviderTypeViaProvider() {

            var p = context.Providers.First();
            var pt = p.ProviderType;

            Assert.IsNotNull(pt);

        }

        [TestMethod]
        public void EFContextGetsStaffingLog()
        {

            var staffingLog = context.StaffingLog.FirstOrDefault();

            if (staffingLog == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }

            Assert.IsInstanceOfType(staffingLog, typeof(StaffingLog));

        }


        [TestMethod]
        public void EFContextGetsStaffingLogProvider()
        {
            var staffingLogProvider = context.StaffingLogProviders.FirstOrDefault();
            if (staffingLogProvider == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }
            Assert.IsInstanceOfType(staffingLogProvider, typeof(StaffingLogProvider));
        }


        [TestMethod]
        public void EFContextGetsReferralChecklist()
        {
            var entity = context.ReferralChecklist.FirstOrDefault();
            if (entity == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }
            Assert.IsInstanceOfType(entity, typeof(ReferralChecklist));
        }


        [TestMethod]
        public void EFContextGetsReferralChecklistItem()
        {
            var entity = context.ReferralChecklistItems.FirstOrDefault();
            if (entity == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }
            Assert.IsInstanceOfType(entity, typeof(ReferralChecklistItem));
        }


        [TestMethod]
        public void EFContextGetsReferralDismissalReason()
        {
            var entity = context.ReferralDismissalReasons.FirstOrDefault();
            if (entity == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }
            Assert.IsInstanceOfType(entity, typeof(ReferralDismissalReason));
        }


        [TestMethod]
        public void EFContextGetsReferralEnumItem()
        {
            var entity = context.ReferralEnums.FirstOrDefault();
            if (entity == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }
            Assert.IsInstanceOfType(entity, typeof(ReferralEnumItem));
        }


        [TestMethod]
        public void EFContextGetsReferralNote()
        {
            var entity = context.ReferralNotes.FirstOrDefault();
            if (entity == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }
            Assert.IsInstanceOfType(entity, typeof(ReferralNote));
        }


        [TestMethod]
        public void EFContextGetsReferral()
        {
            var entity = context.Referrals.FirstOrDefault();
            if (entity == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }
            Assert.IsInstanceOfType(entity, typeof(Referral));
        }


        [TestMethod]
        public void EFContextGetsReferralSourceType()
        {
            var entity = context.ReferralSourceTypes.FirstOrDefault();
            if (entity == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }
            Assert.IsInstanceOfType(entity, typeof(ReferralSourceType));
        }


        [TestMethod]
        public void EFContextGetsLanguage()
        {
            var entity = context.Languages.FirstOrDefault();
            if (entity == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }
            Assert.IsInstanceOfType(entity, typeof(Language));
        }


        [TestMethod]
        public void EFContextGetsProviderSubType()
        {
            var entity = context.ProviderSubTypes.FirstOrDefault();
            if (entity == null)
            {
                Assert.Inconclusive("Null, unable to conclude");
            }
            Assert.IsInstanceOfType(entity, typeof(ProviderSubType));
        }
    }
}
