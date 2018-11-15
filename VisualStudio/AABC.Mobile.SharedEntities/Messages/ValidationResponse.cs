using AABC.Mobile.SharedEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Messages
{
	/// <summary>
	/// Class ValidationResponse.
	/// </summary>
	public class ValidationResponse
	{
		/// <summary>
		/// Gets or sets the validated session identifier that is held on the server.
		/// </summary>
		/// <value>The validated session identifier.</value>
		/// <remarks>
		///		This is the server's id for this session - it will be returned also on the initial login call in the ValidatedSession class
		///		It should be 0 if the session is not validated
		/// </remarks>
		public int ServerValidatedSessionID { get; set; }

		/// <summary>
		/// Gets or sets the errors.
		/// </summary>
		/// <remarks>
		///		Any errors stop the validation from succeeding
		/// </remarks>
		/// <value>The errors.</value>
		public List<string> Errors { get; set; }

		/// <summary>
		/// Gets or sets the warnings.
		/// </summary>
		/// <value>The warnings.</value>
		public List<string> Warnings { get; set; }

		/// <summary>
		/// Gets or sets the messages.
		/// </summary>
		/// <value>The messages.</value>
		public List<string> Messages { get; set; }

		/// <summary>
		/// Gets or sets the questions that are asked when entering notes.
		/// </summary>
		/// <remarks>
		///		These questions are asked when entering notes. The order is preserved
		/// </remarks>
		/// <value>The questions.</value>
		public List<NoteQuestion> NoteQuestions { get; set; }
	}
}
