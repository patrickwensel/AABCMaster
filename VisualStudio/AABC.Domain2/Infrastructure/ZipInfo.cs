namespace AABC.Domain2.Infrastructure
{
    public class ZipInfo
    {
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public int? TimeZone { get; set; }
        public bool? DaylightSavings { get; set; }
        //public string Latitude { get; set; }
        //public string Longitude { get; set; }
        public bool IsActive { get; set; }
    }
}
