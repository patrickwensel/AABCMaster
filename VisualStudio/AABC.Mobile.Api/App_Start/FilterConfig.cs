using AABC.Mobile.Api.Filters;
using System.Configuration;
using System.Web;
using System.Web.Http.Filters;
// using System.Web.Mvc;

namespace AABC.Mobile.Api.App_Start
{
	/// <summary>
	/// Class FilterConfig.
	/// </summary>
	public class FilterConfig
	{
		/// <summary>
		/// Registers the HTTP filters.
		/// </summary>
		/// <param name="filters">The filters.</param>
		public static void RegisterHttpFilters(HttpFilterCollection filters)
		{
			filters.Add(new LogFilterAttribute());
			filters.Add(new NoCacheAttribute());
#if !DEBUG
			// are we going to allow http communication
			var allowHttpSetting = ConfigurationManager.AppSettings["AllowHttp"];
			bool allowHttp;
			if (!bool.TryParse(allowHttpSetting, out allowHttp))
			{
				allowHttp = false;
			}

			if (!allowHttp)
			{
				// make sure we are using https
				filters.Add(new RequireHttpsAttribute());
			}
#endif
		}
	}
}
