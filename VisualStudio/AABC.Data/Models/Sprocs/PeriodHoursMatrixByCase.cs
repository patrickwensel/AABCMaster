namespace AABC.Data.Models.Sprocs
{
    public class PeriodHoursMatrixByCase
    {

        public int CaseID { get; set; }
        public string HoursType { get; set; }
        public decimal TotalHours { get; set; }
        public decimal BillableHours { get; set; }
        public decimal PayableHours { get; set; }

    }
}
