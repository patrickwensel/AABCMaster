using System;

namespace AABC.Web.Models.Cases
{
    public class TimeBillGridAuthItemVM
    {
        public int? ID { get; set; }
        public int AuthID { get; set; }
        public int ClassID { get; set; }
        public string Class { get; set; }
        public string Code { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}