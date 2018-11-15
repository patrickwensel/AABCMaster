using AABC.Mobile.AppServices.Helpers;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.SharedEntities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Attributes;

namespace AABC.Mobile.AppServices.Services
{
	/// <summary>
	/// Class EmbeddedResourcePropertiesService.
	/// </summary>
	public class EmbeddedResourcePropertiesService : IPropertiesService
	{
		const string _resourceName = "Properties.json";

		Dictionary<string, string> _propertyValues;

		IResourceProviderService _resourceProviderService;

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbeddedResourcePropertiesService"/> class.
		/// </summary>
		[InjectionConstructor]
		public EmbeddedResourcePropertiesService(IResourceProviderService resourceProviderService)
		{
			_resourceProviderService = resourceProviderService;
			_propertyValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(_resourceProviderService.GetStringFromResource(_resourceName));
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="EmbeddedResourcePropertiesService" /> class.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <param name="resourceName">Name of the resource.</param>
		public EmbeddedResourcePropertiesService(Assembly assembly, string resourceName)
		{
		}


		/// <summary>
		/// Gets the value or a specified default.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>T.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public T GetValue<T>(string key, T defaultValue = default(T))
		{
			string stringValue;
			if (_propertyValues.TryGetValue(key, out stringValue))
			{
				return (T)Convert.ChangeType(stringValue, typeof(T));
			}
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Gets the value asynchronously.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>Task&lt;T&gt;.</returns>
		public Task<T> GetValueAsync<T>(string key, T defaultValue = default(T))
		{
			// just call the non-async value and return that
			return Task.FromResult<T>(GetValue<T>(key, defaultValue));
		}


		/// <summary>
		/// Gets the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>T.</returns>
		public T GetObject<T>(string key, T defaultValue = default(T))
		{
			string value = GetValue<string>(key);

			return ParseValue(value, defaultValue);
		}


		/// <summary>
		/// get object as an asynchronous operation.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>Task&lt;T&gt;.</returns>
		public async Task<T> GetObjectAsync<T>(string key, T defaultValue = default(T))
		{
			string value = await GetValueAsync<string>(key);

			return ParseValue(value, defaultValue);
		}

		/// <summary>
		/// Parses the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>T.</returns>
		T ParseValue<T>(string value, T defaultValue)
		{
			if (value == null)
			{
				return defaultValue;
			}
			else
			{
				var val = JsonConvert.DeserializeObject<T>(value);
				return val;
			}
		}

	}
}
