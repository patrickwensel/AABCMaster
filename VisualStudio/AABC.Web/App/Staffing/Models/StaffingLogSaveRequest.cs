namespace AABC.Web.App.Staffing.Models
{
    public class StaffingLogSaveRequest
    {
        public int ID { get; set; }
        public string ParentalRestaffRequest { get; set; }
        public decimal? HoursOfABATherapy { get; set; }
        public string AidesRespondingNo { get; set; }
        public string AidesRespondingMaybe { get; set; }
        public int ScheduleRequest { get; set; }
        public int[] SpecialAttentionNeedIds { get; set; }
        public char? ProviderGenderPreference { get; set; }
    }
}