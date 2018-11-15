using System;
using System.Collections.Generic;

namespace AABC.Domain.Hours
{
    public class NoteTemplateGroup
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }

        public virtual ICollection<NoteTemplate> Templates { get; set; }
    }
}
