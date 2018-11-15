using AABC.Scheduling.Contracts;
using AABC.Scheduling.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Scheduling
{
    public class Scheduler : IScheduler
    {
        public IAppointmentRepository AppointmentRepository { get; private set; }
        private readonly IOccurrencesGenerator OccurrencesGenerator;

        public Scheduler
        (
            IAppointmentRepository appointmentRepository,
            IOccurrencesGenerator occurrencesGenerator
        )
        {
            if (appointmentRepository == null) throw new ArgumentNullException(nameof(appointmentRepository));
            if (occurrencesGenerator == null) throw new ArgumentNullException(nameof(occurrencesGenerator));
            AppointmentRepository = appointmentRepository;
            OccurrencesGenerator = occurrencesGenerator;
        }

        public IEnumerable<Appointment> GetSchedule(int caseId, int providerId, DateTime startDate, DateTime endDate)
        {
            var slots = new List<Appointment>();
            var appointments = AppointmentRepository.GetAppointments(caseId, providerId);

            var notRecurring = appointments.Where(m => m.Type == AppointmentType.NotRecurring)
                                           .Select(m => CreateAppointment(m));
            slots.AddRange(notRecurring);

            var recurring = appointments.Where(m => m.Type == AppointmentType.Recurring);
            foreach (var r in recurring)
            {
                var occurrences = OccurrencesGenerator.CreateWeeklyOccurrencies(startDate, endDate, r.Date.DayOfWeek);
                var cancellations = appointments.Where(m => m.Type == AppointmentType.Cancellation && m.RecurringAppointmentId == r.Id)
                                                .Select(m => m.Date.Date);
                var occurrencesWithoutCancelled = occurrences.Except(cancellations);
                var rSlots = occurrencesWithoutCancelled.Select(m => CreateAppointment(r, m.Date));
                slots.AddRange(rSlots);
            }
            return slots.OrderBy(x => x.Date).ThenBy(x => x.StartTime);
        }

        public static Scheduler Create()
        {
            return Create("CoreConnection");
        }

        public static Scheduler Create(string contextConnectionName) {
            var dataContext = new SchedulingDataContext(contextConnectionName);
            var repository = new AppointmentRepository(dataContext);
            var occurrencesGenerator = new OccurrencesGenerator();
            return new Scheduler(repository, occurrencesGenerator);
        }

        private Appointment CreateAppointment(AppointmentData m)
        {
            return CreateAppointment(m, m.Date);
        }

        private Appointment CreateAppointment(AppointmentData appointmentData, DateTime date)
        {
            return new Appointment()
            {
                Id = appointmentData.Id,
                Date = date,
                StartTime = appointmentData.StartTime,
                EndTime = appointmentData.EndTime
                //Type = appointmentData.Type
            };
        }



    }
}
