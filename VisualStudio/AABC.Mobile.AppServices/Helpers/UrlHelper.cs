using AABC.Mobile.SharedEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Helpers
{
	public class UrlHelper
	{
		/// <summary>
		/// The scheme
		/// </summary>
		readonly string _scheme;
		/// <summary>
		/// The server address
		/// </summary>
		readonly string _serverAddress;
		/// <summary>
		/// The path
		/// </summary>
		readonly string _path;
		/// <summary>
		/// The port
		/// </summary>
		readonly int _port;

		/// <summary>
		/// Initializes a new instance of the <see cref="UrlHelper" /> class.
		/// </summary>
		/// <param name="propertiesService">The properties service.</param>
		/// <param name="propertyPrefix">The property prefix.</param>
		public UrlHelper(IPropertiesService propertiesService, string propertyPrefix)
		{
			_scheme = propertiesService.GetValue<string>(propertyPrefix + "Scheme");
			_serverAddress = propertiesService.GetValue<string>(propertyPrefix + "ServerAddress");
			_path = propertiesService.GetValue<string>(propertyPrefix + "ServicePath");
			_port = propertiesService.GetValue<int>(propertyPrefix + "Port");
		}


		/// <summary>
		/// Builds the URL.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="queryStringValues">The query string values.</param>
		/// <returns>Uri.</returns>
		public Uri BuildUrl(string command, Dictionary<string, string> queryStringValues = null)
		{
			var uriBuilder = new UriBuilder(_scheme, _serverAddress) { Path = _path + command };
			if (_port > 0)
			{
				uriBuilder.Port = _port;
			}

			// set the querystring if one is supplied
			if (queryStringValues != null)
			{
				uriBuilder.Query = String.Join("&", (from keyValuePair in queryStringValues select WebUtility.UrlEncode(keyValuePair.Key) + "=" + WebUtility.UrlEncode(keyValuePair.Value)).ToArray());
			}

			Uri uri = uriBuilder.Uri;
			return uri;
		}

		/// <summary>
		/// Builds the URL.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="id">The identifier.</param>
		/// <returns>Uri.</returns>
		public Uri BuildUrl(string command, int id)
		{
			var uriBuilder = new UriBuilder(_scheme, _serverAddress) { Path = _path + command + "/" + id };

			Uri uri = uriBuilder.Uri;
			return uri;
		}

		/// <summary>
		/// Builds the URL.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="id">The identifier.</param>
		/// <returns>Uri.</returns>
		public Uri BuildUrl(string command, string id)
		{
			var uriBuilder = new UriBuilder(_scheme, _serverAddress) { Path = _path + command + "/" + WebUtility.UrlEncode(id) };

			Uri uri = uriBuilder.Uri;
			return uri;
		}
	}
}
