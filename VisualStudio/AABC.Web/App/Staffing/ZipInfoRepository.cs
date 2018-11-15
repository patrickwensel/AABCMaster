using AABC.Data.V2;
using System.Linq;

namespace AABC.Web.App.Staffing
{
    public class ZipInfoRepository
    {
        public string GetCounty(string zipCode)
        {
            return Context.ZipCodes.SingleOrDefault(m => m.ZipCode == zipCode || m.ZipCode == zipCode.Substring(0, 5))?.County ?? string.Empty;
        }

        private readonly CoreContext Context;

        public ZipInfoRepository()
        {
            Context = AppService.Current.DataContextV2;
        }
    }
}