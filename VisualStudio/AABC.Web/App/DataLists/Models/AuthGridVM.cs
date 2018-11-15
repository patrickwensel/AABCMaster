using System.Collections.Generic;

namespace AABC.Web.Models.DataLists
{
    public class AuthGridVM
    {

        public List<AuthGridListItem> Items { get; set; }

    }

    public class AuthGridListItem
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

}