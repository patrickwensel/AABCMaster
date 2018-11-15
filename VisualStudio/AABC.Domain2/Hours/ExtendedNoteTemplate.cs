namespace AABC.Domain2.Hours
{
    public class ExtendedNoteTemplate {

        public int ID { get; set; }
        public int GroupID { get; set; }
        public int ProviderTypeID { get; set; }
        public string Text { get; set; }
        public int DisplaySequence { get; set; }

        public virtual ExtendedNoteTemplateGroup Group { get; set; }
        public virtual Providers.ProviderType ProviderType { get; set; }

    }
}
