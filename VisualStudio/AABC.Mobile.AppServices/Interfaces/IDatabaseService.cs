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
	public interface IDatabaseService
	{
		Task WriteCurrentCases(List<Case> cases, string userName);

		Task WriteValidatedSessions(List<ValidatedSession> validatedSessions, string userName);

		Task<List<Case>> GetAllCases(string userName);

		Task WriteCaseValidation(CaseValidation caseValidation);

		Task<List<CaseValidation>> GetAllValidCaseValidations(Case thisCase);

		Task<CaseValidation> GetActiveCaseValidation();

		Task WriteCaseValidationQuestions(int caseValidationID, List<NoteQuestion> noteQuestions);

		Task<List<CaseValidationQuestion>> GetCaseValidationQuestions(int caseValidationID);

		Task<List<CaseValidationSignature>> GetCaseValidationSignatures(int caseValidationID);

		Task<string> GetCurrentAnswer(CaseValidationQuestion caseValidationQuestion);

		Task SaveAnswer(CaseValidationQuestion caseValidationQuestion, string answer);

		Task WriteSignature(CaseValidation sessionInProgress, SignatureType signatureType, byte[] signatureBytes);

		Task<List<CaseValidation>> GetAllDataToSendToServer();

		Task SaveSettings(IEnumerable<KeyValuePair<string, string>> settings);

		Task<IEnumerable<KeyValuePair<string, string>>> LoadSettings();
	}
}
