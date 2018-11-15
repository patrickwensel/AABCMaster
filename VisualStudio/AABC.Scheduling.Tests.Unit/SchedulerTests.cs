using AABC.Scheduling.Contracts;
using AABC.Scheduling.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Scheduling.Tests
{
    [TestClass]
    [TestCategory("__Unit")]
    [TestCategory("_U_AABC.Scheduling")]
    public class SchedulerTests
    {
        [TestMethod]
        public void SchedulerAppointmentsReturnsMondaysInMonth() {
            // There are 4 Mondays in August 2017
            var startDate = new DateTime(2017, 8, 1);
            var endDate = new DateTime(2017, 8, 31);
            var appointments = new List<AppointmentData>();

            appointments.Add(new AppointmentData
            {
                Type = AppointmentType.Recurring,
                Date = new DateTime(2017, 8, 7), // Monday
                StartTime = new TimeSpan(17, 30, 0),
                EndTime = new TimeSpan(18, 30, 0)
            });

            var mAppointmentRepository = new Mock<IAppointmentRepository>();

            mAppointmentRepository.Setup(m => m.GetAppointments(It.IsAny<int>(), It.IsAny<int>())).Returns(appointments);

            var scheduler = new Scheduler(mAppointmentRepository.Object, new OccurrencesGenerator());
            var occurrences = scheduler.GetSchedule(1, 1, startDate, endDate);

            Assert.IsTrue(occurrences.Count() == 4);
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 7)));
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 14)));
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 21)));
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 28)));
        }


        [TestMethod]
        public void SchedulerAppointmentsReturnsMondaysInMonthPlusNonRecurringAppt() {
            // There are 4 Mondays in August 2017 + an additional appointment on 2017-05-01
            var startDate = new DateTime(2017, 8, 1);
            var endDate = new DateTime(2017, 8, 31);
            var appointments = new List<AppointmentData>();

            appointments.Add(new AppointmentData
            {
                Type = AppointmentType.Recurring,
                Date = new DateTime(2017, 8, 7), // Monday
                StartTime = new TimeSpan(17, 30, 0),
                EndTime = new TimeSpan(18, 30, 0)
            });

            appointments.Add(new AppointmentData
            {
                Type = AppointmentType.NotRecurring,
                Date = new DateTime(2017, 5, 1),
                StartTime = new TimeSpan(17, 30, 0),
                EndTime = new TimeSpan(18, 30, 0)
            });

            var mAppointmentRepository = new Mock<IAppointmentRepository>();
            mAppointmentRepository.Setup(m => m.GetAppointments(It.IsAny<int>(), It.IsAny<int>())).Returns(appointments);

            var scheduler = new Scheduler(mAppointmentRepository.Object, new OccurrencesGenerator());
            var occurrences = scheduler.GetSchedule(1, 1, startDate, endDate);

            Assert.IsTrue(occurrences.Count() == 5);
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 7)));
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 14)));
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 21)));
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 28)));
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 5, 1)));
        }

        [TestMethod]
        public void SchedulerAppointmentsReturnsMondaysInMonthPlusRemoveException() {
            // There are 4 Mondays in August 2017 + but the first one has to be removed
            var startDate = new DateTime(2017, 8, 1);
            var endDate = new DateTime(2017, 8, 31);
            var appointments = new List<AppointmentData>();

            appointments.Add(new AppointmentData
            {
                Id = 1,
                Type = AppointmentType.Recurring,
                Date = new DateTime(2017, 8, 7), // Monday
                StartTime = new TimeSpan(17, 30, 0),
                EndTime = new TimeSpan(18, 30, 0)
            });

            appointments.Add(new AppointmentData
            {
                Type = AppointmentType.Cancellation,
                Date = new DateTime(2017, 8, 7),
                RecurringAppointmentId = 1
            });

            var mAppointmentRepository = new Mock<IAppointmentRepository>();
            mAppointmentRepository.Setup(m => m.GetAppointments(It.IsAny<int>(), It.IsAny<int>())).Returns(appointments);

            var scheduler = new Scheduler(mAppointmentRepository.Object, new OccurrencesGenerator());
            var occurrences = scheduler.GetSchedule(1, 1, startDate, endDate);

            Assert.IsTrue(occurrences.Count() == 3);
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 14)));
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 21)));
            Assert.IsTrue(occurrences.Any(m => m.Date == new DateTime(2017, 8, 28)));
        }
    }
}
