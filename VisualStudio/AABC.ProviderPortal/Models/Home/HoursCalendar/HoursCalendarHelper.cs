using AABC.ProviderPortal.Models.Home.HoursCalendar.Mappings;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.ProviderPortal.Models.Home.HoursCalendar
{
    public class SchedulerDataObject
    {
        public System.Collections.IEnumerable Appointments { get; set; }
        public System.Collections.IEnumerable Resources { get; set; }
    }

    public class SchedulerDataHelper
    {

        public static System.Collections.IEnumerable GetResources(int caseID, int providerID)
        {
            var resources = new List<ResourceDTO>
            {
                ResourceDTO.GetDefaultResource()
            };
            return resources;
        }


        public static System.Collections.IEnumerable GetAppointments(int caseID, int providerID)
        {
            var cutoffDate = GetCurrentHoursHistoryCutoff();
            var caseHours = new Data.Services.CaseService().GetCaseHoursByCaseByProvider(caseID, providerID, cutoffDate)
                .Where(m => m.Status != Domain.Cases.AuthorizationHoursStatus.PreCheckedOnApp)
                .ToList();
            // return appointments
            var appts = AppointmentDTO.MapFromDomainAppointments(caseHours).ToList();

            // only map catalyst hours if we're an Aide
            // (BCBA notes handling is not yet implemented)
            if (AppService.Current.CurrentProvider.ProviderTypeID == (int)Domain2.Providers.ProviderTypeIDs.Aide)
            {
                var catalystHours = new App.Hours.HoursService().GetCatalystPreloads(caseID, providerID, cutoffDate);
                appts.AddRange(AppointmentDTO.MapFromCatalystPreloadEntries(catalystHours));
            }
            return appts;
        }


        public static SchedulerDataObject DataObject(int caseID, int providerID)
        {
            var sdo = new SchedulerDataObject
            {
                Resources = GetResources(caseID, providerID),
                Appointments = GetAppointments(caseID, providerID)
            };
            return sdo;
        }


        private static DateTime GetCurrentHoursHistoryCutoff()
        {
            // only show log and calendar history for current month and last month
            // get the furthest history date to retrieve hours for
            var currentDate = DateTime.Now;
            var firstOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var firstOfPreviousMonth = firstOfCurrentMonth.AddMonths(-1);
            return firstOfPreviousMonth;
        }

    }



    public class SchedulerStorageProvider
    {

        private static MVCxAppointmentStorage defaultAppointmentStorage;
        private static MVCxResourceStorage defaultResourceStorage;

        public static MVCxAppointmentStorage DefaultAppointmentStorage
        {
            get
            {
                if (defaultAppointmentStorage == null)
                {
                    defaultAppointmentStorage = CreateDefaultAppointmentStorage();
                }
                return defaultAppointmentStorage;
            }
        }

        public static MVCxResourceStorage DefaultResourceStorage
        {
            get
            {
                if (defaultResourceStorage == null)
                {
                    defaultResourceStorage = CreateDefaultResourceStorage();
                }
                return defaultResourceStorage;
            }
        }

        private static MVCxAppointmentStorage CreateDefaultAppointmentStorage()
        {
            var appointmentStorage = new MVCxAppointmentStorage();
            appointmentStorage.Mappings.AppointmentId = "UniqueID";
            appointmentStorage.Mappings.Start = "StartDate";
            appointmentStorage.Mappings.End = "EndDate";
            appointmentStorage.Mappings.Subject = "Subject";
            appointmentStorage.Mappings.Description = "Description";
            appointmentStorage.Mappings.Location = "Location";
            appointmentStorage.Mappings.AllDay = "AllDay";
            appointmentStorage.Mappings.Type = "Type";
            appointmentStorage.Mappings.RecurrenceInfo = "RecurrenceInfo";
            appointmentStorage.Mappings.ReminderInfo = "ReminderInfo";
            appointmentStorage.Mappings.Label = "Label";
            appointmentStorage.Mappings.Status = "Status";
            appointmentStorage.Mappings.ResourceId = "ResourceID";
            return appointmentStorage;
        }


        private static MVCxResourceStorage CreateDefaultResourceStorage()
        {
            var resourceStorage = new MVCxResourceStorage();
            resourceStorage.Mappings.ResourceId = "ResourceID";
            resourceStorage.Mappings.Caption = "ResourceName";
            return resourceStorage;
        }

    }

}