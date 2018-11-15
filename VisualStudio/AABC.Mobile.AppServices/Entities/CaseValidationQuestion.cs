using AABC.Mobile.SharedEntities.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Entities
{
	[Table("CaseValidationQuestions")]
	public class CaseValidationQuestion : NoteQuestion
	{
		[PrimaryKey]
		[AutoIncrement]
		public int CaseValidationQuestionID { get; set; }

		public int CaseValidationID { get; set; }

		public int DisplayOrder {  get; set; }
	}
}
