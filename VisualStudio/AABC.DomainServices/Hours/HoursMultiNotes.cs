using AABC.Domain.Hours;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AABC.DomainServices.Hours
{
    [Obsolete("Use DomainServices.Providers.ExtendedNotesTemplateResolver")]
    public class HoursMultiNotes
    {


        AABC.Data.Models.CoreEntityModel context;

        public HoursMultiNotes() {
            this.context = new Data.Models.CoreEntityModel();
        }

        /// <summary>
        /// Get a list of notes keyed in a dictionary by the group name that they were generated from.
        /// E.g., Parent Training key with list of applicable notes
        /// </summary>
        /// <param name="providerTypeID"></param>
        /// <returns></returns>
        public Dictionary<string, List<Note>> GenerateFromTemplates(int providerTypeID) {
            
            var groups = context.HoursNoteTemplateGroups.ToList();

            var result = new Dictionary<string, List<Note>>();

            foreach (var group in groups) {

                var items = new List<Note>();

                foreach (var template in group.HoursNoteTemplates.Where(x => x.TemplateProviderTypeID == providerTypeID)) {
                    var modelTemplate = new Domain.Hours.NoteTemplate();
                    modelTemplate.ID = template.ID;
                    modelTemplate.Text = template.TemplateText;
                    items.Add(new Note() { Template = modelTemplate, TemplateID = template.ID });
                }

                result.Add(group.GroupName, items);
            }
            
            return result;
        }



        public Dictionary<string, List<Note>> GetExistingNotesWithTemplateMerge(int providerTypeID, int hoursID) {

            // a list of all notes we'll need
            var templatedNotes = GenerateFromTemplates(providerTypeID);

            // a list of any existing notes that we have
            var existingNotes = context.CaseAuthHoursNotes.Where(x => x.HoursID == hoursID);
            
            foreach (var note in existingNotes) {

                // grab a reference to the note instance in the templated notes, if we have a match
                var templatedNote = findInTemplates(templatedNotes, note.NotesTemplateID);

                if (templatedNote != null) {
                    // update that templated note instance to hold the values from our existing note
                    templatedNote.Answer = note.NotesAnswer;
                    templatedNote.DateCreated = note.DateCreated;
                    templatedNote.HoursID = note.HoursID;
                    templatedNote.ID = note.ID;
                    templatedNote.TemplateID = note.NotesTemplateID;
                }                    
            }

            // then return the templated notes, which have been updated accordingly with existing notes
            return templatedNotes;
            
        }

        private Note findInTemplates(Dictionary<string, List<Note>> templatedNotes, int templateID) {

            foreach (KeyValuePair<string, List<Note>> group in templatedNotes) {

                foreach (Note note in group.Value) {
                    if (note.TemplateID == templateID) {
                        return note;
                    }
                }

            }

            // nothing found
            return null;

        }



    }
}
