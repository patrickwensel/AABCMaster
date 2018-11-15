using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Entities
{
	/// <summary>
	/// Class ErrorResponse.
	/// </summary>
	public class ErrorResponse
	{
		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>The message.</value>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the state of the model.
		/// </summary>
		/// <value>The state of the model.</value>
		public Dictionary<string, List<string>> ModelState { get; set; }


		/// <summary>
		/// To the string list.
		/// </summary>
		/// <returns>IList&lt;System.String&gt;.</returns>
		public IList<string> ToStringList()
		{
			List<string> response = new List<string>();

			// add the model state messages
			if (ModelState != null)
			{
				foreach (var item in ModelState)
				{
					foreach (var itemEntry in item.Value)
					{
						if (!String.IsNullOrEmpty(itemEntry)) response.Add(itemEntry);
					}
				}
			}

			// if we don't have anything else, use the message
			if (!response.Any() && !String.IsNullOrEmpty(Message)) response.Add(Message);

			return response;
		}
	}
}
