using System;

namespace AABC.Domain.Hours
{




    public class Note
    {

        // THIS IS A FEATURE TOGGLE!  Until I get a better feature toggle system in place,
        // it lives here...
        private const bool BCBA_EXTENDED_NOTES = true;

        public static bool UseExtendedNotes {
            get
            {
                return BCBA_EXTENDED_NOTES;
            }
        }


        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int HoursID { get; set; }
        public Cases.CaseAuthorizationHours Hours { get; set; }

        public int TemplateID { get; set; }
        public NoteTemplate Template { get; set; }

        public string Answer { get; set; }
        
    }
}
