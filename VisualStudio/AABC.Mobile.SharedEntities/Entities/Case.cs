using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Entities
{
	public class Case
	{
		public int ID { get; set; }

		public string UserName { get; set; }

		public Patient Patient { get; set; }

		public Insurance ActiveInsurance { get; set; }

		public List<ActiveAuthorization> ActiveAuthorizations { get; set; }

		public bool AllowManualTime { get; set; }
	}
}
