using AABC.Domain2.Cases;

namespace AABC.Domain2.Notes
{
    public class CaseNote : BaseNote<CaseNoteTask>
    {
        public int CaseID { get; set; }
        public virtual Case Case { get; set; }

        public override int GetParentID()
        {
            return CaseID;
        }

        public override void SetParentId(int parentId)
        {
            CaseID = parentId;
        }

        public override string GetPatientName()
        {
            return Case.Patient.CommonName;
        }
    }
}
