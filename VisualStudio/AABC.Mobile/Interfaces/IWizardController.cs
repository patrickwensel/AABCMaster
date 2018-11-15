using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.Interfaces
{
	/// <summary>
	/// Interface IWizardController
	/// </summary>
	interface IWizardController
	{
		/// <summary>
		/// Moves to the next page.
		/// </summary>
		/// <param name="pageNumber">The page number.</param>
		/// <returns>Task.</returns>
		Task NextPage(int pageNumber);

		/// <summary>
		/// Cancels the wizard.
		/// </summary>
		/// <returns>Task.</returns>
		Task Cancel();
	}
}
