using System;

namespace AABC.Domain.Cases
{
    public class Service
    {
        public int? ID { get; set; }
        public DateTime DateCreated { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
