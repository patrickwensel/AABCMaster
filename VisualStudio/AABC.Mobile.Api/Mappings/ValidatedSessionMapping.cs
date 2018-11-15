using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AABC.Domain2.Hours;
using AABC.Mobile.SharedEntities.Entities;


namespace AABC.Mobile.Api.Mappings
{
    public class ValidatedSessionMapping : IDomainMapper<Hours, ValidatedSession>
    {

        
        public ValidatedSession FromDomain(Hours source, CaseValidationState state) {

            var session = new ValidatedSession();

            session.CaseID = source.CaseID;
            session.DateOfService = source.Date;
            session.Duration = source.EndTime - source.StartTime;
            session.LocationDescription = source.ServiceLocation.Name;
            session.LocationID = source.ServiceLocationID.Value;    // ensure all validated sessions have a Location
            session.ServerValidatedSessionID = source.ID;
            session.ServiceDescription = source.Service.Name;
            session.ServiceID = source.ServiceID.Value;
            session.SsgCaseIds = string.Join(",", source.SSGCaseIDs);
            session.StartTime = source.StartTime;
            session.State = state;

            return session;
        }

        public ValidatedSession FromDomain(Hours source) {
            return FromDomain(source, CaseValidationState.None);
        }

        public Hours ToDomain(ValidatedSession source) {
            throw new NotImplementedException();
        }
    }
}