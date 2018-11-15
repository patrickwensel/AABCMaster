namespace AABC.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Number
    {
        [Key]
        [Column("Number")]
        public int Number1 { get; set; }
    }
}
