using System.Collections.Generic;

namespace AABC.Web.Models.Shared
{
    public class ZipLookupVM
    {
        public List<string> zluZips { get; set; }

        public Domain.General.State zluState { get; set; }
        public string zluCity { get; set; }
        public string zluCounty { get; set; }

        public List<Domain.General.State> zluStatesList { get; set; }
        public List<string> zluCitiesList { get; set; }
        public List<string> zluCountiesList { get; set; }   
    }
}