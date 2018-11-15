using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Entities
{
	[Table("Functionalities")]
	public class Functionality
	{
		[PrimaryKey]
		public string Key { get; set; }

		public bool Value { get; set; }
	}
}
