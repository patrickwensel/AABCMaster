using AABC.PatientPortal.Infrastructure.Filters;
using System.Web.Mvc;

namespace AABC.PatientPortal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RequireAcceptedTerms());
        }
    }
}