using System;
using System.Collections.Generic;

namespace AABC.Domain2.PatientPortal
{
    public class WebMembershipDetail
    {
        
        public int ID { get; set; }
        public string Password { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLockoutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public DateTime FailedPasswordWindowStart { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        public virtual Login User { get; set; }
        public virtual ICollection<PatientPortalSignIn> PatientPortalSignIns { get; set; }
        
    }
}
