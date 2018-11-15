using AABC.DomainServices.Sessions;
using System;
using System.Collections.Generic;

namespace AABC.Shared.Web.App.HoursEntry.Models
{
    public class HoursEntryVM
    {
        public bool IsOnAideLegacyMode { get; set; }
        public int? EntryID { get; set; }
        public int CaseID { get; set; }
        public int Status { get; set; }
        public bool IsAdminMode { get; set; } = false;
        public bool HasData { get; set; }
        public int? CatalystPreloadID { get; set; }
        public bool IsEditable { get; set; }
        public string NonEditableReason { get; set; }
        public int ProviderID { get; set; }
        public int ProviderTypeID { get; set; }
        public string ProviderTypeCode { get; set; }
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public int? InsuranceID { get; set; }
        public string InsuranceName { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public string Note { get; set; }
        public int? ServiceID { get; set; }
        public int? ServiceLocationID { get; set; }
        public int[] SSGCaseIDs { get; set; }
        public bool IsNonParentSSGEntry { get; set; }
        public bool IsTrainingEntry { get; set; }
        public IEnumerable<HoursEntryServiceVM> Services { get; set; } = new List<HoursEntryServiceVM>();
        public IEnumerable<HoursEntryNoteGroupVM> NoteGroups { get; set; } = new List<HoursEntryNoteGroupVM>();
        public IEnumerable<HoursEntryActivePatientVM> ActivePatients { get; set; } = new List<HoursEntryActivePatientVM>();
        public IEnumerable<ServiceLocationListItem> ServiceLocations { get; set; } = new List<ServiceLocationListItem>();
        public SessionReport SessionReport { get; set; }
    }
}