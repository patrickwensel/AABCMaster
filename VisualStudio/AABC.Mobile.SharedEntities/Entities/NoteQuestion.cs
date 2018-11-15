using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Entities
{
	/// <summary>
	/// Class NoteQuestion.
	/// </summary>
	/// <remarks>
	///	This question is asked when entering notes. 
	/// </remarks>
	public class NoteQuestion
	{
		/// <summary>
		/// Gets or sets the note question identifier.
		/// </summary>
		/// <value>The note question identifier.</value>
		public int NoteQuestionID {  get; set; }

		/// <summary>
		/// Gets or sets the question.
		/// </summary>
		/// <value>The question.</value>
		public string Question { get; set; }
	}
}
