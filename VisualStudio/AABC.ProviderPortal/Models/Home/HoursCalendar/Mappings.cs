using AABC.Domain2.Integrations.Catalyst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.ProviderPortal.Models.Home.HoursCalendar.Mappings
{

    public class AppointmentDTO
    {
        public int UniqueID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool AllDay { get; set; }
        public int Type { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }
        public int Label { get; set; }
        public int Status { get; set; }
        public int ResourceID { get; set; }
        public string CustomField1 { get; set; }


        public static IEnumerable<AppointmentDTO> MapFromDomainAppointments(IEnumerable<Domain.Cases.CaseAuthorizationHours> hours)
        {
            return hours.Select(MapFromDomainAppointment);
        }


        public static IEnumerable<AppointmentDTO> MapFromCatalystPreloadEntries(IEnumerable<TimesheetPreloadEntry> catalystHours)
        {
            return catalystHours.Select(MapFromCatalystPreloadEntry);
        }


        /// <summary>
        /// Converts an instance of CaseAuthorizationHours to an AppointmentDTO.
        /// </summary>
        /// <param name="hours">CaseAuthorizationHours instance to map.</param>
        /// <returns></returns>
        private static AppointmentDTO MapFromDomainAppointment(Domain.Cases.CaseAuthorizationHours hours)
        {
            var dto = new AppointmentDTO
            {
                UniqueID = hours.ID.Value,
                StartDate = new DateTime(hours.Date.Year, hours.Date.Month, hours.Date.Day, hours.TimeIn.Hour, hours.TimeIn.Minute, 0),
                EndDate = new DateTime(hours.Date.Year, hours.Date.Month, hours.Date.Day, hours.TimeOut.Hour, hours.TimeOut.Minute, 0),
                Subject = hours.Service?.Name,
                Description = null,
                Location = null,
                AllDay = false,
                Type = 0,
                RecurrenceInfo = null,
                ReminderInfo = null,
                Label = GetDTOLabelFromDomainStatus(hours.Status),
                Status = 0,
                ResourceID = 0, // must have resource mapped! (use ResourceDTO.GetDefaultResource())
                CustomField1 = null
            };
            return dto;
        }


        private static AppointmentDTO MapFromCatalystPreloadEntry(TimesheetPreloadEntry data)
        {
            var dto = new AppointmentDTO
            {
                UniqueID = data.ID * -1,
                StartDate = data.Date.Date,
                EndDate = data.Date.Date,
                Subject = GetCatalystDescription(data.Notes),
                Description = null,
                Location = null,
                AllDay = true,
                Type = 0,
                RecurrenceInfo = null,
                ReminderInfo = null,
                Label = 0, // white
                Status = 0,
                ResourceID = 0,
                CustomField1 = null
            };
            return dto;
        }


        private static string GetCatalystDescription(string notes)
        {
            var length = 30;
            var s = "Catalyst: ";
            if (notes.Length <= length)
            {
                return s + notes;
            }
            else
            {
                return s + notes.Substring(0, length) + "...";
            }
        }


        private static int GetDTOLabelFromDomainStatus(Domain.Cases.AuthorizationHoursStatus status)
        {

            // labels per devex https://documentation.devexpress.com/#AspNet/CustomDocument3811
            /*
             * white:       0
             * red:         1
             * blue:        2
             * green:       3
             * light tan:   4
             * dark tan:    5
             * aqua         6
             * olive        7
             * purple       8
             * teal         9
             * yellow       10
             * 
             * */
            switch (status)
            {
                case Domain.Cases.AuthorizationHoursStatus.PreCheckedOnApp:
                    return 4;
                case Domain.Cases.AuthorizationHoursStatus.Pending:
                    return 1;
                case Domain.Cases.AuthorizationHoursStatus.ComittedByProvider:
                    return 3;
                case Domain.Cases.AuthorizationHoursStatus.FinalizedByProvider:
                    return 2;
                case Domain.Cases.AuthorizationHoursStatus.ScrubbedByAdmin:
                    return 2;
                case Domain.Cases.AuthorizationHoursStatus.ProcessedComplete:
                    return 2;
                default:
                    return 2;
            }
        }

    }

    public class ResourceDTO
    {
        public int UniqueID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int Color { get; set; }
        public object Image { get; set; }
        public string CustomField1 { get; set; }

        public static ResourceDTO GetDefaultResource()
        {
            return new ResourceDTO()
            {
                UniqueID = 0,
                ResourceID = 0,
                ResourceName = "Default"
            };
        }
    }


}