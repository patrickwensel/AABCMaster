using System;

namespace AABC.Domain.Admin
{
    public class UserOption : Option
    {
        public int? UserOptionID { get; set; }
        public DateTime UserOptionDateCreated { get; set; }
        public Option Option { get; set; }
        public string OptionValue { get; set; }
        public bool isAllowed { get; set; }

        public UserOption() {
            UserOptionDateCreated = DateTime.UtcNow;
        }
    }
}
