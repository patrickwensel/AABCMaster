using System;
using System.Collections.Generic;
using System.Linq;
using AABC.Mobile.SharedEntities.Messages;
using AABC.Mobile.SharedEntities.Entities;
using AABC.Domain2.Providers;
using AABC.Mobile.Api.Mappings;
using HoursProcessor = AABC.Shared.Web.App.HoursEntry;

namespace AABC.Mobile.Api.Repositories
{
    public class CasesRepository : ICasesRepository
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Fields
        Data.V2.CoreContext _context;
        ICurrentUserProvider _currentUser;
        ISettingsProvider _settings;


        // Mapping Providers
        CaseMapping caseMapping = new CaseMapping();
        ValidatedSessionMapping validatedSessionMapping = new ValidatedSessionMapping();
        HoursEntryMappings hoursEntryMappings;


        // Constructors
        public CasesRepository(ICurrentUserProvider currentUserProvider, IDBContextProvider dBContextProvider, ISettingsProvider settingsRepository)
        {
            _currentUser = currentUserProvider;
            _context = dBContextProvider.CoreContext;
            _settings = settingsRepository;
            hoursEntryMappings = new HoursEntryMappings(_currentUser.Provider, _context);
        }



        // Public Methods

        public InitialDataResponse GetInitialData()
        {

            var activeCases = _currentUser.Provider.GetActiveCasesAtDate(DateTime.Now);

            var result = new InitialDataResponse
            {
                Settings = _settings.AllClientAppSettings,
                Cases = new List<Case>(),
                ValidatedSessions = new List<ValidatedSession>()
            };

            foreach (var c in activeCases)
            {
                result.Cases.Add(caseMapping.FromDomain(c));
                var validatedSessions = c.GetPrecheckedSessions().Where(x => x.Date >= DateTime.Now).ToList();
                foreach (var session in validatedSessions)
                {
                    result.ValidatedSessions.Add(validatedSessionMapping.FromDomain(session));
                }
            }

            return result;
        }

        public LocationsAndServicesResponse GetLocationsAndServices(int caseID, int providerID, DateTime date)
        {

            var c = _context.Cases.Find(caseID);
            var provider = _context.Providers.Find(providerID);
            var domainLocations = _context.ServiceLocations.ToList();
            var localLocations = new List<Location>();

            var sp = new DomainServices.Services.ServiceProvider(_context);
            var services = sp.GetServices(c, provider, date);

            var response = new LocationsAndServicesResponse
            {
                Services = new List<Service>()
            };

            // the api expects locations per-service, but the system tracks them as a standalone
            // create a list of mapped locations that we can apply to each api service object
            foreach (var loc in domainLocations)
            {
                localLocations.Add(new Location()
                {
                    ID = loc.ID,
                    Description = loc.Name
                });
            }

            foreach (var service in services)
            {
                response.Services.Add(new Service()
                {
                    ID = service.ID,
                    Description = service.Name,
                    Locations = localLocations,
                });
            }

            return response;
        }



        public ValidationResponse Validate(ValidationRequest sessionValidationRequest, bool save)
        {

            // this runs on basic validations
            // does not have notes
            // for manual hours entry, save will be False
            // for session prechecks, save will be true

            var hoursEntryModel = hoursEntryMappings.FromSessionValidationRequest(sessionValidationRequest);
            var apiResponse = new ValidationResponse();
            HoursProcessor.Models.Response.HoursEntryResponseVM hoursEntryResponse = null;


            var hoursEntryService = new Services.HoursEntryService();
            if (save)
            {
                hoursEntryModel.Status = (int)Domain2.Hours.HoursStatus.PreChecked;
                hoursEntryResponse = hoursEntryService.SubmitHoursForProviderAppPreCheck(hoursEntryModel);
            }
            else
            {
                hoursEntryResponse = hoursEntryService.SubmitHoursForProviderAppManualEntryInitialValidation(hoursEntryModel);
            }


            var messages = hoursEntryResponse.Messages;
            apiResponse.Errors = messages.Where(x => x.Severity == HoursProcessor.Models.Response.MessageSeverity.Error).Select(x => x.Message).ToList();
            apiResponse.Warnings = messages.Where(x => x.Severity == HoursProcessor.Models.Response.MessageSeverity.Warning).Select(x => x.Message).ToList();
            apiResponse.Messages = messages.Where(x => x.Severity == HoursProcessor.Models.Response.MessageSeverity.General).Select(x => x.Message).ToList();

            if (hoursEntryResponse.WasProcessed)
            {
                log.Info("Validate Complete, Was Processed with Hours ID: " + hoursEntryResponse.HoursID.Value);
                apiResponse.ServerValidatedSessionID = hoursEntryResponse.HoursID.Value;
            }
            else
            {
                log.Info("Validate Complete, Was Not Processed, therefore Hours ID: 0");
                apiResponse.ServerValidatedSessionID = 0;
            }

            if (_currentUser.Provider.ProviderTypeID == (int)ProviderTypeIDs.BoardCertifiedBehavioralAnalyst)
            {
                throw new NotImplementedException();
            }
            else
            {
                // Aides only have one required not for now...
                apiResponse.NoteQuestions = new List<NoteQuestion>
                {
                    new NoteQuestion()
                    {
                        NoteQuestionID = 0,
                        Question = "Please enter your session notes"
                    }
                };
            }

            return apiResponse;
        }



        public SessionUpdateResponse SessionUpdate(SessionUpdateRequest sessionUpdateRequest)
        {
            // This runs on final entry
            // requires full notes, etc
            var hoursEntryModel = hoursEntryMappings.FromSessionUpdateRequest(sessionUpdateRequest);
            var apiResponse = new SessionUpdateResponse();
            var hoursEntryService = new Services.HoursEntryService();
            try
            {
                var hoursEntryResponse = hoursEntryService.SubmitHoursRequest(hoursEntryModel, DomainServices.HoursResolution.EntryApp.ProviderApp);
                apiResponse.Success = hoursEntryResponse.WasProcessed;
                if (!hoursEntryResponse.WasProcessed)
                {
                    log.Warn("Expected hours submission failed with the following: " + string.Join(";", hoursEntryResponse.Messages.Select(x => x.Message).ToArray()));
                }
            }
            catch (Exception e)
            {
                log.Error("Error in Session Update", e);
                apiResponse.Success = false;
            }
            return apiResponse;
        }



    }
}