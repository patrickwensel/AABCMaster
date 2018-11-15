namespace AABC.Domain2.Referrals
{
    public class BaseReferralInfo
    {
        public string GuardianFirstName { get; set; }
        public string GuardianLastName { get; set; }
        public string GuardianRelationship { get; set; }
        public int? GuardianRelationshipID { get; set; }
        public string GuardianEmail { get; set; }
        public string GuardianCellPhone { get; set; }
        public string GuardianHomePhone { get; set; }
        public string GuardianWorkPhone { get; set; }
        public string GuardianNotes { get; set; }

        public string Guardian2FirstName { get; set; }
        public string Guardian2LastName { get; set; }
        public int? Guardian2RelationshipID { get; set; }
        public string Guardian2Email { get; set; }
        public string Guardian2CellPhone { get; set; }
        public string Guardian2HomePhone { get; set; }
        public string Guardian2WorkPhone { get; set; }
        public string Guardian2Notes { get; set; }

        public string Guardian3FirstName { get; set; }
        public string Guardian3LastName { get; set; }
        public int? Guardian3RelationshipID { get; set; }
        public string Guardian3Email { get; set; }
        public string Guardian3CellPhone { get; set; }
        public string Guardian3HomePhone { get; set; }
        public string Guardian3WorkPhone { get; set; }
        public string Guardian3Notes { get; set; }


        public string PhysicianName { get; set; }
        public string PhysicianAddress { get; set; }
        public string PhysicianPhone { get; set; }
        public string PhysicianFax { get; set; }
        public string PhysicianEmail { get; set; }
        public string PhysicianContact { get; set; }
        public string PhysicianNotes { get; set; }

    }
}
