using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.SharedEntities.Entities;
using AABC.Mobile.SharedEntities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Interfaces
{
	public interface IDataUpdateService
	{
		Task<InitialDataResponse> GetInitialData();

		Task<LocationsAndServicesResponse> GetLocationsAndServices(int caseId, DateTime date);

		Task<ValidationResponse> ValidateRequest(ValidatedSession caseAuthorizationRequest, bool save = true);

		Task<SessionUpdateResponse> PostSessionUpdateRequestToServer(SessionUpdateRequest sessionUpdateRequest);
	}
}
