namespace AABC.Domain2.Cases
{
    public class CaseBillingCorrespondence
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public System.DateTime CorrespondenceDate { get; set; }
        public int CorrespondenceTypeId { get; set; }
        public string ContactPerson { get; set; }
        public int StaffId { get; set; }
        public string Notes { get; set; }
        public string AttachmentFilename{ get; set; }

        public virtual Case Case { get; set; }
        public virtual Staff.Staff Staff { get; set; }
        public virtual CaseBillingCorrespondenceType CaseBillingCorrespondenceType { get; set; }
    }
}
