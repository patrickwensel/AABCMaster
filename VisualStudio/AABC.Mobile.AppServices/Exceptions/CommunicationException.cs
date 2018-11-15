using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Exceptions
{
	/// <summary>
	/// Class CommunicationException.
	/// </summary>
	/// <seealso cref="System.Exception" />
	public class CommunicationException : Exception
	{
		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value>The error message.</value>
		public String ErrorMessage
		{
			get { return ErrorMessages.Any() ? ErrorMessages[0] : null; }
		}

		/// <summary>
		/// Gets or sets the error messages.
		/// </summary>
		/// <value>The error messages.</value>
		public IList<String> ErrorMessages { get; internal set; }



		/// <summary>
		/// Initializes a new instance of the <see cref="CommunicationException"/> class.
		/// </summary>
		/// <param name="errorMessages">The error messages.</param>
		public CommunicationException(IList<String> errorMessages)
		{
			ErrorMessages = errorMessages;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommunicationException"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public CommunicationException(String errorMessage)
		{
			ErrorMessages = new List<string> { errorMessage };
		}
	}
}
