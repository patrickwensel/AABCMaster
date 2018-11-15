using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.Entities
{
	public class DisplayPrecheckedSession
	{
		public DateTime Start { get; set; }
		public TimeSpan Length { get; set; }
		public string Activity { get; set; }
	}
}
