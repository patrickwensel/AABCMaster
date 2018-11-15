using AABC.Domain.Services;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Data.Services
{
    public class ServicesService
    {

        public List<ServiceLocation> GetActiveServiceLocations() {

            var items = new List<ServiceLocation>();
            var entities = new Data.Models.CoreEntityModel().ServiceLocations.Where(x => x.Active).ToList();

            foreach (var e in entities) {
                var item = new ServiceLocation();
                item.Active = e.Active;
                item.ID = e.ID;
                item.MBHID = e.LocationMBHID;
                item.Name = e.LocationName;

                items.Add(item);
            }

            return items;
        }

        public IEnumerable<Domain2.Cases.FunctioningLevel> GetFunctioningLevels()
        {
            return new V2.CoreContext().FunctioningLevels.ToList();
        }
    }
}
