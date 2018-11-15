namespace AABC.Domain.General
{

    public class Language
    {
        public int ID { get; set; }
        public string BiblioCode { get; set; }
        public string TermCode { get; set; }
        public string Code { get; set; }
        public string EnglishName { get; set; }
        public string FrenchName { get; set; }
    }

    public class GeneralLanguage
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

}
