using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Interfaces
{
	public interface ISettingsService
	{
		Task Initialize();

		bool Functionality(string functionality);

		Task UpdateSettings(IEnumerable<KeyValuePair<string, object>> settings);

		T Setting<T>(string key);
	}
}
