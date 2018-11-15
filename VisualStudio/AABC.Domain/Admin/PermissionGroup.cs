namespace AABC.Domain.Admin
{

    public enum PermissionGroups
	{
		Administrator = 1,
		Staff = 2
	}

	public class PermissionGroup
	{
		public PermissionGroups ID { get; set; }
		public string Description { get; set; }

		public PermissionGroup() {

		}
	}

}
