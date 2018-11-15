using System.Collections.Generic;

namespace AABC.Shared.Web.App.HoursEntry.Models
{
    public class HoursEntryNoteGroupVM
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public List<HoursEntryNoteVM> Notes { get; set; } = new List<HoursEntryNoteVM>();
    }
}