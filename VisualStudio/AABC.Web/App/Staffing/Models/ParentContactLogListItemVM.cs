using System;

namespace AABC.Web.App.Staffing.Models
{
    public class ParentContactLogListItemVM
    {
        public DateTime ContactDate { get; set; }

        public string ContactedPerson { get; set; }

        public string ContactMethod { get; set; }

        public string Notes { get; set; }
    }
}