using AABC.DomainServices.Notes;
using System.Collections.Generic;

namespace AABC.Web.App.Models
{
    public class NotesSummaryListVM
    {

        public int ParentID { get; set; }
        public IEnumerable<NoteDetailsDTO> SummaryItems { get; set; }

        public NotesSummaryListVM()
        {
            SummaryItems = new List<NoteDetailsDTO>();
        }
    }
}