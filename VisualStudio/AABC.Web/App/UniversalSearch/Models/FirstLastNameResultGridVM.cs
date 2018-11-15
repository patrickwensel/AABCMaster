using System.Collections.Generic;

namespace AABC.Web.Models.UniversalSearch
{
    public class FirstLastNameResultGridVM
    {
        public List<EntryItem> Items { get; set; }
    }

    public class EntryItem
    {
        public int ID { get; set; }
        public string Type { get; set; }    // patient, provider or referral
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

}