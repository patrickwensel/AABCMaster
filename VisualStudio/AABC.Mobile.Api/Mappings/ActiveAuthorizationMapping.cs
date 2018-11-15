using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AABC.Mobile.SharedEntities.Entities;
using AABC.Domain2.Authorizations;

namespace AABC.Mobile.Api.Mappings
{
    class ActiveAuthorizationMapping : IDomainMapperReadonly<Authorization, ActiveAuthorization>
    {
        public ActiveAuthorization FromDomain(Authorization source) {

            var result = new ActiveAuthorization();

            result.ID = source.ID;
            result.Name = source.AuthorizationCode.Code;
            result.TimeRemainingMinutes = (int)TimeSpan.FromHours((double)source.HoursRemaining).TotalMinutes;
            
            return result;
            
        }        
    }
}