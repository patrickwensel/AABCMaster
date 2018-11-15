using AABC.Domain.General;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Data.Services
{
    public class ZipCodeService
    {


        public List<string> GetZipsByCounty(string state, string county) {

            var context = new Models.CoreEntityModel();
            var items = context.ZipCodes.Where(x => x.ZipState == state && x.ZipCounty == county).Select(x => x.ZipCode1).Distinct().ToList();
            return items;

        }

        public List<State> GetStates() {
            return State.GetStates();
        }

        public List<string> GetZipsByCity(string state, string city) {

            var context = new Models.CoreEntityModel();
            var items = context.ZipCodes.Where(x => x.ZipState == state && x.ZipCity == city).Select(x => x.ZipCode1).Distinct().ToList();
            return items;

        }

        public List<string> GetCitiesList(string state) {
            
            if (state == null) {
                return new List<string>();
            }

            var context = new Models.CoreEntityModel();
            var items = context.ZipCodes.Where(x => x.ZipState == state && x.ZipCity != null).Select(x => x.ZipCity).Distinct().ToList();
            return items;
        }

        public List<string> GetCountiesList(string state) {

            if (state == null) {
                return new List<string>();
            }

            var context = new Models.CoreEntityModel();
            var items = context.ZipCodes.Where(x => x.ZipState == state && x.ZipCounty != null).Select(x => x.ZipCounty).Distinct().ToList();
            return items;

        }









        string connectionString;

        public ZipCodeService() {
            this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }


    }
}
