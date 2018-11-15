namespace AABC.Data.V2.DTOs
{
    public class ProviderPortalUserAdminListItem
    {
        public int ProviderID { get; set; }
        public int? AspNetUserID { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string UserNumber { get; set; }
        public bool? HasAppAccess { get; set; }
    }
}
