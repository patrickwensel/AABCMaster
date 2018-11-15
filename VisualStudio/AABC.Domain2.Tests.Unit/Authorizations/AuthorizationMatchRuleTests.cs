using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AABC.Domain2.Authorizations.Tests.Unit
{
    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.Domain2.Authorizations")]
    public class AuthorizationMatchRuleTests
    {
        [TestMethod]        
        public void FinalAuthRemovesInitialUnitSize() {

            var rule = new AuthorizationMatchRule();

            rule.InitialAuthorization = new AuthorizationCode();
            rule.FinalAuthorization = new AuthorizationCode();

            rule.InitialMinimumMinutes = 16;
            rule.InitialUnitSize = 30;
            rule.FinalMinimumMinutes = 16;
            rule.FinalUnitSize = 30;

            var sut = rule.GetApplicableFinalMinutes(120);

            Assert.AreEqual(90, sut);

        }

        [TestMethod]
        public void FinalAuthRoundsToLowerUnitSize() {

            var rule = new AuthorizationMatchRule();

            rule.InitialAuthorization = new AuthorizationCode();
            rule.FinalAuthorization = new AuthorizationCode();

            rule.InitialMinimumMinutes = 16;
            rule.InitialUnitSize = 30;
            rule.FinalMinimumMinutes = 16;
            rule.FinalUnitSize = 30;

            var sut = rule.GetApplicableFinalMinutes(130);

            Assert.AreEqual(90, sut);

        }

        [TestMethod]
        public void FinalAuthRoundsToUpperUnitSize() {

            var rule = new AuthorizationMatchRule();

            rule.InitialAuthorization = new AuthorizationCode();
            rule.FinalAuthorization = new AuthorizationCode();

            rule.InitialMinimumMinutes = 16;
            rule.InitialUnitSize = 30;
            rule.FinalMinimumMinutes = 16;
            rule.FinalUnitSize = 30;

            var sut = rule.GetApplicableFinalMinutes(140);

            Assert.AreEqual(120, sut);

        }






        [TestMethod]
        public void InitialMinutesUnderMinReturnsZero() {

            var rule = new AuthorizationMatchRule();

            rule.InitialAuthorization = new AuthorizationCode();
            rule.InitialMinimumMinutes = 16;
            rule.InitialUnitSize = 30;

            var sut = rule.GetApplicableInitialMinutes(15);

            Assert.AreEqual(0, sut);

        }

        [TestMethod]
        public void InitialMinutesUnderMaxReturnsUnitSizeRounded() {

            var rule = new AuthorizationMatchRule();

            rule.InitialAuthorization = new AuthorizationCode();
            rule.InitialMinimumMinutes = 16;
            rule.InitialUnitSize = 30;

            var sut = rule.GetApplicableInitialMinutes(35);

            Assert.AreEqual(30, sut);
        }

        [TestMethod]
        public void InitialMinutesOverMaxReturnsMaxWhenFinalAuthPresent() {

            var rule = new AuthorizationMatchRule();

            rule.InitialAuthorization = new AuthorizationCode();
            rule.FinalAuthorization = new AuthorizationCode();
            rule.InitialMinimumMinutes = 16;
            rule.InitialUnitSize = 30;

            var sut = rule.GetApplicableInitialMinutes(120);

            Assert.AreEqual(30, sut);
            

        }

        [TestMethod]
        public void InitialMinutesOverMaxReturnsAllMinutesWhenFinalAuthNotPresent() {

            var rule = new AuthorizationMatchRule();

            rule.InitialAuthorization = new AuthorizationCode();
            rule.InitialMinimumMinutes = 16;
            rule.InitialUnitSize = 30;

            var sut = rule.GetApplicableInitialMinutes(120);

            Assert.AreEqual(120, sut);

        }

        [TestMethod]
        public void InitialMinutesRoundsUpAfterInitialMin() {

            var rule = new AuthorizationMatchRule();

            rule.InitialAuthorization = new AuthorizationCode();
            rule.InitialMinimumMinutes = 16;
            rule.InitialUnitSize = 30;

            var sut = rule.GetApplicableInitialMinutes(78);

            Assert.AreEqual(90, sut);

        }

    }
}
