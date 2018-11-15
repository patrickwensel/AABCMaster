using System.Collections.Generic;

namespace AABC.Web.Models.Patients
{
    public class RestaffReasonVM
    {

        public int ID { get; set; }
        public int? ReasonID { get; set; }
        public List<RestaffReasonListItem> SelectionList { get; set; }

    }
}