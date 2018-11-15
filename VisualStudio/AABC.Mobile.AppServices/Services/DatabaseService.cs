using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.SharedEntities.Entities;
using AABC.Mobile.SharedEntities.Messages;
using Polenter.Serialization;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Services
{
	public class DatabaseService : IDatabaseService
	{
		readonly ISQLite _sqlite;
		readonly SQLiteAsyncConnection _connection;

		const int _currentDatabaseVersion = 1;


		public DatabaseService(ISQLite sqlite)
		{
			_sqlite = sqlite;

			var connection = _sqlite.GetConnection();

			UpdateDatabase(connection);

			_connection = _sqlite.GetAsyncConnection();
		}

		void UpdateDatabase(SQLiteConnection connection)
		{
			// get the current version
			var dbVer = connection.ExecuteScalar<int>("pragma user_version;");

			if (dbVer == 0)
			{
				// create the initial database
				connection.CreateTable<StoredCase>();
				connection.CreateTable<CaseValidation>();
				connection.CreateTable<CaseValidationQuestion>();
				connection.CreateTable<CaseValidationQuestionAnswer>();
				connection.CreateTable<CaseValidationSignature>();
				connection.CreateTable<Setting>();

				connection.CreateIndex("CaseValidations", "CaseID");
				connection.CreateIndex("CaseValidationQuestions", new string[] { "CaseValidationID", "DisplayOrder" });
				connection.CreateIndex("CaseValidationSignatures", new string[] { "CaseValidationID", "SignatureType" }, true);
			}

			// use code such as this to update to the latest schema
			//// update to version 2
			//if (dbVer < 2)
			//{
			//	sqliteConnection.Execute("ALTER TABLE MyTable ADD COLUMN NewColumn TEXT");
			//}

			// set as the latest version
			if (dbVer != _currentDatabaseVersion)
			{
				dbVer = _currentDatabaseVersion;
				connection.Execute($"pragma user_version={dbVer};");
			}
		}


		public async Task WriteCurrentCases(List<Case> cases, string userName)
		{
			// delete any current cases as this will replace all of them
			await _connection.ExecuteAsync("delete from StoredCases where UserName=?", userName);

			var serializer = new SharpSerializer(true);
			foreach (var thisCase in cases)
			{
				using (var outputData = new MemoryStream())
				{
					// serialize this case
					serializer.Serialize(thisCase, outputData);

					// write the buffer to the database
					await _connection.InsertAsync(new StoredCase { CaseID = thisCase.ID, UserName = userName, CaseDetails = outputData.ToArray() });
				}
			}
		}

		public async Task WriteValidatedSessions(List<ValidatedSession> validatedSessions, string userName)
		{
			// delete any sessions that are no longer in the validated sessions
			// don't delete anything that is in a state indicating it has started
			var existingSessions = await _connection.QueryAsync<CaseValidation>("select * from CaseValidations where UserName=? and State=?", userName, CaseValidationState.Valid);
			foreach (var existingSession in existingSessions)
			{
				if (validatedSessions.All(v => v.ServerValidatedSessionID != existingSession.ServerValidatedSessionID))
				{
					// this no longer exists, so can be deleted
					await _connection.ExecuteAsync("delete from CaseValidations where CaseValidationID=?", existingSession.CaseValidationID);
				}
			}

			// update the current sessions with these validated sessions.
			foreach (var validatedSession in validatedSessions)
			{
				// have we currently got a session that matches this sessions ValidatedSessionId
				var storedSession = (await _connection.QueryAsync<CaseValidation>("select * from CaseValidations where ServerValidatedSessionID=?", validatedSession.ServerValidatedSessionID)).FirstOrDefault();

				if (storedSession == null)
				{
					// this is a new validated session, so add it
					validatedSession.UserName = userName;
					await _connection.InsertAsync(new CaseValidation(validatedSession) { State = CaseValidationState.Valid });
				}
				else
				{
					// Do not overwrite any session that isn't in the CaseValidationState.Valid state
					if (storedSession.State == CaseValidationState.Valid)
					{
						// update from the server details
						storedSession.CaseID = validatedSession.CaseID;
						storedSession.DateOfService = validatedSession.DateOfService;
						storedSession.StartTime = validatedSession.StartTime;
						storedSession.Duration = validatedSession.Duration;
						storedSession.ServiceID = validatedSession.ServiceID;
						storedSession.LocationID = validatedSession.LocationID;
						storedSession.LocationDescription = validatedSession.LocationDescription;
						storedSession.ServiceDescription = validatedSession.ServiceDescription;
						await _connection.UpdateAsync(storedSession);
					}
				}
			}
		}

		public async Task<List<Case>> GetAllCases(string userName)
		{
			var storedCases = await _connection.QueryAsync<StoredCase>("select * from StoredCases where UserName=?", userName);

			var cases = new List<Case>();

			var serializer = new SharpSerializer(true);
			foreach (var storedCase in storedCases)
			{
				using (var memoryStream = new MemoryStream(storedCase.CaseDetails))
				{
					var thisCase = serializer.Deserialize(memoryStream) as Case;
					thisCase.UserName = userName;
					cases.Add(thisCase);
				}
			}

			return cases;
		}

		public async Task WriteCaseValidation(CaseValidation caseValidation)
		{
			// write to the database
			if (caseValidation.CaseValidationID == 0)
			{
				await _connection.InsertAsync(caseValidation);
			}
			else
			{
				await _connection.UpdateAsync(caseValidation);
			}
		}

		public async Task<List<CaseValidation>> GetAllValidCaseValidations(Case thisCase)
		{
			var today = DateTime.Now.Date;

			var caseValidations = (await _connection.QueryAsync<CaseValidation>("select * from CaseValidations where CaseID=? and UserName=? and State=?", thisCase.ID, thisCase.UserName, CaseValidationState.Valid))
									.Where(c => c.DateOfService >= today)
									.OrderBy(c => c.DateOfService)
									.ToList();

			foreach (var caseValidation in caseValidations)
			{
				caseValidation.Case = thisCase;
			}	

			return caseValidations;
		}

		public async Task<CaseValidation> GetActiveCaseValidation()
		{
			var caseValidation = (await _connection.QueryAsync<CaseValidation>("select * from CaseValidations where State=?", CaseValidationState.Active)).FirstOrDefault();

			if (caseValidation != null)
			{
				// get the stored case
				var storedCase = (await _connection.QueryAsync<StoredCase>("select * from StoredCases where CaseID=? and UserName=?", caseValidation.CaseID, caseValidation.UserName)).FirstOrDefault();

				// and deserialize it
				var serializer = new SharpSerializer(true);
				using (var memoryStream = new MemoryStream(storedCase.CaseDetails))
				{
					caseValidation.Case = serializer.Deserialize(memoryStream) as Case;
				}
			}
			
			return caseValidation;
		}

		public async Task WriteCaseValidationQuestions(int caseValidationID, List<NoteQuestion> noteQuestions)
		{
			int displayOrder = 0;
			foreach (var noteQuestion in noteQuestions)
			{
				var caseValidationQuestion = new CaseValidationQuestion
									{
										CaseValidationID = caseValidationID,
										DisplayOrder = displayOrder,
										NoteQuestionID = noteQuestion.NoteQuestionID,
										Question = noteQuestion.Question
									};

				await _connection.InsertAsync(caseValidationQuestion);

				displayOrder++;
			}
		}

		public async Task<List<CaseValidationQuestion>> GetCaseValidationQuestions(int caseValidationID)
		{
			return await _connection.QueryAsync<CaseValidationQuestion>("select * from CaseValidationQuestions where CaseValidationID=? order by DisplayOrder", caseValidationID);
		}

		public async Task<string> GetCurrentAnswer(CaseValidationQuestion caseValidationQuestion)
		{
			CaseValidationQuestionAnswer caseValidationQuestionAnswer = await GetCaseValidationQuestionAnswer (caseValidationQuestion);

			return caseValidationQuestionAnswer?.Answer;
		}

		public async Task SaveAnswer(CaseValidationQuestion caseValidationQuestion, string answer)
		{
			CaseValidationQuestionAnswer caseValidationQuestionAnswer = await GetCaseValidationQuestionAnswer (caseValidationQuestion);

			if (caseValidationQuestionAnswer == null)
			{
				await _connection.InsertAsync(new CaseValidationQuestionAnswer { CaseValidationQuestionID = caseValidationQuestion.CaseValidationQuestionID, Answer = answer });
			}
			else
			{
				caseValidationQuestionAnswer.Answer = answer;
				await _connection.UpdateAsync(new CaseValidationQuestionAnswer { CaseValidationQuestionID = caseValidationQuestion.CaseValidationQuestionID, Answer = answer });
			}
		}

		async Task<CaseValidationQuestionAnswer> GetCaseValidationQuestionAnswer(CaseValidationQuestion caseValidationQuestion)
		{
			return (await _connection.QueryAsync<CaseValidationQuestionAnswer>("select * from CaseValidationQuestionAnswers where CaseValidationQuestionID=?", caseValidationQuestion.CaseValidationQuestionID)).FirstOrDefault();
		}

		public async Task WriteSignature(CaseValidation sessionInProgress, SignatureType signatureType, byte[] signatureBytes)
		{
			var currentSignature = (await _connection.QueryAsync<CaseValidationSignature>("select * from CaseValidationSignatures where CaseValidationID=? and SignatureType=?", sessionInProgress.CaseValidationID, (int)signatureType)).FirstOrDefault();

			// if the signature exists for this case
			if (currentSignature == null)
			{
				// update it, 
				await _connection.InsertAsync(new CaseValidationSignature { CaseValidationID = sessionInProgress.CaseValidationID, SignatureType = signatureType, Signature = signatureBytes });
			}
			else
			{
				// otherwise overwrite it
				currentSignature.Signature = signatureBytes;

				await _connection.UpdateAsync(currentSignature);
			}
		}

		public async Task<List<CaseValidationSignature>> GetCaseValidationSignatures(int caseValidationID)
		{
			return (await _connection.QueryAsync<CaseValidationSignature>("select * from CaseValidationSignatures where CaseValidationID=? order by SignatureType", caseValidationID)).ToList();
		}

		public async Task<List<CaseValidation>> GetAllDataToSendToServer()
		{
			return await _connection.QueryAsync<CaseValidation>("select * from CaseValidations where State=? or State=?", CaseValidationState.CompletedAwaitingSendToServer, CaseValidationState.AbandonedAwaitingSendToServer);
		}

		public async Task<IEnumerable<KeyValuePair<string, string>>> LoadSettings()
		{
			var settings = await _connection.Table<Setting>().ToListAsync();

			return settings.Select(f => new KeyValuePair<string, string>(f.Key, f.JsonSerializedValue));
		}

		public async Task SaveSettings(IEnumerable<KeyValuePair<string, string>> settings)
		{
			// delete all the existing Settings
			await _connection.ExecuteAsync("delete from Settings");
			await _connection.InsertAllAsync(settings.Select(f => new Setting{ Key = f.Key, JsonSerializedValue = f.Value }));
		}
	}
}