using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.SharedEntities.Entities;
using AABC.Mobile.SharedEntities.Messages;
using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.AppServices.Services
{
	public class SessionUpdateService : ISessionUpdateService
	{
		readonly IDatabaseService _databaseService;
		readonly IDataUpdateService _dataUpdateService;
		readonly IConnectivity _connectivity;

		public SessionUpdateService(IDatabaseService databaseService, IDataUpdateService dataUpdateService, IConnectivity connectivity)
		{
			_databaseService = databaseService;
			_dataUpdateService = dataUpdateService;
			_connectivity = connectivity;
		}

		public void Start()
		{
			// every n minutes try and send the data to the server
			Device.StartTimer(TimeSpan.FromMinutes(5), () =>
							{
								Device.BeginInvokeOnMainThread(() => SendDataToServer().IgnoreResult() );
								return true;
							});

			// and do it now (we should be in the main thread
			SendDataToServer().IgnoreResult();
		}


		object _sendDataToServerAlreadyRunningLock = new object();
		bool _sendDataToServerAlreadyRunning;

		public async Task SendDataToServer()
		{
			// there's no point in trying if we're not connected
			if (!_connectivity.IsConnected) return;

			// if we're already doing this then just return, 
			// but indicate that we want to run again, because we don't know that we don't
			if (_sendDataToServerAlreadyRunning) return;

			try
			{
				// set the _sendDataToServerAlreadyRunning flag
				lock(_sendDataToServerAlreadyRunningLock)
				{
					if (_sendDataToServerAlreadyRunning)
					{
						return;
					}
					else
					{
						_sendDataToServerAlreadyRunning = true;
					}
				}

				// get the data to update
				var caseValidations = await _databaseService.GetAllDataToSendToServer();

				// for each caseValidation
				foreach (var caseValidation in caseValidations)
				{
					try
					{
						var sessionUpdateRequest = new SessionUpdateRequest
											{
												SessionDetails = caseValidation,
												QuestionResponses = new List<NoteQuestionResponse>(),
												Base64Signatures = new List<string>()
											};

						// get the data for this case...
						// get the questions 
						var questions = await _databaseService.GetCaseValidationQuestions(caseValidation.CaseValidationID);

						// and corresponding answers
						foreach (var question in questions)
						{
							var answer = await _databaseService.GetCurrentAnswer(question);
							sessionUpdateRequest.QuestionResponses.Add(new NoteQuestionResponse { NoteQuestionID = question.NoteQuestionID, Answer = answer} );
						}

						// and the signatures (as base 64 signatures)
						var signatures = await _databaseService.GetCaseValidationSignatures(caseValidation.CaseValidationID);
						sessionUpdateRequest.Base64Signatures.Add(signatures.Where(s => s.SignatureType == SignatureType.Provider).Select(s => Convert.ToBase64String(s.Signature)).FirstOrDefault());
						sessionUpdateRequest.Base64Signatures.Add(signatures.Where(s => s.SignatureType == SignatureType.Parent).Select(s => Convert.ToBase64String(s.Signature)).FirstOrDefault());

						// send it to the server
						var sessionUpdateResponse = await _dataUpdateService.PostSessionUpdateRequestToServer(sessionUpdateRequest);

						if (sessionUpdateResponse.Success)
						{
							// this was successful, so don't send again
							caseValidation.State = (caseValidation.State == CaseValidationState.AbandonedAwaitingSendToServer)
													? CaseValidationState.AbandonedSentToServer
													: CaseValidationState.CompletedSentToServer;

							await _databaseService.WriteCaseValidation(caseValidation);
						}
					}
					catch
					{
						// ignore any exceptions and move onto the next request
					}
				}
			}
			finally
			{
				_sendDataToServerAlreadyRunning = false;
			}
		}

	}
}
