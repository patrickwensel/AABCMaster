using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Entities
{
	[Table("Settings")]
	public class Setting
	{
		[PrimaryKey]
		public string Key { get; set; }

		public string JsonSerializedValue { get; set; }
	}
}
