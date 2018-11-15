using AABC.Scheduling.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Scheduling.Data
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly SchedulingDataContext DataContext;

        public AppointmentRepository
        (
            SchedulingDataContext dataContext
        )
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            DataContext = dataContext;
        }

        public IEnumerable<AppointmentData> GetAppointments(int caseId, int providerId)
        {
            return DataContext.Appointments.Where(m => m.CaseId == caseId && m.ProviderId == providerId).ToList();
        }

        public AppointmentData GetAppointmentById(int id)
        {
            return DataContext.Appointments.SingleOrDefault(m => m.Id == id);
        }

        public AppointmentData CreateAppointment(AppointmentData entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var i = DataContext.Appointments.Add(entity);
            DataContext.SaveChanges();
            return i;
        }

        public void UpdateAppointment(AppointmentData entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            DataContext.SaveChanges();
        }

        public void DeleteAppointment(AppointmentData entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            foreach (var e in entity.Cancellations.ToList())
            {
                DataContext.Appointments.Remove(e);
            }
            DataContext.Appointments.Remove(entity);
            DataContext.SaveChanges();
        }


    }
}
