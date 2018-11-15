using AABC.Mobile.AppServices.Entities;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.Entities
{
	[AddINotifyPropertyChangedInterface]
	public class DisplayQuestionAnswer
	{
		public CaseValidationQuestion CaseValidationQuestion { get; set; }
		public string Answer { get; set; }
		public bool DisplayError { get; set; }
		public string ErrorMessage { get; set; }
	}
}
