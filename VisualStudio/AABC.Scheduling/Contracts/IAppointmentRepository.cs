using AABC.Scheduling.Data;
using System.Collections.Generic;

namespace AABC.Scheduling.Contracts
{
    public interface IAppointmentRepository
    {
        AppointmentData GetAppointmentById(int id);
        AppointmentData CreateAppointment(AppointmentData entity);
        void UpdateAppointment(AppointmentData entity);
        void DeleteAppointment(AppointmentData entity);
        IEnumerable<AppointmentData> GetAppointments(int caseId, int providerId);
    }
}
