using AABC.ATrack.Integrators.ProviderApp.Mappings;
using AABC.Data.V2;
using AABC.Domain2.Hours;
using AABC.Domain2.Providers;
using AABC.DomainServices.HoursResolution;
using AABC.DomainServices.Providers;
using AABC.DomainServices.Services;
using AABC.Shared.Web.App.HoursEntry.Models.Response;
using ATrack.Integrators.ProviderApp.Contracts;
using ATrack.Integrators.ProviderApp.Contracts.Entities;
using ATrack.Integrators.ProviderApp.Contracts.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using AABC.DomainServices.Sessions;
using ATrack.Integrators.ProviderApp.Contracts.Messages.Sessions;

using CustomFieldConfiguration = ATrack.Integrators.ProviderApp.Contracts.Messages.Sessions.CustomFieldConfiguration;
using MultiSelectWithResponses = ATrack.Integrators.ProviderApp.Contracts.Messages.Sessions.MultiSelectWithResponses;
using SessionReportConfiguration = ATrack.Integrators.ProviderApp.Contracts.Messages.Sessions.SessionReportConfiguration;
using TextFieldConfiguration = ATrack.Integrators.ProviderApp.Contracts.Messages.Sessions.TextFieldConfiguration;

namespace AABC.ATrack.Integrators.ProviderApp
{
    public class Integrator : IIntegrator
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CoreContext Context;
        private readonly ProviderProvider ProviderProvider;
        private readonly HoursEntryService HoursEntryService;

        public Integrator(
            CoreContext context,
            ProviderProvider providerProvider
        )
        {
            Context = context;
            ProviderProvider = providerProvider;
            HoursEntryService = new HoursEntryService(Context);
        }


        public GetCasesResponse GetCases(GetCasesRequest request)
        {
            var provider = ProviderProvider.GetProvider(request.UserProviderID);
            if (provider == null)
            {
                throw new InvalidOperationException("Unknown provider");
            }
            var result = new GetCasesResponse
            {
                //Settings = _settings.AllClientAppSettings
            };
            var cutoffDate = DateTime.Now.Date;
            var activeCases = provider?.GetActiveCasesAtDate(DateTime.Now);
            result.Cases = activeCases.Select(c => new CaseData
            {
                Case = Mapper.MapCase(c),
                ValidatedSessions = c.GetPrecheckedSessions().Where(x => x.Date >= cutoffDate).Select(Mapper.MapSession)
            });
            return result;
        }


        public LocationsAndServicesResponse GetLocationsAndServices(LocationsAndServicesRequest request)
        {
            var provider = ProviderProvider.GetProvider(request.UserProviderID);
            if (provider == null)
            {
                throw new InvalidOperationException("Unknown provider");
            }
            var c = Context.Cases.Find(request.CaseID);
            var sp = new ServiceProvider(Context);
            var services = sp.GetServices(c, provider, request.Date);
            // the api expects locations per-service, but the system tracks them as a standalone
            // create a list of mapped locations that we can apply to each api service object
            var locations = Context.ServiceLocations.ToList().Select(Mapper.MapLocation);
            var result = services.Select(m => Mapper.MapService(m, locations));
            return new LocationsAndServicesResponse { Services = result };
        }


        public ValidationResponse Validate(ValidationRequest request, bool save)
        {
            // this runs on basic validations
            // does not have notes
            // for manual hours entry, save will be False
            // for session prechecks, save will be true
            var provider = ProviderProvider.GetProvider(request.UserProviderID);
            if (provider == null)
            {
                throw new InvalidOperationException("Unknown provider");
            }
            var patientID = Context.Cases.SingleOrDefault(m => m.ID == request.RequestedValidatedSession.CaseID).PatientID;
            var hoursEntryModel = Mapper.FromSessionValidationRequest(request, provider.ID, patientID);
            var apiResponse = new ValidationResponse();
            HoursEntryResponseVM hoursEntryResponse = null;
            if (save)
            {
                hoursEntryModel.Status = (int)HoursStatus.PreChecked;
                hoursEntryResponse = HoursEntryService.SubmitHoursForProviderAppPreCheck(hoursEntryModel, true); //json, take a look at the second parameter
            }
            else
            {
                hoursEntryResponse = HoursEntryService.SubmitHoursForProviderAppManualEntryInitialValidation(hoursEntryModel, true); //json, take a look at the second parameter
            }

            var messages = hoursEntryResponse.Messages;            
            apiResponse.Errors = messages.Where(x => x.Severity == MessageSeverity.Error).Select(x => x.Message).ToList();
            apiResponse.Warnings = messages.Where(x => x.Severity == MessageSeverity.Warning).Select(x => x.Message).ToList();
            apiResponse.Messages = messages.Where(x => x.Severity == MessageSeverity.General).Select(x => x.Message).ToList();

            var messagesToLog =
                "ERRORS: " + string.Join("|", apiResponse.Errors) + Environment.NewLine +
                "WARNINGS: " + string.Join("|", apiResponse.Warnings) + Environment.NewLine +
                "MESSAGES: " + string.Join("|", apiResponse.Messages) + Environment.NewLine;

            if (hoursEntryResponse.WasProcessed)
            {
                log.Info("Validate Complete, Was Processed with Hours ID: " + hoursEntryResponse.HoursID.Value);
                log.Info(messagesToLog);
                apiResponse.ServerValidatedSessionID = hoursEntryResponse.HoursID.Value;
            }
            else
            {
                log.Info("Validate Complete, Was Not Processed, therefore Hours ID: 0");
                log.Info(messagesToLog);
                apiResponse.ServerValidatedSessionID = 0;
            }

            if (provider.ProviderTypeID == (int)ProviderTypeIDs.BoardCertifiedBehavioralAnalyst)
            {
                throw new NotImplementedException();
            }
            else
            {
                // Aides only have one required not for now...
                apiResponse.NoteQuestions = new List<NoteQuestion>
                {
                    new NoteQuestion
                    {
                        NoteQuestionID = 0,
                        Question = "Please enter your session notes"
                    }
                };
            }
            return apiResponse;
        }


        public SessionUpdateResponse SessionUpdate<TSessionUpdateRequest>(TSessionUpdateRequest request) where TSessionUpdateRequest : SessionUpdateBaseRequest
        {
            var apiResponse = new SessionUpdateResponse();
            if (request.SessionDetails.State == CaseValidationState.AbandonedAwaitingSendToServer)
            {
                // this is an abandoned session, so do nothing with it.
                // just log it.
                log.Info("Abandoned session received");
                apiResponse.Success = true;
            }
            else
            {
                var sessionUpdateRequest = request as SessionUpdateRequest;
                DomainServices.Sessions.SessionReport sessionReport = null;

                if (sessionUpdateRequest != null && sessionUpdateRequest.SessionReportConfiguration != null)
                {
                    sessionReport = ConvertSessionReport(sessionUpdateRequest.SessionReportConfiguration);
                }

                // This runs on final entry
                // requires full notes, etc
                var provider = ProviderProvider.GetProvider(request.UserProviderID);
                if (provider == null)
                {
                    throw new InvalidOperationException("Unknown provider");
                }

                var patientID = Context.Cases.SingleOrDefault(m => m.ID == request.SessionDetails.CaseID).PatientID;
                var hoursEntryModel = Mapper.FromSessionUpdateRequest(request, sessionReport, provider.ID, provider.ProviderTypeID == (int)ProviderTypeIDs.BoardCertifiedBehavioralAnalyst, patientID);

                try
                {
                    var hoursEntryResponse = HoursEntryService.SubmitHoursRequest(hoursEntryModel, EntryApp.ProviderApp, false); //json, take a look at the second parameter
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
            }
            return apiResponse;
        }


        public SessionReportConfiguration GetSessionReportConfiguration(ValidationRequest sessionValidationRequest)
        {
            // TODO: Change this call to the real call to return the SessionReportConfiguration for the ValidationRequest services
            var aabcSessionReportConfiguration = AABC.DomainServices.Sessions.SessionReportConfiguration.CreateSample();

            // convert the aabcSessionReportConfiguration to an ATrack SessionReportConfiguration
            return ConvertSessionReportConfiguration(aabcSessionReportConfiguration);
        }

        SessionReportConfiguration ConvertSessionReportConfiguration(DomainServices.Sessions.SessionReportConfiguration aabcSessionReportConfiguration)
        {
            var atrackSessionReportConfiguration = new SessionReportConfiguration();

            var summary = aabcSessionReportConfiguration.Fields.FirstOrDefault(f => f.Name == "Summary") as DomainServices.Sessions.TextFieldConfiguration;
            if (summary != null)
            {
                atrackSessionReportConfiguration.Summary = new TextFieldConfiguration
                { Name = summary.Name, ControlType = summary.ControlType };
            }

            var behavior = aabcSessionReportConfiguration.Fields.FirstOrDefault(f => f.Name == "Behaviors") as DomainServices.Sessions.MultiSelectFieldConfiguration<MultiSelectOption>;
            if (behavior != null)
            {
                atrackSessionReportConfiguration.Behaviors = new global::ATrack.Integrators.ProviderApp.Contracts.Messages.Sessions.MultiSelectFieldConfiguration<QuestionAnswer<string, bool>>
                {
                    Name = behavior.Name,
                    ControlType = behavior.ControlType,
                    Options = behavior.Options.Select(o =>
                            new QuestionAnswer<string, bool> { Question = o.Name })
                                                                         .ToList()
                };
            }

            var interventions = aabcSessionReportConfiguration.Fields.FirstOrDefault(f => f.Name == "Interventions") as DomainServices.Sessions.MultiSelectFieldConfiguration<DomainServices.Sessions.MultiSelectOptionWithResponses>;
            if (interventions != null)
            {
                atrackSessionReportConfiguration.Interventions = new global::ATrack.Integrators.ProviderApp.Contracts.Messages.Sessions.MultiSelectFieldConfiguration<MultiSelectWithResponses>()
                {
                    Name = interventions.Name,
                    ControlType = interventions.ControlType,
                    Options = interventions.Options.Select(o =>
                        new MultiSelectWithResponses
                        {
                            Name = o.Name,
                            Responses = o.Responses.Select(r => r.Name).ToList()
                        }).ToList()
                };
            }

            var reinforcers = aabcSessionReportConfiguration.Fields.FirstOrDefault(f => f.Name == "Reinforcers") as DomainServices.Sessions.MultiSelectFieldConfiguration<MultiSelectOption>;
            if (reinforcers != null)
            {
                atrackSessionReportConfiguration.Reinforcers = new global::ATrack.Integrators.ProviderApp.Contracts.Messages.Sessions.MultiSelectFieldConfiguration<QuestionAnswer<string, bool>>
                {
                    Name = reinforcers.Name,
                    ControlType = reinforcers.ControlType,
                    Options = reinforcers.Options.Select(o =>
                            new QuestionAnswer<string, bool> { Question = o.Name })
                                                                         .ToList()
                };
            }

            var goals = aabcSessionReportConfiguration.Fields.FirstOrDefault(f => f.Name == "Goals") as DomainServices.Sessions.CustomFieldConfiguration;
            if (goals != null)
            {
                atrackSessionReportConfiguration.Goals = new CustomFieldConfiguration
                {
                    Name = goals.Name,
                    ControlType = goals.ControlType,
                };
            }

            var barriers = aabcSessionReportConfiguration.Fields.FirstOrDefault(f => f.Name == "Barriers") as DomainServices.Sessions.MultiSelectFieldConfiguration<MultiSelectOption>;
            if (barriers != null)
            {
                atrackSessionReportConfiguration.Barriers = new global::ATrack.Integrators.ProviderApp.Contracts.Messages.Sessions.MultiSelectFieldConfiguration<QuestionAnswer<string, bool>>
                {
                    Name = barriers.Name,
                    ControlType = barriers.ControlType,
                    Options = barriers.Options.Select(o =>
                            new QuestionAnswer<string, bool> { Question = o.Name })
                                                                        .ToList()
                };
            }

            return atrackSessionReportConfiguration;
        }

        internal DomainServices.Sessions.SessionReport ConvertSessionReport(SessionReportConfiguration sessionReportConfiguration)
        {
            var response = new DomainServices.Sessions.SessionReport();

            response.Summary = sessionReportConfiguration.Summary.Answer;

            response.BehaviorsSection = new BehaviorsReportSection
            {
                Behaviors = sessionReportConfiguration.Behaviors.Options
                                                                    .Where(o => o.Answer)
                                                                    .Select(o => new Behavior
                                                                    {
                                                                        Name = o.Question,
                                                                        Description = o.Notes
                                                                    })
                                                    .ToList()
            };

            response.InterventionsSection = new InterventionsReportSection
            {
                Interventions = sessionReportConfiguration.Interventions.Options
                                                    .Where(o => !String.IsNullOrEmpty(o.Answer))
                                                    .Select(o => new Intervention
                                                    {
                                                        Name = o.Name,
                                                        Response = o.Answer,
                                                        Description = o.Notes
                                                    })
                                                    .ToList()
            };

            response.ReinforcersSection = new ReinforcersReportSection
            {
                Reinforcers = sessionReportConfiguration.Reinforcers.Options
                                                      .Where(o => o.Answer)
                                                      .Select(o => new Reinforcer
                                                      {
                                                          Name = o.Question,
                                                          Description = o.Notes
                                                      })
                                                      .ToList()
            };

            response.GoalsSection = new GoalsReportSection
            {
                Goals = new List<Goal>                                                        {
                                                            new Goal
                                                                {
                                                                    Name = sessionReportConfiguration.Goals.Answer,
                                                                    Progress = sessionReportConfiguration.Goals.Progress
                                                                }
                                                        }
                                                .ToList()
            };

            response.BarriersSection = new BarriersReportSection
            {
                Barriers = sessionReportConfiguration.Barriers.Options
                                                   .Where(o => o.Answer)
                                                   .Select(o => new Barrier
                                                   {
                                                       Name = o.Question,
                                                       Description = o.Notes
                                                   })
                                                   .ToList()
            };

            return response;
        }
    }
}
