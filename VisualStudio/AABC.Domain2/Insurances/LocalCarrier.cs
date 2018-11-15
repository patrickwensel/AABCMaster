namespace AABC.Domain2.Insurances
{
    public class LocalCarrier
    {
        public int ID { get; set; }
        public int InsuranceID { get; set; }
        public string Name { get; set; }

        public virtual Insurance Insurance { get; set; }
    }
}
