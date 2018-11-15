using AABC.DomainServices.Sessions;
using System;
using System.Collections.Generic;

namespace AABC.Shared.Web.App.HoursEntry.Models.Request
{
    public abstract class BaseHoursEntryRequestVM
    {
        public int? CatalystPreloadID { get; set; }
        public int? HoursID { get; set; }
        public bool AllowHasDataChanges { get; set; }
        public int Status { get; set; }
        public bool HasData { get; set; }
        public int ProviderID { get; set; }
        public int PatientID { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public int ServiceID { get; set; }
        public int? ServiceLocationID { get; set; }
        public int[] SsgIDs { get; set; }
        public bool? IgnoreWarnings { get; set; }
        public bool IsTrainingEntry { get; set; }
        public Signature[] Signatures { get; set; }

        public bool HasSignatures { get { return Signatures != null && Signatures.Length > 0; } }
    }


    public class HoursEntryRequestVM : BaseHoursEntryRequestVM
    {
        public string Note { get; set; }
        public IEnumerable<HoursEntryRequestExtendedNoteVM> ExtendedNotes { get; set; }
    }

    public class HoursEntryRequest2VM : BaseHoursEntryRequestVM
    {
        public SessionReport SessionReport { get; set; }    
    }


    public class HoursEntryRequestExtendedNoteVM
    {
        public int ID { get; set; }
        public string Answer { get; set; }
        public int TemplateID { get; set; }
    }

    public class Signature
    {
        public string Name { get; set; }
        public string Base64Signature { get; set; }
    }

}
