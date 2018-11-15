using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Interfaces
{
	/// <summary>
	/// Interface IVersionService
	/// </summary>
	public interface IVersionService
	{
		/// <summary>
		/// Versions the number.
		/// </summary>
		/// <returns>System.String.</returns>
		string VersionNumber();

		/// <summary>
		/// Systems the version.
		/// </summary>
		/// <returns>System.String.</returns>
		string SystemVersion();

		/// <summary>
		/// Systems the model.
		/// </summary>
		/// <returns>System.String.</returns>
		string SystemModel();
	}
}
