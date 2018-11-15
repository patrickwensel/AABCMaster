using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain2.Cases.Tests.Unit
{
    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.Domain2.Cases")]
    public class CaseProviderTests
    {


        private CaseProvider getBaseBCBA() {
            var subject = new CaseProvider();
            subject.Provider = new Providers.Provider();
            subject.Provider.ProviderType = new Providers.ProviderType();
            subject.Provider.ProviderType.Code = "BCBA";
            return subject;
        }

        private CaseProvider getBaseAide() {
            var subject = new CaseProvider();
            subject.Provider = new Providers.Provider();
            subject.Provider.ProviderType = new Providers.ProviderType();
            subject.Provider.ProviderType.Code = "AIDE";
            return subject;
        }

        private CaseProvider getBaseVerifyRoles() {
            Case c = new Case();
            c.Providers = new List<CaseProvider>();
            c.Providers.Add(getBaseBCBA());
            c.Providers.Add(getBaseBCBA());
            var cp = getBaseBCBA();
            c.Providers.Add(cp);
            cp.Case = c;
            return cp;
        }




        [TestMethod]
        public void AssessorRoleFailsIfMoreThanOneAssessor() {

            CaseProvider subject = getBaseVerifyRoles();
            subject.Case.Providers.First().IsAssessor = true;
            subject.IsAssessor = true;

            bool results = subject.VerifyRoles(DateTime.Now);

            Assert.IsFalse(results);
        }

        [TestMethod]
        public void SupervisorRolesFailsIfMoreThanOneSupervisor() {

            CaseProvider subject = getBaseVerifyRoles();
            subject.Case.Providers.First().IsSupervisor = true;
            subject.IsSupervisor = true;

            bool results = subject.VerifyRoles(DateTime.Now);

            Assert.IsFalse(results);
        }

        [TestMethod]
        public void AuthorizedBCBARoleFailsIfMoreThanOneAuthorizedBCBA() {

            CaseProvider subject = getBaseVerifyRoles();
            subject.Case.Providers.First().IsAuthorizedBCBA = true;
            subject.IsAuthorizedBCBA = true;

            bool results = subject.VerifyRoles(DateTime.Now);

            Assert.IsFalse(results);
        }



        [TestMethod]
        public void AssesorRoleReturnFalseIfNotBCBA() {

            CaseProvider subject = getBaseAide();
            subject.IsAssessor = true;

            bool result = subject.VerifyRoles(DateTime.Now);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SupervisorRoleReturnsFalseIfNotBCBA() {

            CaseProvider subject = getBaseAide();
            subject.IsSupervisor = true;

            bool result = subject.VerifyRoles(DateTime.Now);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AuthorizedBCBAReturnsFalseIfNotBCBA() {

            CaseProvider subject = getBaseAide();
            subject.IsAuthorizedBCBA = true;

            bool result = subject.VerifyRoles(DateTime.Now);

            Assert.IsFalse(result);
        }

    }
}
