using DevExpress.Web.Mvc;
using System.Collections;
using System.Collections.Generic;

namespace AABC.Web.App.Scheduler.Models
{


    //public class SchedulerDataHelper
    //{
    //    public static System.Collections.IEnumerable GetFakeResources(int caseID, int providerID)
    //    {

    //        var resources = new List<ResourceDTO>();

    //        resources.Add(new ResourceDTO()
    //        {
    //            ResourceID = 1,
    //            ResourceName = "uncommitted",
    //            Color = System.Drawing.Color.Red.ToArgb()
    //        });

    //        resources.Add(new ResourceDTO()
    //        {
    //            ResourceID = 2,
    //            ResourceName = "committed",
    //            Color = System.Drawing.Color.Green.ToArgb()
    //        });

    //        return resources;


    //    }



    //    public static System.Collections.IEnumerable GetResources(int caseID, int providerID)
    //    {

    //        var resources = new List<ResourceDTO>();

    //        resources.Add(ResourceDTO.GetDefaultResource());

    //        return resources;

    //    }

    //    public static System.Collections.IEnumerable GetAppointments(int caseID, int providerID)
    //    {

    //        var cutoffDate = getCurrentHoursHistoryCutoff();

    //        // return appointments
    //        var appts = new List<AppointmentDTO>();
    //        var caseHours = new Data.Services.CaseService().GetCaseHoursByCaseByProvider(caseID, providerID, cutoffDate);


    //        appts = AppointmentDTO.MapFromDomainAppointments(caseHours);

    //        return appts;

    //    }

    //    public static SchedulerDataObject DataObject(int caseID, int providerID)
    //    {

    //        var sdo = new SchedulerDataObject();

    //        sdo.Resources = GetResources(caseID, providerID);
    //        sdo.Appointments = GetAppointments(caseID, providerID);

    //        return sdo;
    //    }


    //    private static DateTime getCurrentHoursHistoryCutoff()
    //    {
    //        // only show log and calendar history for current month and last month
    //        // get the furthest history date to retrieve hours for

    //        var currentDate = DateTime.Now;
    //        var firstOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
    //        var firstOfPreviousMonth = firstOfCurrentMonth.AddMonths(-1);

    //        return firstOfPreviousMonth;

    //    }








    //    public static System.Collections.IEnumerable GetFakeAppointments(int caseID, int providerID)
    //    {

    //        // return appointments
    //        var appts = new List<AppointmentDTO>();






    //        var appt = new AppointmentDTO();
    //        appt.AllDay = false;
    //        appt.Description = "description";
    //        appt.EndDate = new DateTime(2016, 5, 10, 9, 30, 0);
    //        appt.StartDate = new DateTime(2016, 5, 10, 8, 30, 0);
    //        appt.Subject = "subject";
    //        appt.UniqueID = 0;
    //        appt.CustomField1 = null;
    //        appt.Label = 3;
    //        appt.Location = null;
    //        appt.RecurrenceInfo = null;
    //        appt.ReminderInfo = null;
    //        appt.ResourceID = 1;
    //        appt.Status = 0;
    //        appt.Type = 0;


    //        appts.Add(appt);

    //        appt = new AppointmentDTO();
    //        appt.AllDay = false;
    //        appt.Description = "description 2";
    //        appt.EndDate = new DateTime(2016, 5, 12, 9, 30, 0);
    //        appt.StartDate = new DateTime(2016, 5, 12, 8, 30, 0);
    //        appt.Subject = "subject 2";
    //        appt.UniqueID = 1;
    //        appt.CustomField1 = null;
    //        appt.Label = 1;
    //        appt.Location = null;
    //        appt.RecurrenceInfo = null;
    //        appt.ReminderInfo = null;
    //        appt.ResourceID = 1;
    //        appt.Status = 0;
    //        appt.Type = 0;

    //        appts.Add(appt);



    //        appt = new AppointmentDTO();
    //        appt.AllDay = false;
    //        appt.Description = "description 2";
    //        appt.EndDate = new DateTime(2016, 5, 12, 11, 30, 0);
    //        appt.StartDate = new DateTime(2016, 5, 12, 12, 30, 0);
    //        appt.Subject = "long subject goes here (long enough to word wrap)";
    //        appt.UniqueID = 1;
    //        appt.CustomField1 = null;
    //        appt.Label = 3;
    //        appt.Location = null;
    //        appt.RecurrenceInfo = null;
    //        appt.ReminderInfo = null;
    //        appt.ResourceID = 1;
    //        appt.Status = 0;
    //        appt.Type = 0;

    //        appts.Add(appt);



    //        appt = new AppointmentDTO();
    //        appt.AllDay = false;
    //        appt.Description = "description 2";
    //        appt.EndDate = new DateTime(2016, 5, 8, 11, 30, 0);
    //        appt.StartDate = new DateTime(2016, 5, 8, 12, 30, 0);
    //        appt.Subject = "long subject goes here (long enough to word wrap)";
    //        appt.UniqueID = 1;
    //        appt.CustomField1 = null;
    //        appt.Label = 1;
    //        appt.Location = null;
    //        appt.RecurrenceInfo = null;
    //        appt.ReminderInfo = null;
    //        appt.ResourceID = 2;
    //        appt.Status = 0;
    //        appt.Type = 0;

    //        appts.Add(appt);









    //        appt = new AppointmentDTO();
    //        appt.AllDay = false;
    //        appt.Description = "description 2";
    //        appt.EndDate = new DateTime(2016, 5, 13, 6, 30, 0);
    //        appt.StartDate = new DateTime(2016, 5, 13, 8, 30, 0);
    //        appt.Subject = "long subject goes here (long enough to word wrap)";
    //        appt.UniqueID = 1;
    //        appt.CustomField1 = null;
    //        appt.Label = 1;
    //        appt.Location = null;
    //        appt.RecurrenceInfo = null;
    //        appt.ReminderInfo = null;
    //        appt.ResourceID = 2;
    //        appt.Status = 0;
    //        appt.Type = 0;

    //        appts.Add(appt);

    //        appt = new AppointmentDTO();
    //        appt.AllDay = false;
    //        appt.Description = "description 2";
    //        appt.EndDate = new DateTime(2016, 5, 13, 9, 30, 0);
    //        appt.StartDate = new DateTime(2016, 5, 13, 8, 30, 0);
    //        appt.Subject = "subject 2";
    //        appt.UniqueID = 1;
    //        appt.CustomField1 = null;
    //        appt.Label = 1;
    //        appt.Location = null;
    //        appt.RecurrenceInfo = null;
    //        appt.ReminderInfo = null;
    //        appt.ResourceID = 1;
    //        appt.Status = 0;
    //        appt.Type = 0;

    //        appts.Add(appt);

    //        appt = new AppointmentDTO();
    //        appt.AllDay = false;
    //        appt.Description = "description 2";
    //        appt.EndDate = new DateTime(2016, 5, 13, 9, 30, 0);
    //        appt.StartDate = new DateTime(2016, 5, 13, 8, 30, 0);
    //        appt.Subject = "subject 2, long enough to force another word wrap for 'too much' info...";
    //        appt.UniqueID = 1;
    //        appt.CustomField1 = null;
    //        appt.Label = 1;
    //        appt.Location = null;
    //        appt.RecurrenceInfo = null;
    //        appt.ReminderInfo = null;
    //        appt.ResourceID = 1;
    //        appt.Status = 0;
    //        appt.Type = 0;

    //        appts.Add(appt);

    //        return appts;

    //    }
    //}










    public class SchedulerStorageProvider
    {

        static MVCxAppointmentStorage defaultAppointmentStorage;
        static MVCxResourceStorage defaultResourceStorage;

        public static MVCxAppointmentStorage DefaultAppointmentStorage
        {
            get
            {
                if (defaultAppointmentStorage == null)
                {
                    defaultAppointmentStorage = createDefaultAppointmentStorage();
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
                    defaultResourceStorage = createDefaultResourceStorage();
                }
                return defaultResourceStorage;
            }
        }


        static MVCxAppointmentStorage createDefaultAppointmentStorage()
        {

            MVCxAppointmentStorage appointmentStorage = new MVCxAppointmentStorage();
            appointmentStorage.Mappings.AppointmentId = "Id";
            appointmentStorage.Mappings.Start = "Start";
            appointmentStorage.Mappings.End = "End";
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

        static MVCxResourceStorage createDefaultResourceStorage()
        {
            MVCxResourceStorage resourceStorage = new MVCxResourceStorage();
            resourceStorage.Mappings.ResourceId = "ResourceID";
            resourceStorage.Mappings.Caption = "ResourceName";
            return resourceStorage;
        }

        public static IEnumerable GetResources(int caseID, int providerID)
        {

            var resources = new List<ResourceDTO>();

            resources.Add(ResourceDTO.GetDefaultResource());

            return resources;

        }
    }



}


