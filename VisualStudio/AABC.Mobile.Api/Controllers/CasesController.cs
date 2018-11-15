using AABC.Mobile.SharedEntities.Entities;
using AABC.Mobile.SharedEntities.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using AABC.Mobile.Api.Repositories;
using AABC.Mobile.Api.Providers;

namespace AABC.Mobile.Api.Controllers
{
	[Authorize]
	[RoutePrefix("api/Cases")]
	public class CasesController : ApiController
    {

        // ======================
        // Fields and Properties
        // ======================
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICasesRepository _caseRepository;
        private ICurrentUserProvider _currentUserProvider;


        // ======================
        // Constructors
        // ======================
        public CasesController(ICasesRepository caseRepository, ICurrentUserProvider currentUserProvider) {
            _caseRepository = caseRepository;
            _currentUserProvider = currentUserProvider;
        }



        // ======================
        // Endpoints
        //      /   (GetCases)
        //      /LocationsAndServices
        //      /Validate
        //      /SessionUpdate
        // ======================

        [Route("")]
		[HttpGet]
		public InitialDataResponse GetCases()
		{
			log.Info("GetCases");
            var response = _caseRepository.GetInitialData();            
			return response;
		}

		[Route("LocationsAndServices")]
		[HttpGet]
		public LocationsAndServicesResponse GetLocationsAndServices([FromUri] string caseId, [FromUri] string date)
		{
			log.Info("GetLocationsAndServices");
			DateTime dateValue = DateTime.ParseExact(date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);
            var userID = _currentUserProvider.ProviderID;
            var response = _caseRepository.GetLocationsAndServices(int.Parse(caseId), userID, dateValue);
            return response;
		}

		[Route("Validate")]
		[HttpPost]
		public ValidationResponse PostValidate([FromBody] ValidationRequest sessionValidationRequest, [FromUri] bool save = true)
		{
			log.Info("PostValidate " + DataStringProvider.GetDataString(sessionValidationRequest));
            var response = _caseRepository.Validate(sessionValidationRequest, save);
            if (save) {
                log.Info("PostValidate Save mode, returning ServerValidatedSessionID: " + response.ServerValidatedSessionID);
            } else {
                log.Info("PostValidate Non-Save Mode, ServerValidatedSessionID: " + response.ServerValidatedSessionID);
            }
			return response;
		}

		[Route("SessionUpdate")]
		[HttpPost]
		public SessionUpdateResponse PostSessionUpdate([FromBody] SessionUpdateRequest sessionUpdateRequest)
		{
			log.Info("PostSessionUpdate " + DataStringProvider.GetDataString(sessionUpdateRequest));
            log.Info("PostSessionUpdate: ServerValidatedSessionID " + sessionUpdateRequest.SessionDetails.ServerValidatedSessionID);
            var response = _caseRepository.SessionUpdate(sessionUpdateRequest);
            return response;
		}

	}
}