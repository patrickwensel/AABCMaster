using AABC.Mobile.SharedEntities.Entities;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.Pages.C1
{
	[AddINotifyPropertyChangedInterface]
	class EntryBaseInfoPageModel
	{
		public Case Case { get; set; }

		public DateTime DateOfService { get; set; }

		public bool Busy { get; set; }

		public Service SelectedService { get; set; }

		public Location SelectedLocation { get; set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan EndTime { get; set; }
	}
}
