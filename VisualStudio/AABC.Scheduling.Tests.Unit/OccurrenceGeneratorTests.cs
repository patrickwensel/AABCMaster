
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AABC.Scheduling.Tests
{
    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.Scheduling")]
    public class OccurrencesGeneratorTests
    {

        [TestMethod]
        public void SchedulingDateUtilsReturnsNextDayOfWeekWhenFuture() {

            var expectedDate = new DateTime(2017, 8, 21); //Monday
            var startDate = new DateTime(2017, 8, 17); // Thursday

            var resultDate = DateUtils.GetNextDayOfWeek(startDate, expectedDate.DayOfWeek);

            Assert.AreEqual(expectedDate.Date, resultDate.Date);
        }

        [TestMethod]
        public void SchedulingDateUtilsReturnsNextDayOfWeekWhenSameDate() {
            var expectedDate = new DateTime(2017, 8, 21); //Monday
            var resultDate = DateUtils.GetNextDayOfWeek(expectedDate, expectedDate.DayOfWeek);
            Assert.AreEqual(expectedDate.Date, resultDate.Date);
        }

        [TestMethod]
        public void SchedulingOccurencesGeneratorReturnsOccurrencesInOneWeek() {
            var dateStart = new DateTime(2017, 8, 17); //Thursay
            var dateEnd = new DateTime(2017, 8, 23); //Wednesday
            var expectedDate = new DateTime(2017, 8, 21); // Monday
            var generator = new OccurrencesGenerator();
            var occurrences = generator.CreateWeeklyOccurrencies(dateStart, dateEnd, DayOfWeek.Monday);
            Assert.IsTrue(occurrences.Count() == 1);
            Assert.AreEqual(expectedDate, occurrences.First().Date);
        }

        [TestMethod]
        public void SchedulingOccurrenceGeneratorReturnsOccurrencesInOneWeekWhenMatchesFirstDateInRange() {
            var dateStart = new DateTime(2017, 8, 17); //Thursay
            var dateEnd = new DateTime(2017, 8, 23); //Wednesday
            var generator = new OccurrencesGenerator();
            var occurrences = generator.CreateWeeklyOccurrencies(dateStart, dateEnd, DayOfWeek.Thursday);
            Assert.IsTrue(occurrences.Count() == 1);
            Assert.AreEqual(dateStart, occurrences.First().Date);
        }

        [TestMethod]
        public void SchedulingOccurrenceGeneratorReturnsEmptyList() {
            var dateStart = new DateTime(2017, 8, 17); //Thursay
            var dateEnd = new DateTime(2017, 8, 21); //Monday
            var expectedDate = new DateTime(2017, 8, 22); //Tuesday
            var generator = new OccurrencesGenerator();
            var occurrences = generator.CreateWeeklyOccurrencies(dateStart, dateEnd, DayOfWeek.Tuesday);
            Assert.IsTrue(occurrences.Count() == 0);
        }

        [TestMethod]
        public void SchedulingOccurrenceGeneratorReturnsAllMondaysInMonth() {
            var dateStart = new DateTime(2017, 8, 1); //Tuesday
            var dateEnd = new DateTime(2017, 8, 31); //Thursday
            var generator = new OccurrencesGenerator();
            var occurrences = generator.CreateWeeklyOccurrencies(dateStart, dateEnd, DayOfWeek.Monday);
            Assert.IsTrue(occurrences.Count() == 4);
            Assert.IsTrue(occurrences.Any(m => m == new DateTime(2017, 8, 7)));
            Assert.IsTrue(occurrences.Any(m => m == new DateTime(2017, 8, 14)));
            Assert.IsTrue(occurrences.Any(m => m == new DateTime(2017, 8, 21)));
            Assert.IsTrue(occurrences.Any(m => m == new DateTime(2017, 8, 28)));

        }
    }
}
