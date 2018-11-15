using AABC.Domain2.Hours;
using AABC.Domain2.Providers;
using System.Collections.Generic;
using System.Linq;


namespace AABC.DomainServices.Providers
{
    public class ExtendedNotesTemplateResolver
    {

        private static Data.V2.CoreContext _context { get { return ContextProvider.Context; } }

        public static IEnumerable<ExtendedNote> GetExtendedNotesBlankMatrix(ProviderTypeIDs providerType)
        {
            var templates = _context.ExtendedNoteTemplates
                .Where(x => x.ProviderTypeID == (int)providerType)
                .ToList()
                .Select(template => new ExtendedNote
                {
                    Template = template,
                    TemplateID = template.ID
                });
            return templates;
        }


        public static List<ExtendedNote> GetExtendedNotesMatrix(int hoursID)
        {
            var hoursEntry = _context.Hours.Find(hoursID);
            // get a list of all applicable templates
            var templates = _context.ExtendedNoteTemplates.Where(x => x.ProviderTypeID == hoursEntry.Provider.ProviderTypeID).ToList();
            // get a matching set of answers, where applicable
            var existingNotes = _context.ExtendedNotes.Where(x => x.HoursID == hoursID).ToList();
            var results = new List<ExtendedNote>();
            foreach (var template in templates)
            {
                var matchedNote = existingNotes.Where(x => x.TemplateID == template.ID).SingleOrDefault();
                if (matchedNote == null)
                {
                    results.Add(new ExtendedNote
                    {
                        HoursID = hoursID,
                        TemplateID = template.ID,
                        Template = template
                    });
                }
                else
                {
                    results.Add(matchedNote);
                }
            }
            return results;
        }


    }
}
