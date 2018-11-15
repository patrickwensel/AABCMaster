using System;
using System.Collections.Generic;

namespace AABC.ProviderPortal.Models.Home
{
    public class CaseHoursDetailVM
    {

        public List<CaseHoursDetailItem> Items { get; set; }
        public List<CaseHoursEntryItem> EntryItems { get; set; }
        public List<Domain.Cases.Service> ServicesList { get; set; }

        public bool IsSupervisor { get; set; }
        public bool ShowAllHours { get; set; }

        public DateTime? AdditionDate { get; set; }
        public DateTime? AdditionTimeIn { get; set; }
        public DateTime? AdditionTimeOut { get; set; }
        public double? AdditionHours { get; set; }
        public Domain.Cases.Service AdditionService { get; set; }
        public string AdditionNotes { get; set; }


        public CaseHoursDetailVM() {
            Items = new List<CaseHoursDetailItem>();
            ServicesList = new List<Domain.Cases.Service>();
        }

    }


    public class CaseHoursEntryItem
    {
        public int GridRowID { get; set; }
        public int CaseID { get; set; }
        public int? AuthID { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Domain.Cases.Service Service { get; set; }
        public double? Hours { get; set; }
        public string Notes { get; set; }
    }

    public class CaseHoursDetailItem
    {
        public int ID { get; set; }
        public int CaseID { get; set; }
        public string AuthCode { get; set; }
        public string ProviderName { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Domain.Cases.Service Service { get; set; }
        public double Hours { get; set; }
        public string Notes { get; set; }
    }

}