namespace AABC.Web.App.Case.Models
{
    public class GeneralHoursBilledListItem
    {
        public string Month { get; set; }
        public decimal? BCBAHours { get; set; }
        public decimal? AideHours { get; set; }
        public decimal? TotalHours { get; set; }
    }
}