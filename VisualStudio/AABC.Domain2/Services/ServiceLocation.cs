using System;

namespace AABC.Domain2.Services
{
    public class ServiceLocation
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public bool Active { get; set; }
        public string Name { get; set; }
        public int? MBHID { get; set; }
    }
}
