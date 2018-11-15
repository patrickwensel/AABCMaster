using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Entities
{
	[Table("CaseValidationQuestionAnswers")]
	public class CaseValidationQuestionAnswer
	{
		[PrimaryKey]
		public int CaseValidationQuestionID { get; set; }

		public string Answer { get; set; }
	}
}
