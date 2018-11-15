using System;

namespace AABC.Domain.Hours
{
    public class NoteTemplate
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int NoteTemplateGroupID { get; set; }
        public NoteTemplateGroup NoteTemplateGroup { get; set; }

        public int? ProviderTypeID { get; set; }
        public Providers.ProviderType ProviderType { get; set; }

        public int? ServiceID { get; set; }
        public Cases.Service Service { get; set; }

        public string Text { get; set; }
        public string TextDescription { get; set; }

    }

}
