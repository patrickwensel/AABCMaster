using AABC.Mobile.AppServices.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Services
{
	public class SettingsService : ISettingsService
	{
		IDatabaseService _databaseService;

		Dictionary<string, string> _settings = new Dictionary<string, string>();

		public SettingsService(IDatabaseService databaseService)
		{
			_databaseService = databaseService;
		}

		public async Task Initialize()
		{
			// load the cached settings
			UpdateSettingsDictionary(await _databaseService.LoadSettings());
		}

		public bool Functionality(string functionality)
		{
			return Setting<bool>(functionality);
		}


		void UpdateSettingsDictionary(IEnumerable<KeyValuePair<string, string>> settings)
		{
			// update the currently stored values
			_settings.Clear();
			foreach (var setting in settings)
			{
				_settings[setting.Key] = setting.Value;
			}
		}

		public T Setting<T>(string key)
		{
			string dictionaryValue;

			// if we have one
			if (_settings.TryGetValue(key, out dictionaryValue))
			{
				// and it's of the correct type
				return JsonConvert.DeserializeObject<T>(dictionaryValue);
			}

			// not found, so return the default
			return default(T);
		}

		public async Task UpdateSettings(IEnumerable<KeyValuePair<string, object>> settings)
		{
			var settingsToSave = settings.Select(s => new KeyValuePair<string, string>(s.Key, JsonConvert.SerializeObject(s.Value))).ToList();

			UpdateSettingsDictionary(settingsToSave);

			// save the JSON serialize the setting
			await _databaseService.SaveSettings(settingsToSave);
		}
	}
}
