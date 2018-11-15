using AABC.Domain2.Providers;

namespace AABC.Domain2.Notes
{
    public class ProviderNote : BaseNote<ProviderNoteTask>
    {
        public int ProviderID { get; set; }
        public virtual Provider Provider { get; set; }

        public override int GetParentID()
        {
            return ProviderID;
        }

        public override void SetParentId(int parentId)
        {
            ProviderID = parentId;
        }

        public override string GetPatientName()
        {
            return Provider.FirstName + " " + Provider.LastName;
        }
    }
}
