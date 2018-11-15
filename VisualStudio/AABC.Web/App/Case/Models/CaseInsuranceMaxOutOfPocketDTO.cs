using System;

namespace AABC.Web.App.Case.Models
{
    public class CaseInsuranceMaxOutOfPocketDTO

    {
        public int Id { get; set; }
        public int CaseInsuranceId { get; set; }
        public decimal? MaxOutOfPocket { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}