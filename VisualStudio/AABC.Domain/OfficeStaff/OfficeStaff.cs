using System;

namespace AABC.Domain.OfficeStaff
{

    public class OfficeStaff
    {
        public int? ID { get; set; }
        public bool Active { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime DateCreated { get; set; }
        public string UserName { get; set; }

        public string CommonName
        {
            get
            {
                string s = "";
                if (this.FirstName != null)
                {
                    s = this.FirstName;
                }
                if (this.LastName != null)
                {
                    if (s == "")
                    {
                        s = this.LastName;
                    }
                    else
                    {
                        s += " " + this.LastName;
                    }
                }
                return s;
            }
        }

        public string FormalName
        {
            get
            {
                string s = "";
                if (this.LastName != null)
                {
                    s = this.LastName;
                }
                if (this.FirstName != null)
                {
                    if (s == "")
                    {
                        s = this.FirstName;
                    }
                    else
                    {
                        s += ", " + this.FirstName;
                    }
                }
                return s;
            }
        }

    }
}
