using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AABC.Domain2.Cases.Tests
{
    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.Domain2.Cases.MonthlyPeriods")]
    public class MonthlyPeriodTests
    {

        [TestMethod]
        public void IsFullyParentApprovedFalseIfUnapprovedHours() {

            var c = new Case();
            c.Hours = new List<Hours.Hours>();
            c.Hours.Add(new Hours.Hours()
            {
                Date = new DateTime(2016, 11, 1)
            });
            var sut = new MonthlyPeriod(2016, 11, c);

            Assert.IsFalse(sut.IsFullyParentApproved);

        }

        [TestMethod]
        public void IsFullyParentApprovedTrue() {

            var c = new Case();
            c.Hours = new List<Hours.Hours>();
            c.Hours.Add(new Hours.Hours()
            {
                Date = new DateTime(2016, 11, 1),
                ParentApprovalID = 0
            });
            var sut = new MonthlyPeriod(2016, 11, c);

            Assert.IsTrue(sut.IsFullyParentApproved);

        }

    }

}

