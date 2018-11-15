using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain2.Cases.Tests
{
    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.Domain2.Cases")]
    public class CaseTests
    {


        [TestMethod]        
        public void GetAssesorAtDateGetsCorrectAssessor() {

            Case c = new Case();
            c.Providers = new List<CaseProvider>();

            c.Providers.Add(new CaseProvider() { ID = 0, StartDate = null, EndDate = null, IsAssessor = true });
            c.Providers.Add(new CaseProvider() { ID = 1, StartDate = null, EndDate = null });

            var subject = c.GetAssessorAtDate(DateTime.Now);

            Assert.IsTrue(subject.ID == 0);
        }

        [TestMethod]        
        public void GetSupervisorAtDateGetsCorrectSupervisor() {

            Case c = new Case();
            c.Providers = new List<CaseProvider>();

            c.Providers.Add(new CaseProvider() { ID = 0, StartDate = null, EndDate = null, IsSupervisor = true });
            c.Providers.Add(new CaseProvider() { ID = 1, StartDate = null, EndDate = null });

            var subject = c.GetSupervisorAtDate(DateTime.Now);

            Assert.IsTrue(subject.ID == 0);
        }


        [TestMethod]        
        public void GetAuthorizedBCBAAtDateGetsCorrectAuthorizedBCBA() {

            Case c = new Case();
            c.Providers = new List<CaseProvider>();

            c.Providers.Add(new CaseProvider() { ID = 0, StartDate = null, EndDate = null, IsAuthorizedBCBA = true });
            c.Providers.Add(new CaseProvider() { ID = 1, StartDate = null, EndDate = null });

            var subject = c.GetAuthorizedBCBAAtDate(DateTime.Now);

            Assert.IsTrue(subject.ID == 0);
        }


        [TestMethod]        
        public void GetProvidersAtDateWithNoProvidersReturnsEmptyList() {
            Case c = new Case();
            c.Providers = null;

            var subject = c.GetProvidersAtDate(DateTime.Now);

            Assert.IsInstanceOfType(subject, typeof(List<CaseProvider>));
        }

        [TestMethod]        
        public void GetProvidersAtDateIncludesNullStartAndEndDates() {


            Case c = new Case();
            c.Providers = new List<CaseProvider>();
            c.Providers.Add(new CaseProvider() { StartDate = null, EndDate = null });
            c.Providers.Add(new CaseProvider() { StartDate = null, EndDate = null });
            c.Providers.Add(new CaseProvider() { StartDate = null, EndDate = null });

            var subject = c.GetProvidersAtDate(DateTime.Now);

            Assert.IsTrue(subject.Count == 3);
        }

        [TestMethod]        
        public void GetProvidersAtDateIncludesExplicitlyRanged() {

            DateTime startDate = new DateTime(2000, 1, 1);
            DateTime endDate = startDate.AddMonths(1);
            DateTime refDate = startDate.AddDays(10);
            Case c = new Case();

            c.Providers = new List<CaseProvider>();
            c.Providers.Add(new CaseProvider() { StartDate = startDate, EndDate = endDate });
            c.Providers.Add(new CaseProvider() { StartDate = startDate, EndDate = endDate });
            c.Providers.Add(new CaseProvider() { StartDate = startDate, EndDate = endDate });
            // excluded
            c.Providers.Add(new CaseProvider() { StartDate = startDate.AddYears(1), EndDate = endDate.AddYears(1) });

            var subject = c.GetProvidersAtDate(refDate);

            Assert.IsTrue(subject.Count == 3);
        }

        [TestMethod]        
        public void GetProvidersAtDateIncludesMixedNullableAndRanged() {

            DateTime refdate = new DateTime(2000, 6, 1);
            Case c = new Case();
            c.Providers = new List<CaseProvider>();

            // one to include explicitly (id #1)
            c.Providers.Add(new CaseProvider() { ID = 1, StartDate = new DateTime(2000, 5, 1), EndDate = new DateTime(2000, 7, 1) });
            // one to exclude explicitly, too late (id #2)
            c.Providers.Add(new CaseProvider() { ID = 2, StartDate = new DateTime(2000, 7, 1), EndDate = new DateTime(2000, 8, 1) });
            // one to exclude explicitly, too early (id #3)
            c.Providers.Add(new CaseProvider() { ID = 3, StartDate = new DateTime(2000, 4, 1), EndDate = new DateTime(2000, 5, 1) });
            // one to include implicitly, both null (id #4)
            c.Providers.Add(new CaseProvider() { ID = 4, StartDate = null, EndDate = null });
            // one to include implicitly, end null, start before ref (id #5)
            c.Providers.Add(new CaseProvider() { ID = 5, StartDate = new DateTime(2000, 5, 1), EndDate = null });
            // one to include implicitly, end date after ref, start date null (id #6)
            c.Providers.Add(new CaseProvider() { ID = 6, StartDate = null, EndDate = new DateTime(2000, 7, 1) });

            var subject = c.GetProvidersAtDate(refdate);

            Assert.IsTrue(subject.Where(x => x.ID == 1).FirstOrDefault() != null);
            Assert.IsTrue(subject.Where(x => x.ID == 2).FirstOrDefault() == null);
            Assert.IsTrue(subject.Where(x => x.ID == 3).FirstOrDefault() == null);
            Assert.IsTrue(subject.Where(x => x.ID == 4).FirstOrDefault() != null);
            Assert.IsTrue(subject.Where(x => x.ID == 5).FirstOrDefault() != null);
            Assert.IsTrue(subject.Where(x => x.ID == 6).FirstOrDefault() != null);


        }




    }
}
