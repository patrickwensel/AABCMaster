namespace AABC.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public partial class CommonLanguage
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Description { get; set; }
    }
}
