using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AABC.Mobile.SharedEntities.Messages;
using AABC.Mobile.SharedEntities.Entities;


namespace AABC.Mobile.Api.Repositories
{
    class CasesDemoRepository : ICasesRepository
    {

        public SessionUpdateResponse SessionUpdate(SessionUpdateRequest sessionUpdateRequest) {
            return new SessionUpdateResponse { Success = true };        }

        public ValidationResponse Validate(ValidationRequest sessionValidationRequest, bool save) {            
            var validationResponse = new ValidationResponse
            {
                ServerValidatedSessionID = 11,
                Errors = new List<string>(),
                Warnings = new List<string> { "This is a warning" },
                Messages = new List<string> { "This is a message" },
                NoteQuestions = new List<NoteQuestion>
                    {
                        new NoteQuestion { NoteQuestionID = 2, Question = "Programs Reviewed" },
                        new NoteQuestion { NoteQuestionID = 3, Question = "Behaviors Reviewed" }
                    },
            };
            if (sessionValidationRequest.RequestedValidatedSession.Duration > TimeSpan.FromMinutes(30)) {
                validationResponse.Errors.Add("The time cannot be more than 30 minutes");
            }
            return validationResponse;
        }



        public LocationsAndServicesResponse GetLocationsAndServices(int caseID, int providerID, DateTime date) {
            
            return new LocationsAndServicesResponse
            {
                Services = new List<SharedEntities.Entities.Service>
                        {
                            new SharedEntities.Entities.Service {
                                                                    ID = 9,
                                                                    Description = "Direct Care",
                                                                    Locations = new List<SharedEntities.Entities.Location>
                                                                    {
                                                                        new SharedEntities.Entities.Location { ID = 1, Description = "Home" },
                                                                        new SharedEntities.Entities.Location { ID = 2, Description = "Center" },
                                                                    },
                                                                    IsSsg = true,
                                                                },
                            new SharedEntities.Entities.Service {
                                                                    ID = 10,
                                                                    Description = "Parent Training",
                                                                    Locations = new List<SharedEntities.Entities.Location>
                                                                    {
                                                                        new SharedEntities.Entities.Location { ID = 1, Description = "Home" },
                                                                        new SharedEntities.Entities.Location { ID = 2, Description = "Center" },
                                                                        new SharedEntities.Entities.Location { ID = 3, Description = "School" },
                                                                    },
                                                                    IsSsg = false,
                                                                },
                        },
            };


        }

        public InitialDataResponse GetInitialData() {
            
            InitialDataResponse initialDataResponse = new InitialDataResponse();

			initialDataResponse.Cases = new List<SharedEntities.Entities.Case>
								{
									new SharedEntities.Entities.Case
										{
											ID = 31,
											ActiveInsurance = new SharedEntities.Entities.Insurance { ID = 1, InsuranceName = "Aetna Health" },
											ActiveAuthorizations = new List<SharedEntities.Entities.ActiveAuthorization>
												{
													new SharedEntities.Entities.ActiveAuthorization { ID = 11, Name = "Direct Care", TimeRemainingMinutes = (int)TimeSpan.FromHours(12).TotalMinutes },
													new SharedEntities.Entities.ActiveAuthorization { ID = 12, Name = "Parent Training", TimeRemainingMinutes = (int)TimeSpan.FromHours(2.5).TotalMinutes }
												},
											Patient = new SharedEntities.Entities.Patient
												{
													ID = 21,
													PatientFirstName = "Mary",
													PatientLastName = "Smith",
													Gender = Gender.Female,

													PatientAddress1 = "123 City Street",
													PatientAddress2 = "Big Town",
													PatientCity="Queens",
													PatientState="NY",
													PatientZip = "54321",

													PatientGuardianFirstName = "Jane",
													PatientGuardianLastName = "Blogs",
													PatientGuardianRelationship = "Mother",
													PatientGuardianPhone = "(555) 123-4567",

													PatientGuardian2FirstName = "Jim",
													PatientGuardian2LastName = "Blogs",
													PatientGuardian2Relationship = "Father",
													PatientGuardian2Phone = "(555) 123-4567",
												},
											AllowManualTime = true
                                        },
                                    new SharedEntities.Entities.Case
                                        {
                                            ID = 32,
                                            ActiveInsurance = new SharedEntities.Entities.Insurance { ID = 2, InsuranceName = "Aetna Health" },
                                            ActiveAuthorizations = new List<SharedEntities.Entities.ActiveAuthorization>
                                                {
                                                    new SharedEntities.Entities.ActiveAuthorization { ID = 11, Name = "Direct Care", TimeRemainingMinutes = (int)TimeSpan.FromHours(10).TotalMinutes },
                                                    new SharedEntities.Entities.ActiveAuthorization { ID = 12, Name = "Parent Training", TimeRemainingMinutes = (int)TimeSpan.FromHours(0.5).TotalMinutes }
                                                },
                                            Patient = new SharedEntities.Entities.Patient
                                                {
                                                    ID = 22,
                                                    PatientFirstName = "John",
                                                    PatientLastName = "Doe",
                                                    Gender = Gender.Male,

                                                    PatientAddress1 = "123 Potter Hill Rd.",
                                                    PatientCity="Brooklyn",
                                                    PatientState="NY",
                                                    PatientZip = "12345",

                                                    PatientGuardianFirstName = "Amy",
                                                    PatientGuardianLastName = "Doe",
                                                    PatientGuardianRelationship = "Mother",
                                                    PatientGuardianPhone = "(555) 123-4567",

                                                    PatientGuardian2FirstName = "Joseph",
                                                    PatientGuardian2LastName = "Doe",
                                                    PatientGuardian2Relationship = "Father",
                                                    PatientGuardian2Phone = "(555) 123-4567",
                                                },
											AllowManualTime = true
										},
                                    new SharedEntities.Entities.Case
                                        {
                                            ID = 33,
                                            ActiveInsurance = new SharedEntities.Entities.Insurance { ID = 3, InsuranceName = "Aetna Health" },
                                            ActiveAuthorizations = new List<SharedEntities.Entities.ActiveAuthorization>
                                                {
                                                    new SharedEntities.Entities.ActiveAuthorization { ID = 11, Name = "Direct Care", TimeRemainingMinutes = (int)TimeSpan.FromHours(12).TotalMinutes },
                                                    new SharedEntities.Entities.ActiveAuthorization { ID = 12, Name = "Parent Training", TimeRemainingMinutes = (int)TimeSpan.FromHours(2.5).TotalMinutes }
                                                },
                                            Patient = new SharedEntities.Entities.Patient
                                                {
                                                    ID = 23,
                                                    PatientFirstName = "Jamie",
                                                    PatientLastName = "Jones",
                                                    Gender = Gender.Unknown,

                                                    PatientAddress1 = "32a The Road",
                                                    PatientAddress2 = "Small Town",
                                                    PatientCity="Queens",
                                                    PatientState="NY",
                                                    PatientZip = "54321",

                                                    PatientGuardianFirstName = "Jane",
                                                    PatientGuardianLastName = "Jones",
                                                    PatientGuardianRelationship = "Mother",
                                                    PatientGuardianPhone = "(555) 123-4567",

                                                    PatientGuardian2FirstName = "Jim",
                                                    PatientGuardian2LastName = "Jones",
                                                    PatientGuardian2Relationship = "Father",
                                                    PatientGuardian2Phone = "(555) 123-4567",
                                                },
											AllowManualTime = false
										},
                                };

            initialDataResponse.ValidatedSessions = new List<ValidatedSession>
            {
                //													new ValidatedSession { ServerValidatedSessionID = 10, CaseID = 32, DateOfService = DateTime.Now.Date, StartTime = TimeSpan.FromHours(16), Duration = TimeSpan.FromMinutes(30), LocationID = 1, LocationDescription = "Location 1", ServiceID = 2, ServiceDescription = "Service 1" }
            };

            initialDataResponse.Settings = new List<KeyValuePair<string, object>>
                                                {
                                                    new KeyValuePair<string, object>("ServerVersion", "1.0.0.1234"),
                                                    new KeyValuePair<string, object>("MinimumClientVersion", "1.0.0.0"),
                                                    new KeyValuePair<string, object>("Communication.TimeoutSeconds", 15),
                                                    new KeyValuePair<string, object>("ActiveSession.Abandon.Enabled", true),
                                                    new KeyValuePair<string, object>("ActiveSession.Abandon.TimeoutMinutes", 360),
                                                    new KeyValuePair<string, object>("ActiveSession.DateCrossover.Cutoff.Enabled", true),
                                                    new KeyValuePair<string, object>("ActiveSession.DateCrossover.Cutoff.Notification", true),
                                                    new KeyValuePair<string, object>("ActiveSession.DateCrossover.Cutoff.NotificationMinutes", 10),
                                                    new KeyValuePair<string, object>("ActiveSession.GPS.TrackLeave", true),
                                                    new KeyValuePair<string, object>("TestFeature.TabNav.Enabled", true),
                                                    new KeyValuePair<string, object>("TestFeature.InPage.Enabled", true),
                                                    new KeyValuePair<string, object>("NoteEntry.AnswerValidationRegex", @"^\s*\S+(?:\s+\S+){4,}\s*$"),
                                                    new KeyValuePair<string, object>("NoteEntry.AnswerValidationMessage", "Please enter at least 5 words"),
                                                };

            return initialDataResponse;

        }

        
    }
}