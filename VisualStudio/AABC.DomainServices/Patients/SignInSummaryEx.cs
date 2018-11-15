using System;

namespace AABC.DomainServices.Patients
{
    public class SignInSummaryEx
    {

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime LastSignIn { get; set; }
        public int Count { get; set; }
    }
}