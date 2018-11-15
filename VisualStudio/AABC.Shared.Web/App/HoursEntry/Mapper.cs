using AABC.Data.V2;
using AABC.Domain2.Hours;
using AABC.Shared.Web.App.HoursEntry.Models.Request;
using System.Linq;

namespace AABC.Shared.Web.App.HoursEntry
{
    public class Mapper : BaseMapper<HoursEntryRequestVM>
    {
        public Mapper(CoreContext context) : base(context)
        {
        }

        protected override void MapAide(HoursEntryRequestVM request, Hours entry)
        {
            entry.Memo = request.Note;
        }

        protected override void MapBCBA(HoursEntryRequestVM request, Hours entry)
        {
            foreach (var en in request.ExtendedNotes?.AsEnumerable())
            {
                var existingNote = entry.ExtendedNotes.Where(x => x.TemplateID == en.TemplateID).SingleOrDefault();
                if (existingNote != null)
                {
                    existingNote.Answer = en.Answer;
                }
                else
                {
                    entry.ExtendedNotes.Add(new ExtendedNote { TemplateID = en.TemplateID, Answer = en.Answer });
                }
            }
        }
    }
}
