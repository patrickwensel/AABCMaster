using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AABC.Mobile.SharedEntities.Messages;
using AABC.Mobile.SharedEntities.Entities;

namespace AABC.Mobile.Api.Repositories
{
    public interface ICasesRepository
    {

        InitialDataResponse GetInitialData();
        LocationsAndServicesResponse GetLocationsAndServices(int caseID, int providerID, DateTime date);
        ValidationResponse Validate(ValidationRequest sessionValidationRequest, bool save);
        SessionUpdateResponse SessionUpdate(SessionUpdateRequest sessionUpdateRequest);
    }
}