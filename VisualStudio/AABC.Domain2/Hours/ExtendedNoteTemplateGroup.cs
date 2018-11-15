using System.Collections.Generic;

namespace AABC.Domain2.Hours
{
    public class ExtendedNoteTemplateGroup
    {

        public int ID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ExtendedNoteTemplate> Notes { get; set; } = new List<ExtendedNoteTemplate>();

    }
}
