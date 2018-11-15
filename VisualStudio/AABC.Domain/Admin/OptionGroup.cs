namespace AABC.Domain.Admin
{

    public enum OptionGroups
	{
		OptionGroup1 = 1,
		OptionGroup2 = 2,
		OptionGroup3 = 3
	}

	public class OptionGroup
	{
		public OptionGroups ID { get; set; }
		public string Description { get; set; }

		public OptionGroup() {

		}
	}
}

