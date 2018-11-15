using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.HoursResolution
{
    public class HoursNotesValidator
    {
        public ValidationResults Validate(Domain2.Hours.Hours entry, EntryApp entryApp, bool isOnAideLegacyMode)
        {
            if (entry.Provider.IsAide && isOnAideLegacyMode)
            {
                if (string.IsNullOrWhiteSpace(entry.Memo))
                {
                    return new ValidationResults { IsValid = false, Errors = new[] { "Notes are required for this entry." } };
                }
                if (entryApp != EntryApp.ProviderApp && !ValidateMemo(entry.Memo))
                {
                    return new ValidationResults { IsValid = false, Errors = new[] { "Note must be at least 3 sentences, with at least 18 characters per sentence." } };
                }
            }
            else if (entry.Provider.IsBCBA && (entry.ExtendedNotes == null || entry.ExtendedNotes.Count < 3))
            {
                return new ValidationResults { IsValid = false, Errors = new[] { "A minimum of three notes are required for this entry." } };
            }
            return new ValidationResults { IsValid = true };
        }

        private static bool ValidateMemo(string memo)
        {
            return !(memo.Count(x => x == '.') < 3) && !(memo.Length < 3 * 18);
            //var sentences = memo.Trim().Split('.').Select(m => m.Trim()).Where(m => m.Length > 0);
            //return sentences.Count() >= 3 && sentences.All(m => m.Count() >= 18);
        }

    }

    public class ValidationResults
    {
        public bool IsValid { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
