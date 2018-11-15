using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Messages
{
	public class CurrentVersionResponse
	{
		public string LatestVersion { get; set; }

		public bool UpdateRequired { get; set; }

		public bool UpdateAvailable { get; set; }
	}
}
