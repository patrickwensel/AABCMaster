namespace AABC.Domain.Admin
{

    public enum Options
	{
		PatientSearchDefaultSort = 1,
		PatientSearchDefaultFilter = 2
	}

	public class Option {
		public int ID { get; set; }
		public OptionGroup OptionGroup { get; set; }
		public string OptionName { get; set; }
		public string Description { get; set; }

		public Option() {

		}
	
	}

}
