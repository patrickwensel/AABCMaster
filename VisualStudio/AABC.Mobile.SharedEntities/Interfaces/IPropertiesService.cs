using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.SharedEntities.Interfaces
{
	/// <summary>
	/// Interface IPropertiesService
	/// </summary>
	public interface IPropertiesService
	{
		/// <summary>
		/// Gets the value or a specified default.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>T.</returns>
		T GetValue<T>(string key, T defaultValue = default(T));

		/// <summary>
		/// Gets the value asynchronously.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>Task&lt;T&gt;.</returns>
		Task<T> GetValueAsync<T>(string key, T defaultValue = default(T));

		/// <summary>
		/// Gets the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>T.</returns>
		T GetObject<T>(string key, T defaultValue = default(T));

		/// <summary>
		/// Gets the object asynchronous.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>Task&lt;T&gt;.</returns>
		Task<T> GetObjectAsync<T>(string key, T defaultValue = default(T));
	}
}
