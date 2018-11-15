using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Entities
{
	[Table("StoredCases")]
	public class StoredCase
	{
		[PrimaryKey]
		[AutoIncrement]
		public int StoredCaseID { get; set; }

		public int CaseID { get; set; }

		public string UserName { get; set; }

		public byte[] CaseDetails { get; set; }
	}
}
