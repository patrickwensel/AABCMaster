using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Entities
{
	public class NoteQuestionResponse
	{
		/// <summary>
		/// Gets or sets the note question identifier.
		/// </summary>
		/// <value>The note question identifier.</value>
		public int NoteQuestionID {  get; set; }

		/// <summary>
		/// Gets or sets the answer to the question.
		/// </summary>
		/// <value>The answer.</value>
		public string Answer { get; set; }
	}
}
