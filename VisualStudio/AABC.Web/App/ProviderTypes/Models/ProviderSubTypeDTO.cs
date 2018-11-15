namespace AABC.Web.App.ProviderTypes.Models
{
    public class ProviderSubTypeDTO
    {
        public int Id { get; set; }
        public int ProviderTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsBeingUsed { get; set; }
    }
}