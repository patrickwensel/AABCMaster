using AABC.Mobile.SharedEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Messages
{
	/// <summary>
	/// Class SessionUpdateRequest.
	/// </summary>
	public class SessionUpdateRequest
	{
		/// <summary>
		/// Gets or sets the session details.
		/// </summary>
		/// <value>The session details.</value>
		public ValidatedSession SessionDetails { get; set; }

		/// <summary>
		/// Gets or sets the question responses.
		/// </summary>
		/// <value>The question responses.</value>
		public List<NoteQuestionResponse> QuestionResponses { get; set; }

		/// <summary>
		/// Gets or sets the base64 signatures.
		/// </summary>
		/// <remarks>
		///		The first element is the provider's signature, the second one is the parent's
		/// </remarks>
		/// <value>The base64 signatures.</value>
		public List<string> Base64Signatures { get; set; }

	}
}
