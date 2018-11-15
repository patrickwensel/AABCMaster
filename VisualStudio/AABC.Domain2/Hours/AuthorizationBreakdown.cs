namespace AABC.Domain2.Hours
{
    public class AuthorizationBreakdown
    {

        public int ID { get; set; }
        
        public int HoursID { get; set; }
        public int AuthorizationID { get; set; }
        public int Minutes { get; set; }

        public virtual Hours HoursEntry { get; set; }
        public virtual Authorizations.Authorization Authorization { get; set; }


    }
}
