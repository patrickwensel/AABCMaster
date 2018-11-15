using System;

namespace AABC.Domain.Cases
{

    public class Authorization
    {
        public int? ID { get; set; }
        public DateTime DateCreated { get; set; }
        
        public string Code { get; set; }
        public string Description { get; set; }
        
        public Authorization() {
            DateCreated = DateTime.UtcNow;
        }

    }
}
