namespace AABC.Domain.Admin
{

    public enum Permissions
	{
		UserManagement = 0,
		ReferralDelete = 1,
		PatientDelete = 2,
		OfficeStaffDelete = 3,
		ProviderHoursView = 4,
		CaseHoursView = 5,
        ProviderRateView = 6,
        InsuranceEdit = 7
	}


	public class Permission
	{
		public Permissions ID { get; set; }
		public PermissionGroup PermissionGroup { get; set; }
		public string PermissionName { get; set; }
		public string Description { get; set; }

		public Permission() {
		}

	}
}