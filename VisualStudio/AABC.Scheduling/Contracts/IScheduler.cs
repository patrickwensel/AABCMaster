using System;
using System.Collections.Generic;

namespace AABC.Scheduling.Contracts
{
    public interface IScheduler
    {
        IAppointmentRepository AppointmentRepository { get; }

        IEnumerable<Appointment> GetSchedule(int caseId, int providerId, DateTime startDate, DateTime endDate);
    }
}
