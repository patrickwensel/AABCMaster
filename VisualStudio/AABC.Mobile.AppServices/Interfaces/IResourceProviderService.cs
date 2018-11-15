using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.AppServices.Interfaces
{
	/// <summary>
	/// Interface IResourceProviderService
	/// </summary>
	public interface IResourceProviderService
	{
		/// <summary>
		/// Gets the image from resources.
		/// </summary>
		/// <param name="resourceName">Name of the resource.</param>
		/// <returns>Xamarin.Forms.ImageSource.</returns>
		ImageSource GetImageFromResources(string resourceName);

		/// <summary>
		/// Gets the bytes from resource.
		/// </summary>
		/// <param name="resourceName">Name of the resource.</param>
		/// <returns>System.Byte[].</returns>
		byte[] GetBytesFromResource(string resourceName);

		/// <summary>
		/// Gets the string from resource.
		/// </summary>
		/// <param name="resourceName">Name of the resource.</param>
		/// <returns>System.String.</returns>
		string GetStringFromResource(string resourceName);
	}
}
