using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Entities
{
	public class Service
	{
		public int ID { get; set; }

		public string Description { get; set; }

		public List<Location> Locations { get; set; }

		public bool IsSsg { get; set; }
	}
}
