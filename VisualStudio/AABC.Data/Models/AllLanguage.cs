namespace AABC.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public partial class AllLanguage
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string BiblioCode { get; set; }

        [StringLength(50)]
        public string TermCode { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string EnglishName { get; set; }

        [StringLength(50)]
        public string FrenchName { get; set; }
    }
}
