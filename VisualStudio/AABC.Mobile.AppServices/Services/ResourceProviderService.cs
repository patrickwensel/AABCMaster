using AABC.Mobile.AppServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.AppServices.Services
{
	/// <summary>
	/// Class ResourceProviderService.
	/// </summary>
	public class ResourceProviderService : IResourceProviderService
	{
		/// <summary>
		/// The resource assembly
		/// </summary>
		Assembly _resourceAssembly;

		/// <summary>
		/// The resource prefix
		/// </summary>
		string _resourcePrefix;

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceProviderService" /> class.
		/// </summary>
		/// <param name="resourceAssembly">The resource assembly.</param>
		/// <param name="resourcePrefix">The resource prefix.</param>
		public ResourceProviderService(Assembly resourceAssembly, string resourcePrefix)
		{
			_resourceAssembly = resourceAssembly;
			_resourcePrefix = resourcePrefix;
		}

		/// <summary>
		/// Gets the bytes from resource.
		/// </summary>
		/// <param name="resourceName">Name of the resource.</param>
		/// <returns>System.Byte[].</returns>
		public byte[] GetBytesFromResource(string resourceName)
		{
			using (Stream stream = _resourceAssembly.GetManifestResourceStream(_resourcePrefix + resourceName))
			{
				using (var memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					return memoryStream.ToArray();
				}
			}
		}

		/// <summary>
		/// Gets the image from resources.
		/// </summary>
		/// <param name="resourceName">Name of the resource.</param>
		/// <returns>Xamarin.Forms.ImageSource.</returns>
		public ImageSource GetImageFromResources(string resourceName)
		{
			return ImageSource.FromResource(_resourcePrefix + resourceName, _resourceAssembly);
		}

		/// <summary>
		/// Gets the string from resource.
		/// </summary>
		/// <param name="resourceName">Name of the resource.</param>
		/// <returns>System.String.</returns>
		public string GetStringFromResource(string resourceName)
		{
			using (Stream stream = _resourceAssembly.GetManifestResourceStream(_resourcePrefix + resourceName))
			{
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}
