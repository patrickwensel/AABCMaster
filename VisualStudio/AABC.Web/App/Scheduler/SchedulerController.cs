using AABC.Scheduling;
using AABC.Scheduling.Contracts;
using AABC.Scheduling.Data;
using AABC.Web.App.Scheduler.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AABC.Web.App.Scheduler
{
    public class SchedulerController : Controller
    {
        private readonly IScheduler Scheduler;

        public SchedulerController()
        {
            Scheduler = Scheduling.Scheduler.Create();
        }

        public PartialViewResult Index(int caseID, int providerID)
        {
            var context = AppService.Current.DataContextV2;
            var c = context.Cases.Find(caseID);
            var auths = c.GetActiveAuthorizations();
            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var min = auths.Select(m => m.StartDate).DefaultIfEmpty(firstDayOfMonth).Min();
            var max = auths.Select(m => m.EndDate).DefaultIfEmpty(lastDayOfMonth).Max();
            var model = new SchedulerDataObject
            {
                Appointments = Scheduler.GetSchedule(caseID, providerID, min, max).Select(m => new AppointmentForSchedulerViewModel(m)),
                Resources = SchedulerStorageProvider.GetResources(caseID, providerID)
            };
            return PartialView("Calendar", model);
        }

        [HttpGet]
        public PartialViewResult CreateAppointment(int caseId, int providerId, DateTime date)
        {
            var model = new AppointmentCreateViewModel()
            {
                CaseId = caseId,
                ProviderId = providerId,
                Date = date
            };
            return PartialView("AppointmentCreatePopUp", model);
        }

        [HttpPost]
        public ActionResult CreateAppointment(AppointmentCreateRequest model)
        {
            var data = new AppointmentData();
            data.Type = model.Type;
            data.CaseId = model.CaseId;
            data.ProviderId = model.ProviderId;
            data.Date = model.Date.Date;
            data.StartTime = model.StartTime.TimeOfDay;
            data.EndTime = model.EndTime.TimeOfDay;
            Scheduler.AppointmentRepository.CreateAppointment(data);
            return Content("Ok");
        }

        [HttpPost]
        public ActionResult CreateAppointmentFromRecurring(AppointmentCreateFromRecurringRequest model)
        {
            var appointment = Scheduler.AppointmentRepository.GetAppointmentById(model.RecurringAppointmentId);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            if (appointment.Type != AppointmentType.Recurring)
            {
                return Content("Error: only recurring appointments can be cancelled.");
            }
            if (model.OccurrenceDate == model.Date && appointment.StartTime == model.StartTime.TimeOfDay && appointment.EndTime == model.EndTime.TimeOfDay)
            {
                return Content("Not Ok");
            }
            var data = new AppointmentData();
            data.Type = AppointmentType.NotRecurring;
            data.CaseId = appointment.CaseId;
            data.ProviderId = appointment.ProviderId;
            data.Date = model.Date.Date;
            data.StartTime = model.StartTime.TimeOfDay;
            data.EndTime = model.EndTime.TimeOfDay;
            Scheduler.AppointmentRepository.CreateAppointment(data);
            var cancellationAppointment = new AppointmentData
            {
                Type = AppointmentType.Cancellation,
                RecurringAppointmentId = appointment.Id,
                CaseId = appointment.CaseId,
                ProviderId = appointment.ProviderId,
                Date = model.OccurrenceDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime
            };
            Scheduler.AppointmentRepository.CreateAppointment(cancellationAppointment);
            return Content("Ok");
        }

        [HttpGet]
        public ActionResult EditAppointment(int appointmentId, DateTime occurrenceDate)
        {
            var appointment = Scheduler.AppointmentRepository.GetAppointmentById(appointmentId);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            var model = new AppointmentEditViewModel()
            {
                AppointmentId = appointment.Id,
                IsRecurring = appointment.Type == AppointmentType.Recurring,
                Date = appointment.Date,
                OccurrenceDate = occurrenceDate,
                StartTime = DateTime.Now.Date + appointment.StartTime,
                EndTime = DateTime.Now.Date + appointment.EndTime
            };
            return PartialView("AppointmentEditPopUp", model);
        }

        [HttpPost]
        public ActionResult EditAppointment(int appointmentId, AppointmentEditRequest model)
        {
            var data = Scheduler.AppointmentRepository.GetAppointmentById(appointmentId);
            if (data == null)
            {
                return HttpNotFound();
            }
            if (data.Type == AppointmentType.NotRecurring)
            {
                data.Date = model.Date;
            }
            data.StartTime = model.StartTime.TimeOfDay;
            data.EndTime = model.EndTime.TimeOfDay;
            Scheduler.AppointmentRepository.UpdateAppointment(data);
            return Content("Ok");
        }

        [HttpPost]
        public ActionResult DeleteAppointment(int appointmentId)
        {
            var data = Scheduler.AppointmentRepository.GetAppointmentById(appointmentId);
            if (data == null)
            {
                return HttpNotFound();
            }
            Scheduler.AppointmentRepository.DeleteAppointment(data);
            return Content("Ok");
        }

        [HttpPost]
        public ActionResult CancelAppointment(int appointmentId, DateTime date)
        {
            var data = Scheduler.AppointmentRepository.GetAppointmentById(appointmentId);
            if (data == null)
            {
                return HttpNotFound();
            }
            if (data.Type != AppointmentType.Recurring)
            {
                return Content("Error: only recurring appointments can be cancelled.");
            }
            var cancellationAppointment = new AppointmentData
            {
                Type = AppointmentType.Cancellation,
                RecurringAppointmentId = data.Id,
                CaseId = data.CaseId,
                ProviderId = data.ProviderId,
                Date = date,
                StartTime = data.StartTime,
                EndTime = data.EndTime
            };
            Scheduler.AppointmentRepository.CreateAppointment(cancellationAppointment);
            return Content("Ok");
        }

    }
}