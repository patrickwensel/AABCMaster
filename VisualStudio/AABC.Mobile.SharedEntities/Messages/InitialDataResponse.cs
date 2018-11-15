using AABC.Mobile.SharedEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Messages
{
	/// <summary>
	/// Class InitialDataResponse.
	/// </summary>
	public class InitialDataResponse
	{
		/// <summary>
		/// Gets or sets the cases.
		/// </summary>
		/// <value>The cases.</value>
		public List<Case> Cases { get; set; }

		/// <summary>
		/// Gets or sets the validated sessions.
		/// </summary>
		/// <value>The validated sessions.</value>
		public List<ValidatedSession> ValidatedSessions { get; set; }

		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		/// <value>The settings.</value>
		public List<KeyValuePair<string, object>> Settings {get; set; }
	}
}
