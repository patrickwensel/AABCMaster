namespace AABC.Domain2.PatientPortal
{
    public class PatientPortalSignIn
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public System.DateTime SignInDate { get; set; }
        public string SignInType { get; set; }

        public virtual WebMembershipDetail PatientPortalWebMembership { get; set; }


    }
}
