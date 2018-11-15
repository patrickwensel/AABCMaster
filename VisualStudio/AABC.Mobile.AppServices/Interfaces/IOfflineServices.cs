using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Interfaces
{
	public interface IOfflineServices
	{
		bool ValidateCredentials(string username, string password);

		void SaveCredentials(string username, string password);
	}
}
