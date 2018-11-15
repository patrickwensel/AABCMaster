using System;

namespace AABC.Domain2.Infrastructure
{
    public class Language
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }

    //ID DateCreated rv LangIsActive    LangBiblioCode LangTermCode    LangCommonCode LangEnglishName LangFrenchName
}
