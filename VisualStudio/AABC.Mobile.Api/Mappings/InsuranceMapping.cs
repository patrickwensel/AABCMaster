using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AABC.Domain2.Insurances;
using AABC.Mobile.SharedEntities.Entities;

namespace AABC.Mobile.Api.Mappings
{
    class InsuranceMapping : IDomainMapperReadonly<Domain2.Insurances.Insurance, SharedEntities.Entities.Insurance>
    {
        public SharedEntities.Entities.Insurance FromDomain(Domain2.Insurances.Insurance source) {

            var result = new SharedEntities.Entities.Insurance();

            result.ID = source.ID;
            result.InsuranceName = source.Name;

            return result;
        }
    }
}