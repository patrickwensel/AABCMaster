namespace AABC.Domain2.Hours
{
    public class ExtendedNote
    {

        public int ID { get; set; }
        public int HoursID { get; set; }
        public int TemplateID { get; set; }
        public string Answer { get; set; }

        public virtual Hours HoursEntry { get; set; }
        public virtual ExtendedNoteTemplate Template { get; set; }


    }
}
