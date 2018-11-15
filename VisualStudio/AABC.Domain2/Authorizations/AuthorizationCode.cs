using System;
using System.Collections.Generic;

namespace AABC.Domain2.Authorizations
{
    public class AuthorizationCode
    {
        
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Authorization> Authorizations { get; set; }

    }
}
